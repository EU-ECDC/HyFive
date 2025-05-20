using System.Threading.Tasks;
using HyFive.Models.V1.Authentication;
using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Configuration;
using Fhi.HelseId.Common.Identity;
using Fhi.HelseId.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HyFive.Services.Authentication.Controllers
{
    [AllowAnonymous]
    public abstract class BaseAccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private RedirectPagesKonfigurasjon _redirectConfiguration{ get; }
        private HandhygieneHelseIdKonfigurasjon healthIdConfiguration { get; }

        protected BaseAccountController(IUserService userService, 
            IOptions<HandhygieneHelseIdKonfigurasjon> HandHygieneConfiguration, 
            IOptions<RedirectPagesKonfigurasjon> redirectConfiguration)
        {
            _userService = userService;
            _redirectConfiguration = redirectConfiguration.Value;
            healthIdConfiguration = HandHygieneConfiguration.Value;
        }

        /// <summary>
        /// Fetch logged-in user from API
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<LoggedInUser>> Get()
        {
            if (_userService.IsUserLoggedIn())
            {
                return Ok(await _userService.GetUser());
            }

            return Unauthorized();

        }

        /// <summary>
        /// Fetch logged-in user from API
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("IsLoggedIn")]
        public ActionResult<bool> IsUserLoeggedIn()
        {
            return Ok(_userService.IsUserLoggedIn());
        }


        [HttpGet("Login")]
        public async Task Login()
        {
            if (healthIdConfiguration.AuthUse)
            {
                await HttpContext.ChallengeAsync(
                    HelseIdContext.Scheme,
                    new AuthenticationProperties
                    {
                        RedirectUri = "/"
                    });
            }
            else
            {
                HttpContext.Response.Redirect("/index.html");
            }
        }

        [HttpGet("Logout")]
        public async Task Logout()
        {
            if (healthIdConfiguration.AuthUse)
            {
                await HttpContext.SignOutAsync(HelseIdContext.Scheme, new AuthenticationProperties
                {
                    RedirectUri = _redirectConfiguration.LoggedOut,
                });
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}