using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Dataaksess;
using HyFive.Domene.Bruker;
using MediatR;

namespace HyFive.Tjenester.ForesporselOmBrukertilgang
{
    public class OpprettForesporselOmBrukertilgang
    {
        public class Command : IRequest<int>
        {
            public Modeller.V1.ForesporselOmBrukertilgang.OpprettForesporselOmBrukertilgangRequest ForesporselOmBrukertilgang { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly HandhygieneContext _context;

            public Handler(HandhygieneContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var foresporselFinnesAllerede = _context.ForesporselOmBrukertilgang
                                                            .FirstOrDefault(f =>
                                                                f.InstitusjonId == request.ForesporselOmBrukertilgang.InstitusjonId 
                                                                && f.IdentPseudonym == request.ForesporselOmBrukertilgang.IdentPseudonym
                                                                && f.Status == ForesporselOmBrukertilgangStatus.Registrert);

                if (foresporselFinnesAllerede != null)
                    return foresporselFinnesAllerede.Id;

                var institusjon = _context.Institusjon.Find(request.ForesporselOmBrukertilgang.InstitusjonId);
                var nyForesporselOmBrukertilgang = new Domene.Bruker.ForesporselOmBrukertilgang()
                {
                    BrukerFornavn = request.ForesporselOmBrukertilgang.BrukerFornavn,
                    BrukerEtternavn = request.ForesporselOmBrukertilgang.BrukerEtternavn,
                    HPRNummer = request.ForesporselOmBrukertilgang.HPRNummer != "0" ? request.ForesporselOmBrukertilgang.HPRNummer : null,
                    IdentPseudonym = request.ForesporselOmBrukertilgang.IdentPseudonym,
                    InstitusjonId = institusjon?.Id,
                    Status = ForesporselOmBrukertilgangStatus.Registrert,
                    Opprettettidspunkt = DateTime.UtcNow
                };

                _context.ForesporselOmBrukertilgang.Add(nyForesporselOmBrukertilgang);
                _context.SaveChanges();

                return nyForesporselOmBrukertilgang.Id;
            }
        }
    }
}
