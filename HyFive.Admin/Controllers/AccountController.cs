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

namespace HyFive.Admin.Controllers
{
    [Route("Account")]
    [AllowAnonymous]
    public class AccountController : Services.Authentication.Controllers.BaseAccountController
    {
        public AccountController(IUserService brukerService, IOptions<HandHygieneHealthIdConfiguration> handhygieneKonfigurasjon, IOptions<RedirectPagesKonfigurasjon> redirectKonfigurasjon) : base(brukerService, handhygieneKonfigurasjon, redirectKonfigurasjon)
        {
        }
    }
}