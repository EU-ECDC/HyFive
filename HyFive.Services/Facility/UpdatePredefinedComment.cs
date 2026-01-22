using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.Place;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PredefinedComment = HyFive.Models.V1.Facility.PredefinedComment;

namespace HyFive.Services.Facility
{
    public class UpdatePredefinedComment
    {
        public class Command : IRequest<bool>
        {
            public PredefinedComment PredefinedComment { get; set; }
            public int FacilityId { get; set; }
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
                var comment = await _context.PredefinedComment.FirstOrDefaultAsync(pk => pk.Id == request.PredefinedComment.Id 
                                                                       && pk.FacilityId == request.FacilityId 
                                                                       && pk.SessionType == SessionType.ProtectiveEquipment, cancellationToken);
                if (comment == null)
                    return false;

                comment.Comment = request.PredefinedComment.Comment;

                _context.Update(comment);
                await  _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
