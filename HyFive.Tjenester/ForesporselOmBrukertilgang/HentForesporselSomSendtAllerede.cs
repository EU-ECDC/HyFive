using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class HentForesporselSomSendtAllerede
    {
        public class Query : IRequest<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest>
        {
            public string HprNummer { get; set; }
            public string IdentPseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest> Handle(Query request, CancellationToken cancellationToken)
            {
                var foresporsel = _context.UserAccessRequest
                                    .OrderByDescending(f => f.Opprettettidspunkt)
                                    .FirstOrDefault(f =>
                                    f.HPRNummer == request.HprNummer &&
                                    f.IdentPseudonym == request.IdentPseudonym &&
                                    f.Status == ForesporselOmBrukertilgangStatus.Registrert);

                if (foresporsel == null)
                    return null;

                var institusjon = _context.Institution.FirstOrDefault(i => i.Id == foresporsel.InstitusjonId);

                if (institusjon == null)
                    return null;

                return _mapper.Map<Modeller.V1.ForesporselOmBrukertilgang.UserAccessRequest>(foresporsel);
            }
        }
    }
}
