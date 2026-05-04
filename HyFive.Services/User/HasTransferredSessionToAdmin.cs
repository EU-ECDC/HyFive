using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.User
{
    public class HasTransferredSessionToAdmin
    {
        public class Command : IRequest<bool>
        {
            public int ObserverId { get; set; }
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
                return await _context.Session
                    .AsNoTracking()
                    .AnyAsync(
                        s => s.ObserverId == request.ObserverId &&
                             s.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin,
                        cancellationToken);
            }
        }
    }
}
