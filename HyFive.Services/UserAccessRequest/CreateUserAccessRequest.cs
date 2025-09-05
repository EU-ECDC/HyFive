using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;

namespace HyFive.Services.UserAccessRequest
{
    public class CreateUserAccessRequest
    {
        public class Command : IRequest<int>
        {
            public Models.V1.UserAccessRequest.CreateUserAccessRequest UserAccessRequest { get; set; }
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
                var requestAlreadyExists = _context.UserAccessRequest
                                                            .FirstOrDefault(f =>
                                                                f.InstitutionId == request.UserAccessRequest.InstitutionId 
                                                                && f.IdentityPseudonym == request.UserAccessRequest.IdentityPseudonym
                                                                && f.Status == UserAccessRequestStatus.Registered);

                if (requestAlreadyExists != null)
                    return requestAlreadyExists.Id;

                var institution = _context.Institution.Find(request.UserAccessRequest.InstitutionId);
                var newUserAccessRequest = new Domain.User.UserAccessRequest()
                {
                    UserFirstName = request.UserAccessRequest.UserFirstName,
                    UserLastName = request.UserAccessRequest.UserLastName,
                    HPRNumber = request.UserAccessRequest.HPRNumber != "0" ? request.UserAccessRequest.HPRNumber : null,
                    Email = request.UserAccessRequest.Email,
                    IdentityPseudonym = request.UserAccessRequest.IdentityPseudonym,
                    InstitutionId = institution?.Id,
                    Status = UserAccessRequestStatus.Registered,
                    CreatedTime = DateTime.UtcNow
                };

                _context.UserAccessRequest.Add(newUserAccessRequest);
                _context.SaveChanges();

                return newUserAccessRequest.Id;
            }
        }
    }
}
