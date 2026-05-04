using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.OrganisationUnit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class UpdateFacilityType
    {
        public class Command : IRequest<OrganisationUnitType>
        {
            public OrganisationUnitType FacilityType { get; set; }
        }

        public class Handler : IRequestHandler<Command, OrganisationUnitType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<OrganisationUnitType> Handle(Command request, CancellationToken cancellationToken)
            {
                var facilityType = await _context.OrganisationUnitType
                .FirstOrDefaultAsync(i => i.Id == request.FacilityType.Id, cancellationToken);

                facilityType.Name = request.FacilityType.Name;

                await _context.SaveChangesAsync(cancellationToken);

                var mappedFacilityType =
                    _mapper.Map<Domain.Place.OrganisationUnitType, OrganisationUnitType>(facilityType);

                return mappedFacilityType;
            }
        }
    }
}