using Microsoft.AspNetCore.Builder;

namespace HyFive.Api.Common.ExtensionMethods
{
    public static class ApplicationBuilderExtensions
    {
        public static void BrukSwagger(this IApplicationBuilder app, string apiTittel)
        {
            app.UseSwagger(setupAction =>
            {
                setupAction.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    $"{apiTittel}/swagger.json",
                    apiTittel);
                setupAction.RoutePrefix = "swagger";
            });
        }
    }
}
