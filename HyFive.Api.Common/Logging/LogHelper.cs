using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace HyFive.Api.Common.Logging
{
    /// <summary>
    /// Copied from https://andrewlock.net/using-serilog-aspnetcore-in-asp-net-core-3-logging-the-selected-endpoint-name-with-serilog/
    /// </summary>
    public static class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            diagnosticContext.Set("RequestPathAnonymized", AnonymizePersonalIdentifiers(request.Path.Value));
            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", AnonymizePersonalIdentifiers(request.QueryString.Value));
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        /// <summary>
        /// Search a string for personal identifiers, return a scrambled version of the string.
        /// </summary>
        /// <param name="sourceToAnonymize">A string possibly containing 1/more personal identifiers </param>
        /// <returns>A scrambled version of the string.</returns>

        internal static string AnonymizePersonalIdentifiers(string sourceToAnonymize)
        {
            if (string.IsNullOrWhiteSpace(sourceToAnonymize))
                return sourceToAnonymize;

            return Regex.Replace(sourceToAnonymize, "\\d{6,11}", "***********");
        }
    }
}
