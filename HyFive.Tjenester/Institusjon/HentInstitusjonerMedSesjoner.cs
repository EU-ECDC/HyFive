using System.Linq;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Overview;
using System.Collections.Generic;
using HyFive.Models.V1.Session;
using System;
using HyFive.Models.V1.Constants;

namespace HyFive.Services.Institusjon
{
    public class HentInstitusjonerMedSesjoner
    {
        public class Query : IRequest<List<InstitutionOverviewReport>>
        {
            public SessionType? Sesjontype { get; set; }
            public DateTime? FraDato { get; set; }
            public DateTime? TilDato { get; set; }
            public int? InstitusjonId { get; set; }
            public string OverforingsstatusType { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<InstitutionOverviewReport>>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<InstitutionOverviewReport>> Handle(Query query, CancellationToken cancellationToken)
            {
                var sesjonType = HentSesjonType(query.Sesjontype);
                var institusjoner = await _context.Institution
                    .AsNoTracking()
                    .Include(i => i.Departments)
                    .ThenInclude(a => a.Sesjoner
                        .Where(s => query.Sesjontype == null || s.Discriminator == sesjonType)
                        .Where(s => query.FraDato == null || s.Opprettettidspunkt.Date >= query.FraDato.Value.Date)
                        .Where(s => query.TilDato == null || s.Opprettettidspunkt.Date <= query.TilDato.Value.Date)
                        .Where(s => TransferStatusTypeConstants.GetTransferStatusTypes(query.OverforingsstatusType).Contains(s.Overforingstatus.Kode))
                    )
                    .Where(x => query.InstitusjonId == null || query.InstitusjonId == x.Id)
                    .OrderBy(i => i.Navn)
                    .ToListAsync(cancellationToken);

                var rapporter  = _mapper.Map<List<Domene.Place.Institution>, List<InstitutionOverviewReport>>(institusjoner);

                foreach (var rapport in rapporter)
                {
                    foreach (var avdeling in rapport.Departments)
                    {
                        var antall = HentAntallObservasjoner(avdeling.Id, query);
                        avdeling.NumberOfObservations = antall;
                        rapport.NumberOfObservations += antall;
                    }

                    var sortertListeMedObservasjoner = rapport.Departments.Where(a => a.NumberOfSessions > 0).OrderBy(a => a.Name);
                    var sortertListeUtenObservasjoner = rapport.Departments.Where(a => a.NumberOfSessions == 0).OrderBy(a => a.Name);

                    rapport.Departments = sortertListeMedObservasjoner.Union(sortertListeUtenObservasjoner).ToList(); 
                }

                return rapporter.Where(r=>r.NumberOfObservations > 0).ToList();
            }

            private int HentAntallObservasjoner(int avdelingId, Query query)
            {
                switch (query.Sesjontype)
                {
                    case SessionType.ProtectiveEquipment:
                        return HentAntallObservasjonerForBeskyttelsesutstyr(avdelingId, query);
                    case SessionType.FourIndications:
                        return HentAntallObservasjonerForFireIndikasjoner(avdelingId, query);
                    case SessionType.Gloves:
                        return HentAntallObservasjonerForHansker(avdelingId, query);
                    case SessionType.HandJewelry:
                        return HentAntallObservasjonerForHandsmykker(avdelingId, query);
                    default:
                        return HentAggregertAntallObservasjoner(avdelingId, query);
                }
            }

            private int HentAggregertAntallObservasjoner(in int avdelingId, Query query)
            {
                return 
                    HentAntallObservasjonerForBeskyttelsesutstyr(avdelingId, query)
                    + HentAntallObservasjonerForFireIndikasjoner(avdelingId, query)
                    + HentAntallObservasjonerForHansker(avdelingId, query)
                    + HentAntallObservasjonerForHandsmykker(avdelingId, query);
            }

            private int HentAntallObservasjonerForHandsmykker(int avdelingId, Query query)
            {
                return _context.HandJewelryObservation
                    .AsNoTracking()
                    .Include(b => b.HandJewelrySession)
                    .Count(bo => bo.HandJewelrySession.Department.Id == avdelingId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.OverforingsstatusType).Contains(bo.HandJewelrySession.TransmissionStatus.Code)
                                 && (query.FraDato == null || bo.RegistrationTime.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.RegistrationTime.Date <= query.TilDato.Value.Date));
            }

            private int HentAntallObservasjonerForHansker(int avdelingId, Query query)
            {
                return _context.GloveObservation
                    .AsNoTracking()
                    .Include(b => b.GloveSession)
                    .Count(bo => bo.GloveSession.Department.Id == avdelingId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.OverforingsstatusType).Contains(bo.GloveSession.TransmissionStatus.Code)
                                 && (query.FraDato == null || bo.RegistrationTime.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.RegistrationTime.Date <= query.TilDato.Value.Date));
            }

            private int HentAntallObservasjonerForFireIndikasjoner(int avdelingId, Query query)
            {
                return _context.FourIndicationsObservation
                    .AsNoTracking()
                    .Include(b => b.FourIndicationsSession)
                    .Count(bo => bo.FourIndicationsSession.Department.Id == avdelingId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.OverforingsstatusType).Contains(bo.FourIndicationsSession.TransmissionStatus.Code)
                                 && (query.FraDato == null || bo.RegistrationTime.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.RegistrationTime.Date <= query.TilDato.Value.Date));
            }

            private int HentAntallObservasjonerForBeskyttelsesutstyr(int avdelingId, Query query)
            {
                return _context.ProtectiveEquipmentObservation
                    .AsNoTracking()
                    .Include(b => b.ProtectiveEquipmentSession)
                    .Count(bo => bo.ProtectiveEquipmentSession.Department.Id == avdelingId 
                                 && TransferStatusTypeConstants.GetTransferStatusTypes(query.OverforingsstatusType).Contains(bo.ProtectiveEquipmentSession.TransmissionStatus.Code)
                                 && (query.FraDato == null || bo.RegistrationTime.Date >= query.FraDato.Value.Date)
                                 && (query.TilDato == null || bo.RegistrationTime.Date <= query.TilDato.Value.Date));
            }

            private static string HentSesjonType(SessionType? type)
            {
                switch (type)
                {
                    case SessionType.FourIndications:
                        return nameof(Domene.Session.FourIndicationsSession);
                    case SessionType.HandJewelry:
                        return nameof(Domene.Session.HandJewelrySession);
                    case SessionType.ProtectiveEquipment:
                        return nameof(Domene.Session.ProtectiveEquipmentSession);
                    case SessionType.Gloves:
                        return nameof(Domene.Session.GloveSession);
                    default:
                        return "";
                }
            }
        }
    }
}
