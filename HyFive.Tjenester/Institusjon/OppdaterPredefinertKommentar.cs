using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domene.Place;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PredefinedComment = HyFive.Models.V1.Institution.PredefinedComment;

namespace HyFive.Services.Institusjon
{
    public class OppdaterPredefinertKommentar
    {
        public class Command : IRequest<bool>
        {
            public PredefinedComment PredefinertKommentar { get; set; }
            public int Institusjonid { get; set; }
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
                var kommentar = await _context.PredefinedComments.FirstOrDefaultAsync(pk => pk.Id == request.PredefinertKommentar.Id 
                                                                       && pk.InstitutionId == request.Institusjonid 
                                                                       && pk.SessionType == SessionType.ProtectiveEquipment, cancellationToken);
                if (kommentar == null)
                    return false;

                kommentar.Comment = request.PredefinertKommentar.Comment;

                _context.Update(kommentar);
                _context.SaveChanges();

                return true;
            }
        }
    }
}
