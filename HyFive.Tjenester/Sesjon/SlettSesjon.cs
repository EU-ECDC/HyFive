using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Sesjon;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Sesjon
{
    public class SlettSesjon
    {
        public class Command : IRequest<SlettSesjonRespons>
        {
            public Guid SesjonId { get; set; }
            public int InstitusjonId { get; set; }
            public string OverforingstatusKode { get; set; }
        }

        public class Handler : IRequestHandler<Command, SlettSesjonRespons>
        {
            private readonly HandhygieneContext _databaseContext;

            public Handler(HandhygieneContext databaseContext)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<SlettSesjonRespons> Handle(Command request, CancellationToken cancellationToken)
            {
                var respons = new SlettSesjonRespons();
                var sesjonOgType = await _databaseContext.Sesjon
                    .AsNoTracking()
                    .Include(s => s.Avdeling).ThenInclude(a => a.Institusjon)
                    .Select(s => new {s.Id, s.Discriminator, OverforingstatusKode = s.Overforingstatus.Kode, InstitusjonId = s.Avdeling.Institusjon.Id})
                    .FirstOrDefaultAsync(s => 
                        s.Id == request.SesjonId
                        && s.OverforingstatusKode == request.OverforingstatusKode
                        && s.InstitusjonId == request.InstitusjonId
                    );

                if (sesjonOgType == null)
                {
                    throw new ArgumentException(
                        $"Kunne ikke finne sesjon med id  {request.SesjonId} og overføringsstatuskode {request.OverforingstatusKode}");
                }

                var sesjonType = SesjonHelper.HentSesjonType(sesjonOgType.Discriminator);

                switch (sesjonType)
                {
                    case SesjonType.FireIndikasjoner:
                        respons.Suksess = SlettSesjonFireIndikasjoner(request.SesjonId);
                        break;
                    case SesjonType.Handsmykker:
                        respons.Suksess = SlettSesjonHandsmykker(request.SesjonId);
                        break;
                    case SesjonType.Hansker:
                        respons.Suksess = SlettSesjonHansker(request.SesjonId);
                        break;
                    case SesjonType.Beskyttelsesutstyr:
                        respons.Suksess = SlettSesjonBeskyttelsesutstyr(request.SesjonId);
                        break;
                    default:
                        throw new ArgumentException(
                            $"Sletting av sesjonstype {sesjonType} er ikke støttet.");
                }

                return respons;
            }

            private bool SlettSesjonFireIndikasjoner(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.FireIndikasjonerSesjon
                    .Include(s => s.Observasjoner)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                _databaseContext.RemoveRange(sesjon.Observasjoner);
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool SlettSesjonHandsmykker(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.HandsmykkeSesjon
                    .Include(s => s.Observasjoner)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                _databaseContext.RemoveRange(sesjon.Observasjoner);
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool SlettSesjonHansker(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.HanskeSesjon
                    .Include(s => s.Observasjoner)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                _databaseContext.RemoveRange(sesjon.Observasjoner);
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool SlettSesjonBeskyttelsesutstyr(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.BeskyttelsesutstyrSesjon
                    .Include(s => s.Observasjoner).ThenInclude(b => b.Beskyttelsesutstyrliste)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                if (sesjon.Observasjoner.Any())
                {
                    var utstyr = sesjon.Observasjoner.SelectMany(o => o.Beskyttelsesutstyrliste);
                    if (utstyr.Any())
                    {
                        _databaseContext.RemoveRange(utstyr);
                    }
                    _databaseContext.RemoveRange(sesjon.Observasjoner);
                }
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }
        }

        public class SlettSesjonRespons
        {
            public bool Suksess { get; set; }
        }
    }
}
