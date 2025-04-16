using HyFive.DataAccess;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class HasTransferredSessionToFHI
    {
        public class Command : IRequest<bool>
        {
            public int DepartmentId { get; set; }
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
                var SessionsTransferredToFHI = await _context.Session.Where(s => s.Department.Id == request.DepartmentId && s.TransmissionStatus.Code == TransferStatusTypeConstants.TransferredToFhi).AnyAsync();

                return SessionsTransferredToFHI;
            }
        }
    }

}
