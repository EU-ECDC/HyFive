using System.Threading.Tasks;
using HyFive.Models.V1.Authentication;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HyFive.Admin.Controllers
{
    [Route("Account")]
    [AllowAnonymous]
    public class AccountController : Services.Authentication.Controllers.BaseAccountController
    {
        public AccountController(IUserService userService, IOptions<HandhygieneConfiguration> handHygieneConfig, IOptions<RedirectPagesSettings> redirectPagesConfig) : base(userService, handHygieneConfig, redirectPagesConfig)
        {
        }
    }
}