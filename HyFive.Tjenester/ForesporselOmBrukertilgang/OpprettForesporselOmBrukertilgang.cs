using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Bruker;
using MediatR;

namespace HyFive.Services.ForesporselOmBrukertilgang
{
    public class OpprettForesporselOmBrukertilgang
    {
        public class Command : IRequest<int>
        {
            public Models.V1.UserAccessRequest.CreateUserAccessRequest ForesporselOmBrukertilgang { get; set; }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var foresporselFinnesAllerede = _context.UserAccessRequest
                                                            .FirstOrDefault(f =>
                                                                f.InstitusjonId == request.ForesporselOmBrukertilgang.InstitutionId 
                                                                && f.IdentPseudonym == request.ForesporselOmBrukertilgang.IdentityPseudonym
                                                                && f.Status == ForesporselOmBrukertilgangStatus.Registrert);

                if (foresporselFinnesAllerede != null)
                    return foresporselFinnesAllerede.Id;

                var institusjon = _context.Institution.Find(request.ForesporselOmBrukertilgang.InstitutionId);
                var nyForesporselOmBrukertilgang = new Domene.Bruker.ForesporselOmBrukertilgang()
                {
                    BrukerFornavn = request.ForesporselOmBrukertilgang.UserFirstName,
                    BrukerEtternavn = request.ForesporselOmBrukertilgang.UserLastName,
                    HPRNummer = request.ForesporselOmBrukertilgang.HPRNumber != "0" ? request.ForesporselOmBrukertilgang.HPRNumber : null,
                    IdentPseudonym = request.ForesporselOmBrukertilgang.IdentityPseudonym,
                    InstitusjonId = institusjon?.Id,
                    Status = ForesporselOmBrukertilgangStatus.Registrert,
                    Opprettettidspunkt = DateTime.Now
                };

                _context.UserAccessRequest.Add(nyForesporselOmBrukertilgang);
                _context.SaveChanges();

                return nyForesporselOmBrukertilgang.Id;
            }
        }
    }
}
