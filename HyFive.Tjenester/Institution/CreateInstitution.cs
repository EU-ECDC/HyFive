using System;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domain.User;
using HyFive.Models.V1.Institution;

namespace HyFive.Services.Institution
{
    public class CreateInstitution
    {
        public class Command : IRequest<Models.V1.Institution.Institution>
        {
            public CreateInstitutionRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Institution.Institution>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Institution.Institution> Handle(Command command, CancellationToken cancellationToken)
            {
                // check institution type
                var institutionType = await _context.InstitutionType.FirstOrDefaultAsync(i => i.Id  == command.Request.InstitutionTypeId);
                if (institutionType == null)
                    throw new ArgumentException(
                        $"InstitutionType with id {command.Request.InstitutionTypeId} was not found in the database");

                var municipality = await _context.Municipality.FirstOrDefaultAsync(k => k.Id == command.Request.MunicipalityId);
                
                var healthcareOrganization = await _context.HealthcareOrganization.FirstOrDefaultAsync(h => h.Id == command.Request.InstitutionId);
                
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
                
                var institution = new Domain.Place.Institution();
                institution.Name = command.Request.InstitutionName;
                institution.Abbreviation = command.Request.Abbreviation;
                institution.HERId = command.Request.HERId;
                institution.InstitutionType = institutionType;
                institution.Municipality = municipality;
                institution.HealthcareOrganization = healthcareOrganization;

                institution.Users.Add(coordinator);
                institution.Users.Add(observer);
                _context.Institution.Add(institution);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Institution.Institution>(institution);
                return mapped;
            }
        }
    }
}
