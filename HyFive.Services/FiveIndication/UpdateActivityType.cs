using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Observation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.FiveIndication
{
    public class UpdateActivityType
    {
        public class Command : IRequest<ActivityType>
        {
            public ActivityType ActivityType { get; set; }
        }

        public class Handler : IRequestHandler<Command, ActivityType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<ActivityType> Handle(Command command, CancellationToken cancellationToken)
            {
                var activityType = await _context.ActivityType
                    .FirstOrDefaultAsync(i => i.Id == command.ActivityType.Id, cancellationToken);

                if (activityType == null) throw new DomainException("ActivityTypeNotFound", command.ActivityType.Id);

                activityType.Name = command.ActivityType.Name;

                _context.ActivityType.Update(activityType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<ActivityType>(activityType);
                return mapped;
            }
        }
    }
}