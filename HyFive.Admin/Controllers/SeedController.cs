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
        /// Seeder alle nædvendige statiske kodeverk som skal tl for å få applikasjonen til å kjøre:
        /// OverforingstatusTyper, AktivitetTyper, Indications, HandsmykkeTyper, BeskyttelsesutstyrTyper, BeskyttelsesutstyrsettingTyper,
        /// IndicatedGloveTypes, GeneralPurposeGloveType, PostGloveHandHygiene.
        /// I tillegg seedes det en FHI Admin: Felix Mørk.
        /// For å seede kommuner må du kjøre scriptet "HyFive.DataAccess\Scripts\Kommuner.sql"
        /// </summary>
        /// <returns></returns>
        [HttpGet("kodeverk")]
        public async Task<IActionResult> SeedKodeverk()
        {
            var ok = await _mediator.Send(new Services.Seed.SeedCodebook.Command());
            return Ok();
        }

        /// <summary>
        /// Seeder et begrenset sett med institusjoner, InstitusjonTyper, avdelinger, AvdelingTyper, Roles og Users.
        /// Hvis du trenger mer enn dette kan du kjøre sql-scriptet "HyFive.DataAccess\Scripts\Institutions, avdelinger mm.sql".
        /// NB! Du må bruke ENTEN denne api-metoden ELLER scriptet
        /// </summary>
        /// <returns></returns>
        [HttpGet("institusjoner")]
        public async Task<IActionResult> SeedInstitusjoner()
        {
            var ok = await _mediator.Send(new Services.Seed.SeedInstitutions.Command());
            return Ok();
        }
    }
}
