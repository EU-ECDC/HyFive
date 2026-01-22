using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.User
{
    public class UpdateCoordinator
    {
        public class Command : IRequest<Models.V1.User.User>
        {
            public Models.V1.User.User User { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.User.User>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Models.V1.User.User> Handle(Command command, CancellationToken cancellationToken)
            {
                await UserUpdateHelper.UpdateUserBaseFields<Coordinator>(
                    _context,
                    command.User,
                    cancellationToken
                );

                return _mapper.Map<Models.V1.User.User>(
                    await _context.User.OfType<Coordinator>().FirstAsync(u => u.Id == command.User.Id, cancellationToken)
                );
            }
        }
    }
}
