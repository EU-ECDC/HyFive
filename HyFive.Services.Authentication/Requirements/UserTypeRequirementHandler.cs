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

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserTypeRequirement requirement)
        {
            var email = GetEmail();

            switch (requirement.UserType)
            {
                case UserType.Coordinator:
                    {
                        if (await _userService.IsCoordinator(email))
                            context.Succeed(requirement);
                        break;
                    }

                case UserType.Observer:
                    {
                        if (await _userService.IsObserver(email))
                            context.Succeed(requirement);
                        break;
                    }

                case UserType.Admin:
                    {
                        if (await _userService.IsAdmin(email))
                            context.Succeed(requirement);
                        break;
                    }

                case UserType.AdminOrCoordinator:
                    {
                        if (await _userService.IsAdminOrCoordinator(email))
                            context.Succeed(requirement);
                        break;
                    }
            }
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
