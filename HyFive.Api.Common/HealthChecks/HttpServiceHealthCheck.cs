using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HyFive.Api.Common.HealthChecks
{
    public class HttpServiceHealthCheck: IHealthCheck
    {
        public HttpServiceHealthCheck(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var helseIdResult = await new HttpClient().SendAsync( new HttpRequestMessage(){ RequestUri = new Uri(Url) } );
            var healthstatus = helseIdResult.IsSuccessStatusCode ? HealthStatus.Healthy : HealthStatus.Unhealthy;
            var result = new HealthCheckResult(healthstatus);
            return result;
        }
    }
}