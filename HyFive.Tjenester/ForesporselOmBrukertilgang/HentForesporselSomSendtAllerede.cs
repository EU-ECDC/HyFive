using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class HentForesporselSomSendtAllerede
    {
        public class Query : IRequest<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang>
        {
            public string HprNummer { get; set; }
            public string IdentPseudonym { get; set; }
        }

        public class Handler : IRequestHandler<Query, Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang> Handle(Query request, CancellationToken cancellationToken)
            {
                var foresporsel = _context.ForesporselOmBrukertilgang
                                    .OrderByDescending(f => f.Opprettettidspunkt)
                                    .FirstOrDefault(f =>
                                    f.HPRNummer == request.HprNummer &&
                                    f.IdentPseudonym == request.IdentPseudonym &&
                                    f.Status == ForesporselOmBrukertilgangStatus.Registrert);

                if (foresporsel == null)
                    return null;

                var institusjon = _context.Institusjon.FirstOrDefault(i => i.Id == foresporsel.InstitusjonId);

                if (institusjon == null)
                    return null;

                return _mapper.Map<Modeller.V1.ForesporselOmBrukertilgang.ForesporselOmBrukertilgang>(foresporsel);
            }
        }
    }
}
