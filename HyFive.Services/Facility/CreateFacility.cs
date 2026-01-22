using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domain.Exceptions;

namespace HyFive.Services.Facility
{
    public class CreateFacility
    {
        public class Command : IRequest<Models.V1.Facility.Facility>
        {
            public CreateFacilityRequest Request { get; set; }
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
                // check facility type
                var facilityType = await _context.FacilityType.FirstOrDefaultAsync(i => i.Id  == command.Request.FacilityTypeId);
                if (facilityType == null)
                    throw new DomainException("FacilityTypeNotFound", command.Request.FacilityTypeId);
                
                var city = await _context.City.FirstOrDefaultAsync(h => h.Id == command.Request.CityId);
                if (city == null)
                    throw new DomainException("CityNotFound", command.Request.CityId);
                var coordinator = new Coordinator()
                {
                    FirstName = command.Request.CoordinatorFirstName,
                    LastName = command.Request.CoordinatorLastName,
                    HPRNumber = command.Request.CoordinatorHPRNumber,
                    IdentityPseudonym =  command.Request.CoordinatorPseudonym,
                    Email = command.Request.CoordinatorEmail
                };

                var observer = new Observer()
                {
                    FirstName = command.Request.CoordinatorFirstName,
                    LastName = command.Request.CoordinatorLastName,
                    HPRNumber = command.Request.CoordinatorHPRNumber,
                    IdentityPseudonym = command.Request.CoordinatorPseudonym,
                    Email = command.Request.CoordinatorEmail
                };
                
                var facility = new Domain.Place.Facility();
                facility.Name = command.Request.FacilityName;
                facility.Abbreviation = command.Request.Abbreviation;
                facility.HERId = command.Request.HERId;
                facility.FacilityType = facilityType;
                facility.City = city;

                facility.Users.Add(coordinator);
                facility.Users.Add(observer);
                _context.Facility.Add(facility);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Facility.Facility>(facility);
                return mapped;
            }
        }
    }
}
