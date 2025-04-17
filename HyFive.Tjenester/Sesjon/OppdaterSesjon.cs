using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using MediatR;

namespace HyFive.Services.Sesjon
{
    public class OppdaterSesjon
    {
        public class Command : IRequest<OppdaterSesjonRespons>
        {
            public Guid SesjonId { get; set; }
            public int InstitutionId { get; set; }
            public string Kommentar { get; set; }
            public DateTime? Starttidspunkt { get; set; }
        }

        public class Handler : IRequestHandler<Command, OppdaterSesjonRespons>
        {
            private readonly HandHygieneContext _databaseContext;

            public Handler(HandHygieneContext databaseContext)
            {
                _databaseContext = databaseContext;
            }
            
            public async Task<OppdaterSesjonRespons> Handle(Command request, CancellationToken cancellationToken)
            {
                var respons = new OppdaterSesjonRespons();

                var sesjon = _databaseContext.Session.FirstOrDefault(s => s.Id == request.SesjonId && s.Department.InstitutionId == request.InstitutionId);

                if (sesjon == null)
                {
                    throw new ArgumentException(
                        $"Kunne ikke finne sesjon med id  {request.SesjonId}");
                }

                if (request.Kommentar != null)
                {
                    sesjon.Kommentar = request.Kommentar;
                }

                await _databaseContext.SaveChangesAsync();
                respons.Suksess = true;
                return respons;
            }
        }

        public class OppdaterSesjonRespons
        {
            public bool Suksess { get; set; }
        }
    }
}