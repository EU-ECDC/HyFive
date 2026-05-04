using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.ProtectiveEquipment
{
    public class GetProtectiveEquipmentObservation
    {
        public class Query : IRequest<ProtectiveEquipmentObservation>
        {
            public string ObservationId { get; set; }
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
                var isGuid = Guid.TryParse(request.ObservationId, out Guid guidObservationId);

                if (!isGuid)
                {
                    throw new ValidationException("ObservationIdInvalid", request.ObservationId);
                }

                var observation = await _context.ProtectiveEquipmentObservation
                    .Include(b => b.ProtectiveEquipmentList)
                    .ThenInclude(bl => bl.MisuseTypes)
                    .Include(b => b.ProtectiveEquipmentList)
                    .ThenInclude(bl => bl.EquipmentType)
                    .ThenInclude(bu => bu.MisuseTypes)
                    .Include(b => b.ProtectiveEquipmentSession)
                    .ThenInclude(b => b.OrganisationUnit)
                    .ThenInclude(ou => ou.OrganisationUnitRoles)
                    .ThenInclude(our => our.Role)
                    .Include(b => b.SettingType)
                    .ThenInclude(b => b.ProtectiveEquipmentSettingTypeProtectiveEquipmentTypes)
                    .ThenInclude(b => b.ProtectiveEquipmentType)
                    .FirstAsync(b => b.Id == guidObservationId);

                return _mapper.Map<ProtectiveEquipmentObservation>(observation);
            }
        }
    }
}