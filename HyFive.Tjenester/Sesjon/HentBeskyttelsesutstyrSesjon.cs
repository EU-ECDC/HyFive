using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Session;
using HyFive.Services.Authentication.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Sesjon
{
    public class HentBeskyttelsesutstyrSesjon
    {
        public class Query : IRequest<ProtectiveEquipmentSession>
        {
            public Guid SesjonId { get; set; }
            public string HPRNummer { get; set; }
            public string Pseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, ProtectiveEquipmentSession>
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

            public async Task<ProtectiveEquipmentSession> Handle(Query request, CancellationToken cancellationToken)
            {
                var sesjon = await _context.ProtectiveEquipmentSession
                    .AsNoTracking()
                    .Include(s => s.Department)
                    .Include(s => s.Observer).ThenInclude(obs => obs.Institution)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Beskyttelsesutstyrliste).ThenInclude(o => o.Utstyrstype)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Beskyttelsesutstyrliste).ThenInclude(o => o.Utstyrstype).ThenInclude(u => u.Feilbruktyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Beskyttelsesutstyrliste).ThenInclude(o => o.Feilbruktyper)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Settingtype)
                    .Include(s => s.Observasjoner).ThenInclude(o => o.Role)
                    .FirstOrDefaultAsync(s => s.Id == request.SesjonId);
                
                if (!_brukerService.HasHprOrPseudonymAndIsActive<Observer>(request.HPRNummer, request.Pseudonym).Compile()(sesjon.Observer))
                    throw new Exception(
                        $"Sesjonen med ID {request.SesjonId} er ikke tilknyttet bruker med HPR-nummer {request.HPRNummer} / Pseudonym XXX ");
                
                var beskyttelsesutstyrSesjon = _mapper.Map<ProtectiveEquipmentSession>(sesjon);
                return beskyttelsesutstyrSesjon;
            }
        }
    }
}
