using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HyFive.Admin.Controllers
{
    [Route("api/seed")]
    public class SeedController : Controller
    {
        private readonly IMediator _mediator;

        public SeedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Seeds all necessary static code lists required for the application to run:
        /// OverforingstatusTyper (TransferStatusTypes), AktivitetTyper (ActivityTypes), Indications, HandsmykkeTyper (HandJewelryTypes), 
        /// BeskyttelsesutstyrTyper (ProtectiveEquipmentTypes), BeskyttelsesutstyrsettingTyper (ProtectiveEquipmentSettingTypes),
        /// IndicatedGloveTypes, GloveWithoutIndicationType, PostGloveHandHygieneType.
        /// In addition, an FHI Admin is seeded: Felix Mørk.
        /// To seed municipalities, you must run the script "HyFive.DataAccess\Scripts\Kommuner.sql"
        /// </summary>
        /// <returns></returns>
        [HttpGet("codeSystem")]
        public async Task<IActionResult> SeedCodeSystem()
        {
            var ok = await _mediator.Send(new Services.Seed.SeedCodebook.Command());
            return Ok();
        }

        /// <summary>
        /// Seeds a limited set of institutions, InstitutionTypes, departments, DepartmentTypes, Roles, and Users.
        /// If you need more than this, you can run the SQL script "HyFive.DataAccess\Scripts\Institutions, departments, etc.sql".
        /// NOTE: You must use EITHER this API method OR the script.
        /// </summary>
        /// <returns></returns>
        [HttpGet("institutions")]
        public async Task<IActionResult> SeedInstitutions()
        {
            var ok = await _mediator.Send(new Services.Seed.SeedInstitutions.Command());
            return Ok();
        }
    }
}
