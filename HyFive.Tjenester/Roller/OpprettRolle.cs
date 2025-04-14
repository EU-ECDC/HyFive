using System;
using System.Collections.Generic;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HyFive.Models.V1.Observation;

namespace HyFive.Services.Roller
{
    public class OpprettRolle
    {
        public class Command : IRequest<Models.V1.Observation.Role>
        {
            public CreateRoleRequest Request { get; set; }
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
                // sjekk institusjon
                var rolle = new Domene.Observation.Role()
                {
                    Name = command.Request.Name,
                    Description = command.Request.Description
                };

                _context.Role.Add(rolle);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<Models.V1.Observation.Role>(rolle);
                return mapped;
            }
        }
    }
}
