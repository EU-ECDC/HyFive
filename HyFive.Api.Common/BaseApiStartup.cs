using HyFive.Api.Common.ExtensionMethods;
using HyFive.Api.Common.Infrastructure.Helpers;
using HyFive.Api.Common.Logging;
using HyFive.DataAccess;
using HyFive.Services.Authentication.Configuration;
using HyFive.Services.Authentication.Requirements;
using HyFive.Services.Authentication.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HyFive.Api.Common
{
    public abstract class BaseApiStartup
    {
        protected abstract string ApiTitle { get; }
        protected abstract Type ApiType { get; }
        private const string HandHygieneConnection = "HandHygieneConnection";
        public const string AppInsightsConnectionStringVariable = "APPLICATIONINSIGHTS_CONNECTION_STRING";

        protected readonly IConfigurationSection _handHygieneConfigSection;
        protected readonly IConfigurationSection _redirectPagesSettingsSection;
        protected readonly IConfigurationSection _dataProtectionConfigSection;
        protected readonly HandhygieneConfiguration _handHygieneConfiguration;
        protected readonly RedirectPagesSettings _redirectPagesSettings;

        public BaseApiStartup(IConfiguration configuration)
        {
            Configuration = configuration;           
           

            _handHygieneConfigSection = Configuration.GetSection(nameof(HandhygieneConfiguration));
            _handHygieneConfiguration = _handHygieneConfigSection.Get<HandhygieneConfiguration>();

            _redirectPagesSettingsSection = Configuration.GetSection(nameof(RedirectPagesSettings));
            _redirectPagesSettings = _redirectPagesSettingsSection.Get<RedirectPagesSettings>();

            TestDatabaseConnection();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            var appInsightsConnectionString =
                Environment.GetEnvironmentVariable(AppInsightsConnectionStringVariable);

            if (!string.IsNullOrEmpty(appInsightsConnectionString))
            {
                services.AddApplicationInsightsTelemetry(Configuration);
            }

            // Retrieve application settings to use in configuring services
            var securitySettings = Configuration.Get<SecuritySettings>();

            // Core services
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddLocalization();

            // Configure request localization (extracted)
            ConfigureRequestLocalization(services);

            // Config sections and project services
            services.Configure<HandhygieneConfiguration>(_handHygieneConfigSection);
            services.Configure<RedirectPagesSettings>(Configuration.GetSection("RedirectPagesSettings"));
            services.AddCors();
            services.AddServices(Configuration, ApiTitle, ApiType);

            // Database-context (extracted)
            ConfigureDatabase(services);

            // Authentication (cookies + OpenID Connect) (extracted)
            ConfigureAuthentication(services, securitySettings);

            // Application services and authorization (extracted)
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthorizationHandler, UserTypeRequirementHandler>();
            ConfigureAuthorizationPolicies(services);

            // SPA static files
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        private static void ConfigureRequestLocalization(IServiceCollection services)
        {
            var supportedCultures = new[] { "el", "en" };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<HandHygieneContext>(dboptions =>
            {
                dboptions.UseNpgsql(Configuration.GetConnectionString(HandHygieneConnection),
                    sqloptions =>
                    {
                        sqloptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    });
            });
        }

        private void ConfigureAuthentication(IServiceCollection services, SecuritySettings securitySettings)
        {
            SameSiteMode sameSiteMode = securitySettings.OpenIdConnect.SameSiteMode switch
            {
                "none" => SameSiteMode.None,
                "lax" => SameSiteMode.Lax,
                "strict" => SameSiteMode.Strict,
                "unspecified" => SameSiteMode.Unspecified,
                _ => SameSiteMode.None
            };

            CookieSecurePolicy cookiePolicy = securitySettings.OpenIdConnect.CookieSecurePolicy switch
            {
                "none" => CookieSecurePolicy.None,
                "sameasrequest" => CookieSecurePolicy.SameAsRequest,
                "always" => CookieSecurePolicy.Always,
                _ => CookieSecurePolicy.Always
            };

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = sameSiteMode;
                    options.Cookie.SecurePolicy = cookiePolicy;

                    options.AccessDeniedPath = "/Forbidden";
                })
               .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.NonceCookie.SameSite = SameSiteMode.None;
                    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Authority = securitySettings.OpenIdConnect.Authority;
                    options.ClientId = securitySettings.OpenIdConnect.ClientId;
                    // Only disable https requirement on specifically "false" value
                    options.RequireHttpsMetadata = securitySettings.OpenIdConnect.RequireHttpsMetadata;

                    if (Configuration["OpenIdConnect:SaveTokens"] == "true")
                    {
                        // Persist tokens so we can supply the id_token as an id_token_hint when signing out
                        options.SaveTokens = true;
                    }
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                    options.SignedOutCallbackPath = new PathString(_redirectPagesSettings.LoggedOut);
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = securitySettings.ClaimTypes.RoleClaimType
                    };
                    options.Events = new OpenIdConnectEvents
                    {
                        OnAuthorizationCodeReceived = async context =>
                        {
                            var request = context.TokenEndpointRequest;
                            request.ClientSecret = null;

                            var clientSecret = Configuration["OpenIdConnect:ClientSecret"];
                            var creds = Convert.ToBase64String(
                                System.Text.Encoding.ASCII.GetBytes($"{context.Options.ClientId}:{clientSecret}"));

                            context.Backchannel.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", creds);
                        },

                        OnRedirectToIdentityProvider = context =>
                        {
                            if (context.Request != null && Helper.IsAjaxRequest(context.Request))
                            {
                                context.HttpContext.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";
                                context.HttpContext.Response.WriteAsync("{data:'access denied - ajax call' }");
                                context.HandleResponse();
                            }
                            else
                            {
                                var redirectUri = _redirectPagesSettings.RedirectLogInUri;
                                context.ProtocolMessage.RedirectUri = redirectUri;
                            }
                            return Task.CompletedTask;
                        },

                        OnRedirectToIdentityProviderForSignOut = async context =>
                        {
                            var postLogoutRedirectLogOutUri = _redirectPagesSettings.RedirectLogOutUri;

                            context.ProtocolMessage.PostLogoutRedirectUri = postLogoutRedirectLogOutUri;

                            // Try to include the id_token as an id_token_hint
                            try
                            {
                                var idToken = await context.HttpContext.GetTokenAsync("id_token");
                                if (!string.IsNullOrEmpty(idToken))
                                {
                                    context.ProtocolMessage.IdTokenHint = idToken;
                                }
                            }
                            catch
                            {
                                // Silently continue on failure in case the
                                // provider accepts client_id.
                            }
                        }
                    };
                });
        }

        private static void ConfigureAuthorizationPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(HandhygienePolicy.Coordinator, policy =>
                    policy.Requirements.Add(new UserTypeRequirement(UserType.Coordinator)));
                options.AddPolicy(HandhygienePolicy.Observer, policy =>
                    policy.Requirements.Add(new UserTypeRequirement(UserType.Observer)));
                options.AddPolicy(HandhygienePolicy.Admin, policy =>
                    policy.Requirements.Add(new UserTypeRequirement(UserType.Admin)));
                options.AddPolicy(HandhygienePolicy.AdminOrCoordinator, policy =>
                {
                    policy.Requirements.Add(new UserTypeRequirement(UserType.AdminOrCoordinator));
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            InitializeDatabase(app);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            var staticFileOptions = new StaticFileOptions() { ContentTypeProvider = provider };
            if (!_handHygieneConfiguration.CacheStaticAssets)
            {
                staticFileOptions.OnPrepareResponse = (context) =>
                {
                    // Disable caching of all static files.
                    context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store";
                    context.Context.Response.Headers["Pragma"] = "no-cache";
                    context.Context.Response.Headers["Expires"] = "-1";
                };
            }

            app.UseStaticFiles(staticFileOptions);

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles(staticFileOptions);
            }
            app.UseSwagger(ApiTitle);
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPathAnonymized} responded {StatusCode} in {Elapsed:0.0000} ms";
            });

            app.UseRouting();
            
            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);
            

            app.Use(async (context, next) =>
            {
                // If redirected from PingOne logout to root, redirect to /profile
                if (context.Request.Path == "/" && !context.User.Identity.IsAuthenticated)
                {
                    context.Response.Redirect("/profile");
                    return;
                }

                await next();
            });

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMiddleware<HyFive.Api.Common.Middleware.ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.DefaultPageStaticFileOptions = staticFileOptions;
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }

        protected static void InitializeDatabase(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
            using var context = scope?.ServiceProvider.GetRequiredService<HandHygieneContext>();
            context.Database.Migrate();
        }

        /// <summary>
        /// Azure SQL per-second database has an annoying tendency to go dormant, and the first connection attempt when the database is dormant always fails.
        /// It can take up to 1 minute to start the database. We therefore try to connect up to X times, to make sure the database is online before we run Migrate.
        /// </summary>
        private void TestDatabaseConnection()
        {
            var connectionString = Configuration.GetConnectionString(HandHygieneConnection);
            var opts = new DbContextOptionsBuilder<HandHygieneContext>();
            opts.UseNpgsql(connectionString);
            using var context = new HandHygieneContext(opts.Options);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            var connectionAttempts = 1;
            var connectionAttemptLimit = 5;
            while (connectionAttempts <= connectionAttemptLimit)
            {
                try
                {
                    Log.Logger.Information($"Database connection attempt #{connectionAttempts}/{connectionAttemptLimit} ");
                    var connectionIsSuccessful = context.Database.CanConnect();
                    if (connectionIsSuccessful)
                    {
                        Log.Logger.Information($"Database connection successful - no more connection attempts necessary");
                        break;
                    }
                }
                catch (Exception)
                {
                    Log.Logger.Error($"Database connection failed (Attempt #{connectionAttempts}/{connectionAttemptLimit} )");
                }
                connectionAttempts++;
            }
        }
    }
}