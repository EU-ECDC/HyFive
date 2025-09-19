using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Domain.User;

namespace HyFive.Services.Facility
{
    public class GetFacilitiesForObserver
    {
        public class Query : IRequest<Models.V1.Facility.Facility[]>
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Facility.Facility[]>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;
            private readonly IUserService _userService;

            public Handler(HandHygieneContext context, IMapper mapper, IUserService userService)
            {
                _context = context;
                _mapper = mapper;
                _userService = userService;
            }


            public async Task<Models.V1.Facility.Facility[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var facilities = await _context.Observer
                    
                    .AsNoTracking()
                    .Include(i => i.Facility)
                    .ThenInclude(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Where(_userService.HasEmailAndIsActive<Observer>(request.Email))
                    .Select(b => b.Facility)
                    .ToListAsync();

                var mapped = _mapper.Map<Models.V1.Facility.Facility[]>(facilities);
                return mapped;
            }
        }
    }
}