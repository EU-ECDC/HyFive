using System.Threading.Tasks;
using HyFive.Modeller.V1.Autentisering;
using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Konfigurasjon;
using Fhi.HelseId.Common.Identity;
using Fhi.HelseId.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HyFive.Tjenester.Autentisering.Controllers
{
    [AllowAnonymous]
    public abstract class BaseAccountController : ControllerBase
    {
        private readonly IBrukerService _brukerService;
        private RedirectPagesKonfigurasjon _redirectkonfigurasjon{ get; }
        private HandhygieneHelseIdKonfigurasjon _helseidkonfigurasjon { get; }

        protected BaseAccountController(IBrukerService brukerService, 
            IOptions<HandhygieneHelseIdKonfigurasjon> handhygieneKonfigurasjon, 
            IOptions<RedirectPagesKonfigurasjon> redirectKonfigurasjon)
        {
            _brukerService = brukerService;
            _redirectkonfigurasjon = redirectKonfigurasjon.Value;
            _helseidkonfigurasjon = handhygieneKonfigurasjon.Value;
        }

        /// <summary>
        /// Hent innlogget bruker fra API
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<InnloggetBruker>> Get()
        {
            if (_brukerService.ErBrukerLoggetInn())
            {
                return Ok(await _brukerService.HentBruker());
            }

            return Unauthorized();

        }

        /// <summary>
        /// Hent innlogget bruker fra API
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("IsLoggedIn")]
        public ActionResult<bool> ErBrukerLoggetInn()
        {
            return Ok(_brukerService.ErBrukerLoggetInn());
        }


        [HttpGet("Login")]
        public async Task Login()
        {
            if (_helseidkonfigurasjon.AuthUse)
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
            if (_helseidkonfigurasjon.AuthUse)
            {
                await HttpContext.SignOutAsync(HelseIdContext.Scheme, new AuthenticationProperties
                {
                    RedirectUri = _redirectkonfigurasjon.LoggedOut,
                });
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}