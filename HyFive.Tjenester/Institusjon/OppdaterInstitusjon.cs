using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Institusjon
{
    public class OppdaterInstitusjon
    {
        public class Command : IRequest<Models.V1.Institution.Institution>
        {
            public Models.V1.Institution.Institution Institusjon { get; set; }
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
                // sjekk institusjontype:
                var institusjontype = await _context.InstitutionType.FirstOrDefaultAsync(i => i.Id == command.Institusjon.InstitutionType.Id);
                if (institusjontype == null)
                    throw new ArgumentException(
                        $"Institusjonstype med id {command.Institusjon.InstitutionType.Id} fantes ikke i databasen");

                var institusjon = await _context.Institution
                                                .Include(i => i.Municipality)
                                                .Include(i => i.HealthcareOrganization)
                                                .FirstOrDefaultAsync(i => i.Id == command.Institusjon.Id);

                if (command.Institusjon.Comment != null && command.Institusjon.InstitutionType.Code == InstitutionTypeConstants.NursingHome)
                {
                    var kommune = await _context.Municipality.FirstOrDefaultAsync(k => k.Id == command.Institusjon.Comment.Id);
                    institusjon.Municipality = kommune;
                }
                else
                {
                    institusjon.Municipality = null;
                }

                if (command.Institusjon.HealthcareProvider != null && command.Institusjon.InstitutionType.Code == InstitutionTypeConstants.NursingHome)
                {
                    var helseforetak = await _context.HealthcareOrganization.FirstOrDefaultAsync(h => h.Id == command.Institusjon.HealthcareProvider.Id);
                    institusjon.HealthcareOrganization = helseforetak;
                }
                else
                {
                    institusjon.HealthcareOrganization = null;
                }
                
                institusjon.Name = command.Institusjon.Name;
                institusjon.Abbreviation = command.Institusjon.Abbreviation;
                institusjon.HERId = command.Institusjon.HERId;
                institusjon.InstitutionType = institusjontype;

                _context.Institution.Update(institusjon);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Institution.Institution>(institusjon);
                return mapped;
            }
        }
    }
}
