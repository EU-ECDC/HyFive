using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class UpdateFacility
    {
        public class Command : IRequest<Models.V1.Facility.Facility>
        {
            public Models.V1.Facility.Facility Facility { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Facility.Facility>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Facility.Facility> Handle(Command command, CancellationToken cancellationToken)
            {
                // Check facility type:
                var facilityType = await _context.FacilityType.FirstOrDefaultAsync(i => i.Id == command.Facility.FacilityType.Id);
                if (facilityType == null)
                    throw new ArgumentException(
                        $"Facility type with ID {command.Facility.FacilityType.Id} was not found in the database.");

                //check city:
                var city = await _context.City.FirstOrDefaultAsync(i => i.Id == command.Facility.City.Id);
                if (city == null)
                    throw new ArgumentException(
                        $"City with ID {command.Facility.City.Id} was not found in the database.");

                var facility = await _context.Facility
                                                .Include(i => i.City)
                                                .FirstOrDefaultAsync(i => i.Id == command.Facility.Id);
                
                facility.Name = command.Facility.Name;
                facility.Abbreviation = command.Facility.Abbreviation;
                facility.HERId = command.Facility.HERId;
                facility.FacilityType = facilityType;
                facility.City = city;

                _context.Facility.Update(facility);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Facility.Facility>(facility);
                return mapped;
            }
        }
    }
}
