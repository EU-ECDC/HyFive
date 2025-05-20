using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Institution
{
    public class UpdateInstitution
    {
        public class Command : IRequest<Models.V1.Institution.Institution>
        {
            public Models.V1.Institution.Institution Institution { get; set; }
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
                // Check institution type:
                var institutiontype = await _context.InstitutionType.FirstOrDefaultAsync(i => i.Id == command.Institution.InstitutionType.Id);
                if (institutiontype == null)
                    throw new ArgumentException(
                        $"Institution type with ID {command.Institution.InstitutionType.Id} was not found in the database.");

                var institution = await _context.Institution
                                                .Include(i => i.Municipality)
                                                .Include(i => i.HealthcareOrganization)
                                                .FirstOrDefaultAsync(i => i.Id == command.Institution.Id);

                if (command.Institution.Municipality != null && command.Institution.InstitutionType.Code == InstitutionTypeConstants.NursingHome)
                {
                    var municipality = await _context.Municipality.FirstOrDefaultAsync(k => k.Id == command.Institution.Municipality.Id);
                    institution.Municipality = municipality;
                }
                else
                {
                    institution.Municipality = null;
                }

                if (command.Institution.HealthcareOrganization != null && command.Institution.InstitutionType.Code == InstitutionTypeConstants.NursingHome)
                {
                    var healthcareOrganization = await _context.HealthcareOrganization.FirstOrDefaultAsync(h => h.Id == command.Institution.HealthcareOrganization.Id);
                    institution.HealthcareOrganization = healthcareOrganization;
                }
                else
                {
                    institution.HealthcareOrganization = null;
                }
                
                institution.Name = command.Institution.Name;
                institution.Abbreviation = command.Institution.Abbreviation;
                institution.HERId = command.Institution.HERId;
                institution.InstitutionType = institutiontype;

                _context.Institution.Update(institution);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Institution.Institution>(institution);
                return mapped;
            }
        }
    }
}
