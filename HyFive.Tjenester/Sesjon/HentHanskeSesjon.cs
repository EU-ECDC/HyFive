using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Autentisering.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Sesjon
{
    public class HentHanskeSesjon
    {
        public class Query : IRequest<HanskeSesjon>
        {
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
            public Guid SesjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, HanskeSesjon>
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

            public async Task<HanskeSesjon> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.HanskeSesjon
                    .AsNoTracking()
                    .Include(s => s.Avdeling)
                    .Include(s => s.Observator).ThenInclude(r => r.Institusjon)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Rolle)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.HanskeMedIndikasjonTyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.HanskeUtenIndikasjonTyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.HandhygieneEtterHanskebrukType)
                    .FirstOrDefaultAsync(s => s.Id == request.SesjonId, cancellationToken);

                if (!_brukerService.HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer, request.Pseudonym).Compile()(sesjon.Observator))
                    throw new Exception(
                        $"Sesjonen med ID {request.SesjonId} er ikke tilknyttet bruker med HPR-nummer {request.HPRNummer}");

                var hanskeSesjon = _mapper.Map<HanskeSesjon>(sesjon);

                return hanskeSesjon;
            }
        }
    }
}