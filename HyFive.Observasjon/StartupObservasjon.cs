using HyFive.Api.Common;
using Microsoft.Extensions.Configuration;
using System;

namespace HyFive.Observasjon
{
    public class StartupObservasjon : BaseApiStartup
    {
        protected override string ApiTittel { get; } = "HyFive.Observasjon.Api";
        protected override Type ApiType { get; } = typeof(StartupObservasjon);

        public StartupObservasjon(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
