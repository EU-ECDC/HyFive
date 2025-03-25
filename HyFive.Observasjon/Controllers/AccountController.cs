using HyFive.Tjenester.Autentisering.Bruker;
using HyFive.Tjenester.Autentisering.Controllers;
using HyFive.Tjenester.Autentisering.Konfigurasjon;
using Fhi.HelseId.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HyFive.Observasjon.Controllers
{
    [Route("account")]
    [AllowAnonymous]
    public class AccountController : BaseAccountController
    {
        public AccountController(IBrukerService brukerService, IOptions<HandhygieneHelseIdKonfigurasjon> handhygieneKonfigurasjon, IOptions<RedirectPagesKonfigurasjon> redirectKonfigurasjon) : base(brukerService, handhygieneKonfigurasjon, redirectKonfigurasjon)
        {

        }
    }
}