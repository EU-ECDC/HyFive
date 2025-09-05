using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;

namespace HyFive.Services.UserAccessRequest
{
    public class GetAlreadySentRequest
    {
        public class Query : IRequest<Models.V1.UserAccessRequest.UserAccessRequest>
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.UserAccessRequest.UserAccessRequest>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }
            public async Task<Models.V1.UserAccessRequest.UserAccessRequest> Handle(Query request, CancellationToken cancellationToken)
            {
                var userAccessRequest = _context.UserAccessRequest
                                    .OrderByDescending(f => f.CreatedTime)
                                    .FirstOrDefault(f =>
                                    f.Email == request.Email &&
                                    f.Status == UserAccessRequestStatus.Registered);

                if (userAccessRequest == null)
                    return null;

                var institution = _context.Institution.FirstOrDefault(i => i.Id == userAccessRequest.InstitutionId);

                if (institution == null)
                    return null;

                return _mapper.Map<Models.V1.UserAccessRequest.UserAccessRequest>(userAccessRequest);
            }
        }
    }
}
