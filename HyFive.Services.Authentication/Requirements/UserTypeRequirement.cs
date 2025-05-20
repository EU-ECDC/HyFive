using Microsoft.AspNetCore.Authorization;

namespace HyFive.Services.Authentication.Requirements
{
    public class UserTypeRequirement : IAuthorizationRequirement
    {
        public UserType UserType { get; set; }
        public UserTypeRequirement(UserType userType)
        {
            UserType = userType;
        }
    }
}
