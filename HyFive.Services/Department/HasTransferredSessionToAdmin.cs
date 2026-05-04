using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Department
{
    public class HasTransferredSessionToAdmin
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
                //enforce it is a Department OU
                var isDepartment = await _context.OrganisationUnit
                    .AsNoTracking()
                    .Include(ou => ou.LevelRef)
                    .AnyAsync(ou => ou.Id == request.DepartmentId
                                 && ou.LevelRef.Level == OrganisationUnitLevels.Department, cancellationToken);

                if (!isDepartment)
                    throw new DomainException("OrganisationUnitIsNotDepartment");

                var SessionsTransferredToAdmin = await _context.Session.Where(s => s.OrganisationUnitId == request.DepartmentId && s.TransferStatus.Code == TransferStatusTypeConstants.TransferredToAdmin).AnyAsync();

                return SessionsTransferredToAdmin;
            }
        }
    }

}
