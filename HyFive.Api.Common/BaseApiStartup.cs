using HyFive.Api.Common.ExtensionMethods;
using HyFive.Api.Common.HealthChecks;
using HyFive.Api.Common.Logging;
using HyFive.Dataaksess;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Konfigurasjon;
using HyFive.Tjenester.Autentisering.Requirements;
using Fhi.HelseId.Web;
using Fhi.HelseId.Web.ExtensionMethods;
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

namespace HyFive.Api.Common
{
    public abstract class BaseApiStartup
    {
        protected abstract string ApiTittel { get; }
        protected abstract Type ApiType { get; }
        private const string HandhygieneConnection = "HandhygieneConnection";
        public const string AppInsightsConnectionStringVariable = "APPLICATIONINSIGHTS_CONNECTION_STRING";

        protected readonly IConfigurationSection _helseIdConfigurationSection;
        protected readonly IConfigurationSection _redirectPagesConfigurationSection;
        protected readonly IConfigurationSection _dataprotectionConfigSection;
        protected readonly HandhygieneHelseIdKonfigurasjon _handhygieneHelseIdConfiguration;
        protected readonly RedirectPagesKonfigurasjon _redirectPagesConfiguration;

        public BaseApiStartup(IConfiguration configuration)
        {
            Configuration = configuration;

            var webConfig = Configuration.GetSection(nameof(HelseIdWebKonfigurasjon)).Get<HelseIdWebKonfigurasjon>() ?? throw new Exception(nameof(HelseIdWebKonfigurasjon));

            _helseIdConfigurationSection = Configuration.GetSection(nameof(HandhygieneHelseIdKonfigurasjon));
            _handhygieneHelseIdConfiguration = _helseIdConfigurationSection.Get<HandhygieneHelseIdKonfigurasjon>();

            _redirectPagesConfigurationSection = Configuration.GetSection(nameof(RedirectPagesKonfigurasjon));
            _redirectPagesConfiguration = _redirectPagesConfigurationSection.Get<RedirectPagesKonfigurasjon>();
            
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

            services.AddHttpContextAccessor();
            services.Configure<HandhygieneHelseIdKonfigurasjon>(_helseIdConfigurationSection);
            services.Configure<RedirectPagesKonfigurasjon>(_redirectPagesConfigurationSection);
            services.AddCors();
            services.LeggTilTjenester(Configuration, ApiTittel, ApiType);

            // Database-context
            services.AddDbContext<HandhygieneContext>(dboptions => {
                dboptions.UseNpgsql(Configuration.GetConnectionString(HandhygieneConnection),
                    sqloptions => {
                        sqloptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    });
            });

            services.AddHelseIdWebAuthentication(Configuration).UseJwkKeySecretHandler().Build();
            services.AddScoped<IBrukerService ,BrukerService>();

            services.AddScoped<IAuthorizationHandler, BrukertypeRequirementHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy(HandhygienePolicy.Koordinator, policy =>
                    policy.Requirements.Add(new BrukertypeRequirement(Brukertype.Koordinator)));
                options.AddPolicy(HandhygienePolicy.Observator, policy =>
                    policy.Requirements.Add(new BrukertypeRequirement(Brukertype.Observator)));
                options.AddPolicy(HandhygienePolicy.FhiAdmin, policy =>
                    policy.Requirements.Add(new BrukertypeRequirement(Brukertype.FhiAdmin)));
                options.AddPolicy(HandhygienePolicy.FhiAdminEllerKoordinator, policy =>
                {
                    policy.Requirements.Add(new BrukertypeRequirement(Brukertype.FhiAdminEllerKoordinator));
                });
             
            });

            LeggTilHelsesjekker(services);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
                
            });
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            InitializeDatabase(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";
            var staticFileOptions = new StaticFileOptions() {ContentTypeProvider = provider};
            if(_handhygieneHelseIdConfiguration.CacheStaticAssets == false){
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
            app.BrukSwagger(ApiTittel);
            app.UseSerilogRequestLogging(opts
                =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.MessageTemplate =
                    "HTTP {RequestMethod} {RequestPathAnonymized} responded {StatusCode} in {Elapsed:0.0000} ms";
            });

            app.UseRouting();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            
            app.UseAuthentication();

            if (env.ApplicationName.Contains("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                app.UseHelseIdProtectedPaths(new List<PathString> { "/signin-callback", "/signout-callback", "/api/User/Logout" });
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");    
               
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

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
            using var context = scope?.ServiceProvider.GetRequiredService<HandhygieneContext>();
            context.Database.Migrate();
        }

        /// <summary>
        /// Azure SQL per-second database har en lei tendens til å gå i dvale, og første connection attempt når databasen er i dvale feiler alltid.
        /// Det kan ta opptil 1 minutt å starte databasen. Vi prøver derfor å koble til opptil X ganger, for å være sikker på at databasen er online før vi kjører Migrate.
        /// </summary>
        /// <param name="context"></param>
        private void TestDatabaseConnection()
        {
            var connectionString = Configuration.GetConnectionString(HandhygieneConnection);
            var opts = new DbContextOptionsBuilder<HandhygieneContext>();
            opts.UseNpgsql(connectionString);
            using var context = new HandhygieneContext(opts.Options);
            
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

        private void LeggTilHelsesjekker(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddTypeActivatedCheck<HttpServiceHealthCheck>("HelseIdAuthorityServer", HealthStatus.Unhealthy,
                    new[] { "helseid", "authority" }, this._handhygieneHelseIdConfiguration.Authority);
        }
    }
}
