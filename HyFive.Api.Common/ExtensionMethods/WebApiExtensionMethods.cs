using System;
using System.Reflection;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.FireIndikasjoner;
using HyFive.Tjenester.Rapporter.FireIndikasjoner;
using HyFive.Tjenester.Rapporter.Handsmykker;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class WebApiExtensionMethods
    {
        public static IServiceCollection LeggTilTjenester(this IServiceCollection tjenester,
            IConfiguration configuration, string apitittel, Type apiType)
        {
            tjenester.LeggTilSwagger(apitittel, apiType);
            tjenester.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(HentMediatRAssemblies()));
            tjenester.AddAutoMapper(HentAutomapperAssemblies());
            tjenester.AddScoped<FireIndikasjonerPdfRapportService>();
            tjenester.AddScoped<HandsmykkePdfRapportService>();

            return tjenester;
        }

        public static Assembly[] HentAutomapperAssemblies()
        {
            return new[]
            {
                Assembly.GetAssembly(typeof(LagreSesjon)), // HyFive.Tjenester
                Assembly.GetAssembly(typeof(FireIndikasjonerSesjon)), // HyFive.Domene
                Assembly.GetAssembly(typeof(Modeller.V1.Sesjon.FireIndikasjonerSesjon)), // HyFive.Modeller
            };
        }

        /// <summary>
        /// Last alle assemblies som trengs for MediatR
        /// </summary>
        /// <returns></returns>
        public static Assembly[] HentMediatRAssemblies()
        {
            return new[]
            {
                Assembly.GetAssembly(typeof(LagreSesjon)),                      // HyFive.Tjenester
                Assembly.GetAssembly(typeof(HentHandsmykkeRapportForAvdeling)) // HyFive.Tjenester.Rapporter
            };
        }
    }
}
