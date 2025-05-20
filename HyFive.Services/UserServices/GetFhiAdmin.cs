using AutoMapper;
using AutoMapper.QueryableExtensions;
using HyFive.DataAccess;
using HyFive.Models.V1.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.UserServices
{
    public class GetFhiAdmin
    {
        public class Query : IRequest<Models.V1.User.User[]>
        {
        }

        public class Handler : IRequestHandler<Query, Models.V1.User.User[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.User.User[]> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.User
                    .OfType<Domain.User.FhiAdmin>()
                    .AsNoTracking()
                    .ProjectTo<Models.V1.User.User>(_mapper.ConfigurationProvider)
                    .ToArrayAsync();
            }
        }
    }
}
