using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Overview;
using HyFive.Models.V1.Session;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Sesjon
{
    public class HentSesjonerForInstitusjon
    {
        public class Query : IRequest<List<SessionOverviewReport>>
        {
            public int InstitusjonId { get; set; }
            public int? ObservatorId { get; set; }
            public SesjonType? Sesjontype { get; set; }
            public DateTime? Fra { get; set; }
            public DateTime? Til { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<SessionOverviewReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<SessionOverviewReport>> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjonOversiktRapport = new List<SessionOverviewReport>();
                
                if (request.Sesjontype == null || request.Sesjontype.Value == SesjonType.FireIndikasjoner)
                {
                    var fireIndikasjonerSesjonerRapport= await LagFireIndikasjonerSesjonerRapport(request, cancellationToken);
                    sesjonOversiktRapport.AddRange(fireIndikasjonerSesjonerRapport); 
                }
                if (request.Sesjontype == null || request.Sesjontype.Value == SesjonType.Handsmykker)
                {
                    var handsmykkeSesjonerRapport = await LagHandsmykkeSesjonerRapport(request, cancellationToken);
                    sesjonOversiktRapport.AddRange(handsmykkeSesjonerRapport);
                }
                if (request.Sesjontype == null || request.Sesjontype.Value == SesjonType.Hansker)
                {
                    var hanskeSesjonerRapport = await LagHanskeSesjonerRapport(request, cancellationToken);
                    sesjonOversiktRapport.AddRange(hanskeSesjonerRapport);
                }
                if (request.Sesjontype == null || request.Sesjontype.Value == SesjonType.Beskyttelsesutstyr)
                {
                    var beskyttelsesutstyrSesjonerRapport = await LagBeskyttelsesutstyrSesjonerRapport(request, cancellationToken);
                    sesjonOversiktRapport.AddRange(beskyttelsesutstyrSesjonerRapport);
                }

                sesjonOversiktRapport = sesjonOversiktRapport.OrderByDescending(s => s.CreatedTime).ToList();
                sesjonOversiktRapport.ForEach(s =>
                {
                    s.Observations = s.Observations.OrderByDescending(o => o.RegisteredTime).ToList();
                });

                return sesjonOversiktRapport;
            }

            private async Task<List<SessionOverviewReport>> LagFireIndikasjonerSesjonerRapport(Query request, CancellationToken cancellationToken)
            {
                var fireIndikasjonerSesjoner = await _context.FourIndicationsSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.IndicationTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.Activity.ActivityType)
                                     .Where(s => s.Avdeling.InstitusjonId == request.InstitusjonId)
                                     .Where(s => request.ObservatorId == null || s.Observator.Id == request.ObservatorId)
                                     .Where(s => request.Fra == null || s.Opprettettidspunkt.Date >= request.Fra.Value.Date)
                                     .Where(s => request.Til == null || s.Opprettettidspunkt.Date <= request.Til.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var fireIndikasjonerSesjonerRapport = _mapper.Map<List<Domene.Session.FourIndicationsSession>, List<SessionOverviewReport>>(fireIndikasjonerSesjoner);

                return fireIndikasjonerSesjonerRapport;
            }

            private async Task<List<SessionOverviewReport>> LagHandsmykkeSesjonerRapport(Query request, CancellationToken cancellationToken)
            {
                var handsmykkeSesjoner = await _context.HandJewelrySession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.HandJewelry)
                                     .Where(s => s.Avdeling.InstitusjonId == request.InstitusjonId)
                                     .Where(s => request.ObservatorId == null || s.Observator.Id == request.ObservatorId)
                                     .Where(s => request.Fra == null || s.Opprettettidspunkt.Date >= request.Fra.Value.Date)
                                     .Where(s => request.Til == null || s.Opprettettidspunkt.Date <= request.Til.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var handsmykkeSesjonerRapport = _mapper.Map<List<Domene.Session.HandJewelrySession>, List<SessionOverviewReport>>(handsmykkeSesjoner);

                return handsmykkeSesjonerRapport;
            }

            private async Task<List<SessionOverviewReport>> LagHanskeSesjonerRapport(Query request, CancellationToken cancellationToken)
            {
                var hanskeSesjoner = await _context.GloveSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.IndicatedGloveTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.GeneralPurposeGloveTypes)
                                     .Include(s => s.Observations).ThenInclude(o => o.HandhygieneEtterHanskebrukType)
                                     .Where(s => s.Avdeling.InstitusjonId == request.InstitusjonId)
                                     .Where(s => request.ObservatorId == null || s.Observator.Id == request.ObservatorId)
                                     .Where(s => request.Fra == null || s.Opprettettidspunkt.Date >= request.Fra.Value.Date)
                                     .Where(s => request.Til == null || s.Opprettettidspunkt.Date <= request.Til.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var hanskeSesjonerRapport = _mapper.Map<List<Domene.Session.GloveSession>, List<SessionOverviewReport>>(hanskeSesjoner);

                return hanskeSesjonerRapport;
            }

            private async Task<List<SessionOverviewReport>> LagBeskyttelsesutstyrSesjonerRapport(Query request, CancellationToken cancellationToken)
            {
                var beskyttelsesutstyrSesjoner = await _context.ProtectiveEquipmentSession
                                     .Include(s => s.Department)
                                     .Include(s => s.Observer)
                                     .Include(s => s.TransmissionStatus)
                                     .Include(s => s.Observations).ThenInclude(o => o.Role)
                                     .Include(s => s.Observations).ThenInclude(o => o.SettingType)
                                     .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.EquipmentType)
                                     .Include(s => s.Observations).ThenInclude(o => o.ProtectiveEquipmentList).ThenInclude(b => b.MisuseTypes)
                                     .Where(s => s.Avdeling.InstitusjonId == request.InstitusjonId)
                                     .Where(s => request.ObservatorId == null || s.Observator.Id == request.ObservatorId)
                                     .Where(s => request.Fra == null || s.Opprettettidspunkt.Date >= request.Fra.Value.Date)
                                     .Where(s => request.Til == null || s.Opprettettidspunkt.Date <= request.Til.Value.Date)
                                     .AsNoTracking()
                                     .ToListAsync(cancellationToken);
                var beskyttelsesutstyrSesjonerRapport = _mapper.Map<List<Domene.Session.ProtectiveEquipmentSession>, List<SessionOverviewReport>>(beskyttelsesutstyrSesjoner);

                return beskyttelsesutstyrSesjonerRapport;
            }
        }
    }
}