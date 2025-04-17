using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using HyFive.Models.V1.Session;
using MediatR;
using PredefinedComment = HyFive.Domain.Place.PredefinedComments;

namespace HyFive.Services.Institution
{
    public class CreatePredefinedComment
    {
        public class Command : IRequest<bool>
        {
            public CreatePredefinedCommentRequest NewPredefinedComment { get; set; }
            public int InstitutionId { get; set; }
            public SessionType SessionType { get; set; } = SessionType.ProtectiveEquipment;

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
                var comment = new PredefinedComment
                {
                    InstitutionId = request.InstitutionId,
                    Comment = request.NewPredefinedComment.Comment,
                    SessionType = (Domain.Place.SessionType)request.SessionType
                };

                _context.Add(comment);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }
    }
}
