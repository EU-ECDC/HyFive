using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Services.Authentication.User;
using HyFive.Domain.User;

namespace HyFive.Services.Institution
{
    public class GetInstitutionsForObserver
    {
        public class Query : IRequest<Models.V1.Institution.Institution[]>
        {
            public string Email { get; set; }
        }

        public class Handler : IRequestHandler<Query, Models.V1.Institution.Institution[]>
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


            public async Task<Models.V1.Institution.Institution[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var institutions = await _context.Observer
                    
                    .AsNoTracking()
                    .Include(i => i.Institution)
                    .ThenInclude(i => i.Departments)
                    .ThenInclude(a => a.Roles)
                    .Where(_userService.HasEmailAndIsActive<Observer>(request.Email))
                    .Select(b => b.Institution)
                    .ToListAsync();

                var mapped = _mapper.Map<Models.V1.Institution.Institution[]>(institutions);
                return mapped;
            }
        }
    }
}