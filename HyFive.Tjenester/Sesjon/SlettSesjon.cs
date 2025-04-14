using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Session;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Sesjon
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
            private readonly HandHygieneContext _databaseContext;

            public Handler(HandHygieneContext databaseContext)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<SlettSesjonRespons> Handle(Command request, CancellationToken cancellationToken)
            {
                var respons = new SlettSesjonRespons();
                var sesjonOgType = await _databaseContext.Sesjon
                    .AsNoTracking()
                    .Include(s => s.Department).ThenInclude(a => a.Institusjon)
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
                var sesjon = _databaseContext.FourIndicationsSession
                    .Include(s => s.Observations)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                _databaseContext.RemoveRange(sesjon.Observations);
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool SlettSesjonHandsmykker(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.HandJewelrySession
                    .Include(s => s.Observations)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                _databaseContext.RemoveRange(sesjon.Observations);
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool SlettSesjonHansker(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.GloveSession
                    .Include(s => s.Observations)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                _databaseContext.RemoveRange(sesjon.Observations);
                _databaseContext.Remove(sesjon);
                _databaseContext.SaveChanges();
                return true;
            }

            private bool SlettSesjonBeskyttelsesutstyr(Guid requestSesjonId)
            {
                var sesjon = _databaseContext.ProtectiveEquipmentSession
                    .Include(s => s.Observations).ThenInclude(b => b.ProtectiveEquipmentList)
                    .FirstOrDefault(s => s.Id == requestSesjonId);
                if (sesjon.Observations.Any())
                {
                    var utstyr = sesjon.Observations.SelectMany(o => o.ProtectiveEquipmentList);
                    if (utstyr.Any())
                    {
                        _databaseContext.RemoveRange(utstyr);
                    }
                    _databaseContext.RemoveRange(sesjon.Observations);
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
