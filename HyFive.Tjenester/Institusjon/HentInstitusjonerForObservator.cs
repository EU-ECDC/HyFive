using AutoMapper;
using HyFive.Dataaksess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Domene.Bruker;
using HyFive.Tjenester.Autentisering.Bruker;

namespace HyFive.Tjenester.Institusjon
{
    public class HentInstitusjonerForObservator
    {
        public class Query : IRequest<Modeller.V1.Institusjon.Institusjon[]>
        {
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.Institusjon.Institusjon[]>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IBrukerService _brukerService;

            public Handler(HandhygieneContext context, IMapper mapper, IBrukerService brukerService)
            {
                _context = context;
                _mapper = mapper;
                _brukerService = brukerService;
            }


            public async Task<Modeller.V1.Institusjon.Institusjon[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var institusjoner = await _context.Observator
                    
                    .AsNoTracking()
                    .Include(i => i.Institusjon)
                    .ThenInclude(i => i.Avdelinger)
                    .ThenInclude(a => a.Roller)
                    .Where(_brukerService.HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer, request.Pseudonym))
                    .Select(b => b.Institusjon)
                    .ToListAsync();

                var mapped = _mapper.Map<Modeller.V1.Institusjon.Institusjon[]>(institusjoner);
                return mapped;
            }
        }
    }
}