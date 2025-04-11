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
    public class HentBeskyttelsesutstyrSesjon
    {
        public class Query : IRequest<BeskyttelsesutstyrSesjon>
        {
            public Guid SesjonId { get; set; }
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, BeskyttelsesutstyrSesjon>
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

            public async Task<BeskyttelsesutstyrSesjon> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.ProtectiveEquipmentSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institusjon)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Beskyttelsesutstyrliste).ThenInclude(o => o.Utstyrstype)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Beskyttelsesutstyrliste).ThenInclude(o => o.Utstyrstype).ThenInclude(u => u.Feilbruktyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Beskyttelsesutstyrliste).ThenInclude(o => o.Feilbruktyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Settingtype)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Rolle)
                    .FirstOrDefaultAsync(s => s.Id == request.SesjonId);
                
                if (!_brukerService.HarHprEllerPseudonymOgErAktiv<Observator>(request.HPRNummer, request.Pseudonym).Compile()(sesjon.Observator))
                    throw new Exception(
                        $"Sesjonen med ID {request.SesjonId} er ikke tilknyttet bruker med HPR-nummer {request.HPRNummer} / Pseudonym XXX ");
                
                var beskyttelsesutstyrSesjon = _mapper.Map<BeskyttelsesutstyrSesjon>(sesjon);
                return beskyttelsesutstyrSesjon;
            }
        }
    }
}
