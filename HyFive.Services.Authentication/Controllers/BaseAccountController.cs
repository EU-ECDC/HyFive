using System.Threading.Tasks;
using HyFive.Models.V1.Authentication;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace HyFive.Services.Authentication.Controllers
{
    [AllowAnonymous]
    public abstract class BaseAccountController : ControllerBase
    {
        private readonly IUserService userService;
        private RedirectPagesSettings redirectConfiguration { get; }
        private HandhygieneConfiguration handHygieneConfiguration { get; }

        protected BaseAccountController(IUserService _userService, 
            IOptions<HandhygieneConfiguration> _handHygieneConfiguration, 
            IOptions<RedirectPagesSettings> _redirectConfiguration)
        {
            userService = _userService;
            redirectConfiguration = _redirectConfiguration.Value;
            handHygieneConfiguration = _handHygieneConfiguration.Value;
        }

        /// <summary>
        /// Fetch logged-in user from API
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<LoggedInUser>> Get()
        {
            if (userService.IsUserLoggedIn())
            {
                return Ok(await userService.GetUser());
            }

            return Unauthorized();

        }

        /// <summary>
        /// Fetch logged-in user from API
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("IsLoggedIn")]
        public ActionResult<bool> IsUserLoggedIn()
        {
            return Ok(userService.IsUserLoggedIn());
        }


        [HttpGet("Login")]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = redirectConfiguration.LoggedIn
            }, 
            OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("Logout")]
        public IActionResult Logout()

        {

            return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = redirectConfiguration.LoggedOut,
            },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
            
        }
    }
}