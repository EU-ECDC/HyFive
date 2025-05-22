using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Controllers;
using HyFive.Services.Authentication.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HyFive.Observation.Controllers
{
    [Route("account")]
    [AllowAnonymous]
    public class AccountController : BaseAccountController
    {
        public AccountController(IUserService userService, IOptions<HandhygieneConfiguration> handHygieneConfig, IOptions<RedirectPagesSettings> redirectPagesSettings) : base(userService, handHygieneConfig, redirectPagesSettings)
        {

        }
    }
}