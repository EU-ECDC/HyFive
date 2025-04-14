using System;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Bruker;
using HyFive.Models.V1.Institution;

namespace HyFive.Services.Institusjon
{
    public class OpprettInstitusjon
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
                // sjekk institusjontype:
                var institusjontype = await _context.InstitutionType.FirstOrDefaultAsync(i => i.Id  == command.Request.InstitutionTypeId);
                if (institusjontype == null)
                    throw new ArgumentException(
                        $"InstitusjonType med id {command.Request.InstitutionTypeId} fantes ikke i databasen");

                var kommune = await _context.Municipality.FirstOrDefaultAsync(k => k.Id == command.Request.MunicipalityId);
                
                var helseforetak = await _context.HealthcareProvider.FirstOrDefaultAsync(h => h.Id == command.Request.InstitutionId);
                
                var koordinator = new Koordinator()
                {
                    Fornavn = command.Request.CoordinatorFirstName,
                    Etternavn = command.Request.CoordinatorLastName,
                    HPRNummer = command.Request.CoordinatorHPRNumber,
                    IdentPseudonym =  command.Request.CoordinatorPseudonym,
                    Epost = command.Request.CoordinatorEmail
                };

                var observator = new Observator()
                {
                    Fornavn = command.Request.CoordinatorFirstName,
                    Etternavn = command.Request.CoordinatorLastName,
                    HPRNummer = command.Request.CoordinatorHPRNumber,
                    IdentPseudonym = command.Request.CoordinatorPseudonym,
                    Epost = command.Request.CoordinatorEmail
                };
                
                var institusjon = new Domene.Place.Institution();
                institusjon.Name = command.Request.InstitutionName;
                institusjon.Abbreviation = command.Request.Abbreviation;
                institusjon.HERId = command.Request.HERId;
                institusjon.InstitutionType = institusjontype;
                institusjon.Municipality = kommune;
                institusjon.HealthcareProvider = helseforetak;

                institusjon.Users.Add(koordinator);
                institusjon.Users.Add(observator);
                _context.Institution.Add(institusjon);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Models.V1.Institution.Institution>(institusjon);
                return mapped;
            }
        }
    }
}
