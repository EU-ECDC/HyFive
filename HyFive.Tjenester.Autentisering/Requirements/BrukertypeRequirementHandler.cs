using System.Linq;
using System.Threading.Tasks;
using HyFive.Tjenester.Autentisering.Bruker;
using Fhi.HelseId.Common.Identity;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Tjenester.Autentisering.Requirements
{
    public class BrukertypeRequirementHandler : AuthorizationHandler<BrukertypeRequirement>
    {
        private readonly IBrukerService _brukerservice;

        public BrukertypeRequirementHandler(IBrukerService brukerservice)
        {
            _brukerservice = brukerservice;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BrukertypeRequirement requirement)
        {
            var brukertype = requirement.Brukertype;
            //var hprnummer = context.User.Claims.FirstOrDefault(x => x.Type == ClaimsPrincipalExtensions.HprNummer)?.Value;
            //var pseudonym = context.User.Claims.FirstOrDefault(x => x.Type == IdentityClaims.PidPseudonym)?.Value;

            var hprnummer = "";
            var pseudonym = "OCW6BpVN57vnbxBUE8WOOTM9FrkCaBixlD2y8FgYCag=";
            
            if (brukertype == Brukertype.Koordinator)
            {
                var erKoordinator = _brukerservice.ErKoordinator(hprnummer, pseudonym);
                if (erKoordinator)
                    context.Succeed(requirement);
            }

            if (brukertype == Brukertype.Observator)
            {
                var erObservator = _brukerservice.ErObservator(hprnummer, pseudonym);
                if (erObservator)
                    context.Succeed(requirement);
            }

            if (brukertype == Brukertype.FhiAdmin)
            {
                var erFhiAdmin = _brukerservice.ErFhiAdmin(pseudonym, hprnummer);
                if(erFhiAdmin)
                    context.Succeed(requirement);
            }
            if (brukertype == Brukertype.FhiAdminEllerKoordinator)
            {
                var erFhiAdminEllerKoordinator = _brukerservice.ErFhiAdminEllerKoordinator(pseudonym, hprnummer);
                if(erFhiAdminEllerKoordinator)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

     
    }
}
