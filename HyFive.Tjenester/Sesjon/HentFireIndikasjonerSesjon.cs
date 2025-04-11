using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Modeller.V1.Sesjon;
using HyFive.Tjenester.Autentisering.Bruker;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Sesjon
{
    public class HentFireIndikasjonerSesjon
    {
        public class Query : IRequest<FireIndikasjonerSesjon>
        {
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
            public Guid SesjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FireIndikasjonerSesjon>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IBrukerService _brukerService;

            public Handler(HandHygieneContext context, IMapper mapper, IBrukerService brukerService)
            {
                _context = context;
                _mapper = mapper;
                _brukerService = brukerService;
            }
            public async Task<FireIndikasjonerSesjon> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.FourIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institusjon)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Aktivitet).ThenInclude(a => a.AktivitetType)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Indikasjonstyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Rolle)
                    .FirstOrDefaultAsync(s => s.Id == request.SesjonId, cancellationToken);

                if (!_brukerService.HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer, request.Pseudonym).Compile()(sesjon.Observator))
                    throw new Exception(
                        $"Sesjonen med ID {request.SesjonId} er ikke tilknyttet bruker med innlogget brukers pseudonym eller HPR-nummer {request.HPRNummer}");

                var fireIndikasjonerSesjon = _mapper.Map<Domene.Session.FourIndicationsSession, FireIndikasjonerSesjon>(sesjon);
                return fireIndikasjonerSesjon;
            }
        }
    }
}
