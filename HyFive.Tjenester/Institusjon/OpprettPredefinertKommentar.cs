using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Session;
using MediatR;
using PredefinertKommentar = HyFive.Domene.Place.PredefinedComments;

namespace HyFive.Services.Institusjon
{
    public class OpprettPredefinertKommentar
    {
        public class Command : IRequest<bool>
        {
            public CreatePredefinedCommentRequest NyPredefinertKommentar { get; set; }
            public int Institusjonid { get; set; }
            public SessionType SesjonType { get; set; } = SessionType.ProtectiveEquipment;

        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }
            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var kommentar = new PredefinertKommentar
                {
                    InstitutionId = request.Institusjonid,
                    Comment = request.NyPredefinertKommentar.Comment,
                    SessionType = (Domene.Place.SessionType)request.SesjonType
                };

                _context.Add(kommentar);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
