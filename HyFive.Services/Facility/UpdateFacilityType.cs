using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Facility
{
    public class UpdateFacilityType
    {
        public class Command : IRequest<FacilityType>
        {
            public FacilityType FacilityType { get; set; }
        }

        public class Handler : IRequestHandler<Command, FacilityType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<FacilityType> Handle(Command request, CancellationToken cancellationToken)
            {
                var facilityType = await _context.FacilityType
                    .FirstOrDefaultAsync(i => i.Id == request.FacilityType.Id, cancellationToken);

                facilityType.Name = request.FacilityType.Name;

                _context.FacilityType.Update(facilityType);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedFacilityType =
                    _mapper.Map<Domain.Place.FacilityType, FacilityType>(facilityType);

                return mappedFacilityType;
            }
        }
    }
}