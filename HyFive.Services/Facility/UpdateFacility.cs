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

                var facility = await _context.Facility
                                                .Include(i => i.Municipality)
                                                .Include(i => i.HealthcareOrganization)
                                                .FirstOrDefaultAsync(i => i.Id == command.Facility.Id);

                //if (command.Facility.Municipality != null && command.Facility.FacilityType.Code == FacilityTypeConstants.NursingHome)
                //{
                //    var municipality = await _context.Municipality.FirstOrDefaultAsync(k => k.Id == command.Facility.Municipality.Id);
                //    facility.Municipality = municipality;
                //}
                //else
                //{
                //    facility.Municipality = null;
                //}

                //if (command.Facility.HealthcareOrganization != null && command.Facility.FacilityType.Code == FacilityTypeConstants.Hospital)
                //{
                //    var healthcareOrganization = await _context.HealthcareOrganization.FirstOrDefaultAsync(h => h.Id == command.Facility.HealthcareOrganization.Id);
                //    facility.HealthcareOrganization = healthcareOrganization;
                //}
                //else
                //{
                //    facility.HealthcareOrganization = null;
                //}
                
                facility.Name = command.Facility.Name;
                facility.Abbreviation = command.Facility.Abbreviation;
                facility.HERId = command.Facility.HERId;
                facility.FacilityType = facilityType;

                _context.Facility.Update(facility);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Facility.Facility>(facility);
                return mapped;
            }
        }
    }
}
