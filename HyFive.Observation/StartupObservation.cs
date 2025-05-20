using HyFive.Api.Common;
using Microsoft.Extensions.Configuration;
using System;

namespace HyFive.Observation
{
    public class StartupObservation : BaseApiStartup
    {
        protected override string ApiTitle { get; } = "HyFive.Observation.Api";
        protected override Type ApiType { get; } = typeof(StartupObservation);

        public StartupObservation(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
