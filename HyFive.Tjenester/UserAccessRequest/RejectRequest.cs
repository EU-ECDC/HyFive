using HyFive.DataAccess;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.UserAccessRequest
{
    public class RejectRequest
    {
        public class Command : IRequest<bool>
        {
            public int RequestId { get; set; }
            public string IdentityPseudonym { get; set; }
            public string HPRNumber { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var request = await _context.UserAccessRequest.FindAsync(command.RequestId);

                if (request == null) return false;

                var user = _context.User.FirstOrDefault(b => b.IdentityPseudonym == command.IdentityPseudonym
                                                    || b.HPRNumber == command.HPRNumber);

                if (user == null) return false;

                request.Status = Domain.User.UserAccessRequestStatus.Rejected;
                request.ProcessedTime = DateTime.UtcNow;
                request.ProcessedByUserID = user.Id;
                request.ProcessedByUsername = user.FirstName + " " + user.LastName;

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
        }


    }
}
