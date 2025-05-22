using System.Linq;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using Microsoft.AspNetCore.Authorization;

namespace HyFive.Services.Authentication.Requirements
{
    public class UserTypeRequirementHandler : AuthorizationHandler<UserTypeRequirement>
    {
        private readonly IUserService _userService;

        public UserTypeRequirementHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            var userType = requirement.UserType;
            //var hprNumber = context.User.Claims.FirstOrDefault(x => x.Type == ClaimsPrincipalExtensions.HprNummer)?.Value;
            //var pseudonym = context.User.Claims.FirstOrDefault(x => x.Type == IdentityClaims.PidPseudonym)?.Value;

            var hprnummer = "";
            var pseudonym = "OCW6BpVN57vnbxBUE8WOOTM9FrkCaBixlD2y8FgYCag=";
            
            if (userType == UserType.Coordinator)
            {
                var isCoordinator = _userService.IsCoordinator(hprnummer, pseudonym);
                if (isCoordinator)
                    context.Succeed(requirement);
            }

            if (userType == UserType.Observer)
            {
                var erObservator = _userService.IsObserver(hprnummer, pseudonym);
                if (erObservator)
                    context.Succeed(requirement);
            }

            if (userType == UserType.FhiAdmin)
            {
                var erFhiAdmin = _userService.IsFhiAdmin(pseudonym, hprnummer);
                if(erFhiAdmin)
                    context.Succeed(requirement);
            }
            if (userType == UserType.FhiAdminOrCoordinator)
            {
                var IsFhiAdminOrCoordinator = _userService.IsFhiAdminOrCoordinator(pseudonym, hprnummer);
                if(IsFhiAdminOrCoordinator)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

     
    }
}
