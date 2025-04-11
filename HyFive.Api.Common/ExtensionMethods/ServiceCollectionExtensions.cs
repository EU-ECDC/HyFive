using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HyFive.Modeller.V1.Institution;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSwagger(this IServiceCollection services, string apiTitle, Type apiAssemblyType)
        {
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(apiTitle, new OpenApiInfo
                {
                    Title = apiTitle,
                    Version = "v1"
                });
                setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                var xmlApiCommentFil = $"{Assembly.GetAssembly(apiAssemblyType).GetName().Name}.xml";
                var xmlApiCommentFullPath = Path.Combine(AppContext.BaseDirectory + xmlApiCommentFil);
                setupAction.IncludeXmlComments(xmlApiCommentFullPath);

                var xmlModellerCommentFil = $"{Assembly.GetAssembly(typeof(Institution)).GetName().Name}.xml";
                var xmlModellerCommentFullPath = Path.Combine(AppContext.BaseDirectory + xmlModellerCommentFil);
                setupAction.IncludeXmlComments(xmlModellerCommentFullPath);
            });
        }

    }
}
