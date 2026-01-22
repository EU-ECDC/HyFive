using HyFive.Domain.Exceptions;
using HyFive.Services.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;


namespace HyFive.Api.Common.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IStringLocalizer<Exceptions> _exceptions;
        private readonly IStringLocalizer<Validation> _validation;

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IStringLocalizer<Exceptions> exceptions,
            IStringLocalizer<Validation> validation
            )
        {
            _next = next;
            _logger = logger;
            _exceptions = exceptions;
            _validation = validation;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error: {Code}", ex.Code);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var localized = _validation[ex.Code, ex.Args];
                var response = new { message = localized ?? ex.Code };

                var json = JsonSerializer.Serialize(response, JsonOptions);
                await context.Response.WriteAsync(json);
            }
            catch (DomainException ex)
            {
                // log English message
                _logger.LogWarning(ex, "Domain error occurred: {Code}", ex.Code);

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                // localize with format arguments
                var localized = _exceptions[ex.Code, ex.Args];

                var response = new { message = localized ?? ex.Code };
                var json = JsonSerializer.Serialize(response, JsonOptions);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                //log unexpected exceptions
                _logger.LogError(ex, "Unhandled server error");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new { message = _exceptions["UnexpectedError"] };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
