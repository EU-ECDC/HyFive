using System;
using System.Reflection;
using HyFive.Models.V1.Session;
using HyFive.Services.FiveIndication;
using HyFive.Services.Reports.FiveIndicators;
using HyFive.Services.Reports.HandJewelry;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class WebApiExtensionMethods
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            IConfiguration configuration, string apiTitle, Type apiType)
        {
            services.AddSwagger(apiTitle, apiType);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(GetMediatRAssemblies()));
            services.AddAutoMapper(GetAutomapperAssemblies());
            services.AddScoped<FiveIndicationsPDFReportService>();
            services.AddScoped<HandJewelryPdfReportService>();

            return services;
        }

        public static Assembly[] GetAutomapperAssemblies()
        {
            return new[]
            {
                Assembly.GetAssembly(typeof(SaveSession)), // HyFive.Services
                Assembly.GetAssembly(typeof(FiveIndicationsSession)), // HyFive.Domain
                Assembly.GetAssembly(typeof(Models.V1.Session.FiveIndicationsSession)), // HyFive.Models
            };
        }

        /// <summary>
        /// Load all assemblies needed for MediatR
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetMediatRAssemblies()
        {
            return new[]
            {
                Assembly.GetAssembly(typeof(SaveSession)),                      // HyFive.Services
                Assembly.GetAssembly(typeof(GetHandJewelryReportForDepartment)) // HyFive.Services.Reports
            };
        }
    }
}