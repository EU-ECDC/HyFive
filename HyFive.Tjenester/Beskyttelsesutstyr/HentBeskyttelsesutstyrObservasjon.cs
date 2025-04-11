using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class HentBeskyttelsesutstyrObservasjon
    {
        public class Query : IRequest<ProtectiveEquipmentObservation>
        {
            public string ObservasjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, ProtectiveEquipmentObservation>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ProtectiveEquipmentObservation> Handle(Query request, CancellationToken cancellationToken)
            {
                var isGuid = Guid.TryParse(request.ObservasjonId, out Guid guidObservasjonId);

                if (!isGuid)
                {
                    throw new ArgumentException($"IBrukerService: Feil ved parsing av ID som GUID: {request.ObservasjonId}.");
                }

                var observasjon = await _context.ProtectiveEquipmentObservation
                    .Include(b => b.ProtectiveEquipmentList)
                    .ThenInclude(bl => bl.MisuseTypes)
                    .Include(b => b.ProtectiveEquipmentList)
                    .ThenInclude(bl => bl.EquipmentType)
                    .ThenInclude(bu => bu.MisuseTypes)
                    .Include(b => b.ProtectiveEquipmentSession)
                    .ThenInclude(b => b.Department)
                    .ThenInclude(b => b.Roller)
                    .Include(b => b.Settingtype)
                    .ThenInclude(b => b.BeskyttelsesutstyrsettingTypeBeskyttelsesutstyrTyper)
                    .ThenInclude(b => b.BeskyttelsesutstyrType)
                    .FirstAsync(b => b.Id == guidObservasjonId);

                return _mapper.Map<ProtectiveEquipmentObservation>(observasjon);
            }
        }
    }
}