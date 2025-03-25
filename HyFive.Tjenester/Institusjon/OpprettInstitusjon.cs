using System;
using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Institusjon;

namespace HyFive.Tjenester.Institusjon
{
    public class OpprettInstitusjon
    {
        public class Command : IRequest<Modeller.V1.Institusjon.Institusjon>
        {
            public OpprettInstitusjonRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Modeller.V1.Institusjon.Institusjon>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Modeller.V1.Institusjon.Institusjon> Handle(Command command, CancellationToken cancellationToken)
            {
                // sjekk institusjontype:
                var institusjontype = await _context.InstitusjonType.FirstOrDefaultAsync(i => i.Id  == command.Request.InstitusjonTypeId);
                if (institusjontype == null)
                    throw new ArgumentException(
                        $"InstitusjonType med id {command.Request.InstitusjonTypeId} fantes ikke i databasen");

                var kommune = await _context.Kommune.FirstOrDefaultAsync(k => k.Id == command.Request.KommuneId);
                
                var helseforetak = await _context.Helseforetak.FirstOrDefaultAsync(h => h.Id == command.Request.HelseforetakId);
                
                var koordinator = new Koordinator()
                {
                    Fornavn = command.Request.KoordinatorFornavn,
                    Etternavn = command.Request.KoordinatorEtternavn,
                    HPRNummer = command.Request.KoordinatorHPRnummer,
                    IdentPseudonym =  command.Request.KoordinatorPseudonym,
                    Epost = command.Request.KoordinatorEpost
                };

                var observator = new Observator()
                {
                    Fornavn = command.Request.KoordinatorFornavn,
                    Etternavn = command.Request.KoordinatorEtternavn,
                    HPRNummer = command.Request.KoordinatorHPRnummer,
                    IdentPseudonym = command.Request.KoordinatorPseudonym,
                    Epost = command.Request.KoordinatorEpost
                };
                
                var institusjon = new Domene.Sted.Institusjon();
                institusjon.Navn = command.Request.Institusjonsnavn;
                institusjon.Forkortelse = command.Request.Forkortelse;
                institusjon.HERId = command.Request.HERId;
                institusjon.Institusjontype = institusjontype;
                institusjon.Kommune = kommune;
                institusjon.Helseforetak = helseforetak;

                institusjon.Brukere.Add(koordinator);
                institusjon.Brukere.Add(observator);
                _context.Institusjon.Add(institusjon);
                await _context.SaveChangesAsync(cancellationToken);
                var mapped = _mapper.Map<Modeller.V1.Institusjon.Institusjon>(institusjon);
                return mapped;
            }
        }
    }
}
