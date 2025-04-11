using Microsoft.AspNetCore.Builder;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseSwagger(this IApplicationBuilder app, string apiTitle)
        {
            app.UseSwagger(setupAction =>
            {
                setupAction.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    $"{apiTitle}/swagger.json",
                    apiTitle);
                setupAction.RoutePrefix = "swagger";
            });
        }
    }
}
