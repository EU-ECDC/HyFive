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
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IUserService _brukerService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService brukerService)
            {
                _context = context;
                _mapper = mapper;
                _brukerService = brukerService;
            }

            public async Task<HanskeSesjon> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.GloveSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(r => r.Institusjon)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Rolle)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.HanskeMedIndikasjonTyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.HanskeUtenIndikasjonTyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.HandhygieneEtterHanskebrukType)
                    .FirstOrDefaultAsync(s => s.Id == request.SesjonId, cancellationToken);

                if (!_brukerService.HasHprOrPseudonymAndIsActive<Observator>(request.HPRNummer, request.Pseudonym).Compile()(sesjon.Observator))
                    throw new Exception(
                        $"Sesjonen med ID {request.SesjonId} er ikke tilknyttet bruker med HPR-nummer {request.HPRNummer}");

                var hanskeSesjon = _mapper.Map<HanskeSesjon>(sesjon);

                return hanskeSesjon;
            }
        }
    }
}