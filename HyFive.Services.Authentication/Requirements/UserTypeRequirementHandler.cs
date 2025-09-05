using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Services.Authentication.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Authentication.Requirements
{
    public class UserTypeRequirementHandler : AuthorizationHandler<UserTypeRequirement>
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HandHygieneContext _context;


        public UserTypeRequirementHandler(IUserService userService, IHttpContextAccessor httpContextAccessor, HandHygieneContext handHygieneContext)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _context = handHygieneContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            var userType = requirement.UserType;
            //var hprNumber = context.User.Claims.FirstOrDefault(x => x.Type == ClaimsPrincipalExtensions.HprNummer)?.Value;
            //var pseudonym = context.User.Claims.FirstOrDefault(x => x.Type == IdentityClaims.PidPseudonym)?.Value;

            var email = GetEmail();


            if (userType == UserType.Coordinator)
            {
                var isCoordinator = _userService.IsCoordinator(email);
                if (isCoordinator)
                    context.Succeed(requirement);
            }

            if (userType == UserType.Observer)
            {
                var erObservator = _userService.IsObserver(email);
                if (erObservator)
                    context.Succeed(requirement);
            }

            if (userType == UserType.FhiAdmin)
            {
                var erFhiAdmin = _userService.IsFhiAdmin(email);
                if(erFhiAdmin)
                    context.Succeed(requirement);
            }
            if (userType == UserType.FhiAdminOrCoordinator)
            {
                var IsFhiAdminOrCoordinator = _userService.IsFhiAdminOrCoordinator(email);
                if(IsFhiAdminOrCoordinator)
                    context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        public string GetHprNumber()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            return user?.HPRNumber;
        }

        public string GetPseudonym()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            return user?.IdentityPseudonym;
        }

        public string GetEmail()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            return email;
        }


    }
}
