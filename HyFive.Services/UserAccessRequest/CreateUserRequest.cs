using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;

namespace HyFive.Services.UserAccessRequest
{
    public class CreateUserRequest
    {
        public class Command : IRequest<bool>
        {
            public int RequestId { get; set; }
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
            }


            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var request = await _context.UserAccessRequest.FindAsync(command.RequestId);
                
                if (request == null) return false;

                var institution = await GetInstitution(request);

                if (institution == null) return false;

                if (!ExistsObserverForInstitution(request.IdentityPseudonym, institution.Id))
                    CreateObserver(request, institution);

                var user = _context.User.FirstOrDefault(b => b.Email == command.Email);

                if(user == null) return false;

                request.Status = UserAccessRequestStatus.Approved;
                request.ProcessedTime = DateTime.UtcNow;
                request.UserFirstName = user.FirstName;
                request.ProcessedByUsername = user.FirstName + " " + user.LastName;

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }

            private bool ExistsObserverForInstitution(string IdentifierPseudonymRequest, int institutionId)
            {
                return _context.User.OfType<Observer>().Any(x => x.IdentityPseudonym == IdentifierPseudonymRequest &&
                                                                     x.Institution.Id == institutionId);
            }

            private void CreateObserver(Domain.User.UserAccessRequest request, Domain.Place.Institution institution)
            {
                var observer = new Observer()
                {
                    FirstName = request.UserFirstName,
                    LastName = request.UserLastName,
                    Institution = institution,
                    HPRNumber = request.HPRNumber,
                    Email = request.Email,
                    IdentityPseudonym = request.IdentityPseudonym,
                    CreatedTime = DateTime.UtcNow,
                    IsDeactivated = false,
                };
                _context.User.Add(observer);
            }

            private async Task<Domain.Place.Institution> GetInstitution(Domain.User.UserAccessRequest request)
            {
                Domain.Place.Institution institution = null;
                if (request.InstitutionId != null)
                {
                    institution = await _context.Institution.FindAsync(request.InstitutionId);
                }
                
                return institution;
            }
        }
    }
}
