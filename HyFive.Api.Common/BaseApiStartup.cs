using HyFive.Api.Common.ExtensionMethods;
using HyFive.Api.Common.HealthChecks;
using HyFive.Api.Common.Logging;
using HyFive.DataAccess;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Configuration;
using HyFive.Services.Authentication.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.Packaging;
using HyFive.Api.Common.Infrastructure.Helpers;
using System.Threading.Tasks;
using System.Linq;

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

            services.AddControllers();
            services.AddHttpContextAccessor();
            services.Configure<HandhygieneConfiguration>(_handHygieneConfigSection);
            services.Configure<RedirectPagesSettings>(Configuration.GetSection("RedirectPagesSettings"));
            services.AddCors();
            services.AddServices(Configuration, ApiTitle, ApiType);

            // Database-context
            services.AddDbContext<HandHygieneContext>(dboptions => {
                dboptions.UseNpgsql(Configuration.GetConnectionString(HandHygieneConnection),
                    sqloptions => {
                        sqloptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    });
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

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
                   var clientSecret = Configuration["OpenIdConnect:ClientSecret"];

                   
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
                               var request = context.Request;
                               var scheme = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault() ?? context.Request.Scheme;
                               var host = context.Request.Headers["X-Forwarded-Host"].FirstOrDefault() ?? context.Request.Host.ToString();
                               var redirectUri = $"{request.Scheme}://{request.Host}/signin-oidc";
                               context.ProtocolMessage.RedirectUri = redirectUri;
                           }
                           return Task.CompletedTask;
                       }
                   };
               });
               /*.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.Authority = portalSettings.Security.OpenIdConnect.Authority;
                   options.Audience = portalSettings.JwtBearerAudience;
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidIssuer = portalSettings.Security.JwtBearerValidIssuer,
                       RoleClaimType = portalSettings.Security.ClaimTypes.RoleClaimType
                   };
               });*/
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IAuthorizationHandler, UserTypeRequirementHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(HandhygienePolicy.Coordinator, policy =>
                    policy.Requirements.Add(new UserTypeRequirement(UserType.Coordinator)));
                options.AddPolicy(HandhygienePolicy.Observer, policy =>
                    policy.Requirements.Add(new UserTypeRequirement(UserType.Observer)));
                options.AddPolicy(HandhygienePolicy.FhiAdmin, policy =>
                    policy.Requirements.Add(new UserTypeRequirement(UserType.FhiAdmin)));
                options.AddPolicy(HandhygienePolicy.FhiAdminOrCoordinator, policy =>
                {
                    policy.Requirements.Add(new UserTypeRequirement(UserType.FhiAdminOrCoordinator));
                });

            });

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                                            Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;

                // Optional: Clear default restrictions (recommended in Azure)
                options.KnownNetworks.Clear(); // Remove the default loopback network restriction
                options.KnownProxies.Clear();  // Remove the default loopback proxy restriction
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
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            InitializeDatabase(app);
            //app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            var staticFileOptions = new StaticFileOptions() { ContentTypeProvider = provider };
            if (_handHygieneConfiguration.CacheStaticAssets == false)
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

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                spa.Options.DefaultPageStaticFileOptions = staticFileOptions;
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        protected void InitializeDatabase(IApplicationBuilder app)
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