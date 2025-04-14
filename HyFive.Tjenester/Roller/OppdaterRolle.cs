using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Roller
{
    public class OppdaterRolle
    {
        public class Command : IRequest<Models.V1.Observation.Role>
        {
            public UpdateRoleRequest Request { get; set; }
        }

        public class Handler : IRequestHandler<Command, Models.V1.Observation.Role>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<Models.V1.Observation.Role> Handle(Command command, CancellationToken cancellationToken)
            {
                var rolle = await _context.Role.FirstOrDefaultAsync(i => i.Id == command.Request.Id);
                rolle.Name = command.Request.Name;
                rolle.Description = command.Request.Description;

                _context.Role.Update(rolle);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<Models.V1.Observation.Role>(rolle);
                return mapped;
            }
        }
    }
}
