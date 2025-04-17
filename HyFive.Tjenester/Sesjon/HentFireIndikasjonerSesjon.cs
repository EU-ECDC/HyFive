using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Sesjon
{
    public class HentFireIndikasjonerSesjon
    {
        public class Query : IRequest<FourIndicationsSession>
        {
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
            public Guid SesjonId { get; set; }
        }

        public class Handler : IRequestHandler<Query, FourIndicationsSession>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IUserService _brukerService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService brukerService)
            {
                _context = context;
                _mapper = mapper;
                _brukerService = brukerService;
            }
            public async Task<FourIndicationsSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.FourIndicationsSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institution)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Aktivitet).ThenInclude(a => a.AktivitetType)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Indikasjonstyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SesjonId, cancellationToken);

                if (!_brukerService.HasHprOrPseudonymAndIsActive<Observer>(request.HPRNummer, request.Pseudonym).Compile()(sesjon.Observer))
                    throw new Exception(
                        $"Sesjonen med ID {request.SesjonId} er ikke tilknyttet bruker med innlogget brukers pseudonym eller HPR-nummer {request.HPRNummer}");

                var fireIndikasjonerSesjon = _mapper.Map<Domene.Session.FourIndicationsSession, FourIndicationsSession>(sesjon);
                return fireIndikasjonerSesjon;
            }
        }
    }
}
