using HyFive.Services.Authentication.User;
using HyFive.Services.Authentication.Controllers;
using HyFive.Services.Authentication.Configuration;
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
        public AccountController(IUserService brukerService, IOptions<HandHygieneHealthIdConfiguration> handhygieneKonfigurasjon, IOptions<RedirectPagesKonfigurasjon> redirectKonfigurasjon) : base(brukerService, handhygieneKonfigurasjon, redirectKonfigurasjon)
        {

        }
    }
}