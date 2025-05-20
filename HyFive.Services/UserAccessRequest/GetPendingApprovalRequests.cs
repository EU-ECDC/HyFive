using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.UserAccessRequest
{
    public class GetPendingApprovalRequests
    {
        public class Query : IRequest<Models.V1.UserAccessRequest.UserAccessRequest[]>
        {
            public int InstitutionId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.UserAccessRequest.UserAccessRequest[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.UserAccessRequest.UserAccessRequest[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.UserAccessRequest
                    .AsNoTracking()
                    .Where(f => f.Status == UserAccessRequestStatus.Registered && f.InstitutionId.Value == request.InstitutionId)
                    .ProjectTo<Models.V1.UserAccessRequest.UserAccessRequest>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
