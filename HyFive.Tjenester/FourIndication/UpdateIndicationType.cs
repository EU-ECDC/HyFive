using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HyFive.Models.V1.Observation;
using System;

namespace HyFive.Services.FourIndication
{
    public class UpdateIndicationType
    {
        public class Command : IRequest<IndicationType>
        {
            public IndicationType IndicationType { get; set; }
        }

        public class Handler : IRequestHandler<Command, IndicationType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IndicationType> Handle(Command request, CancellationToken cancellationToken)
            {
                var indicationType = await _context.IndicationType
                    .FirstOrDefaultAsync(i => i.Id == request.IndicationType.Id, cancellationToken);

                if (indicationType == null) throw new Exception($"Could not find indication type with ID: {request.IndicationType.Id}");

                indicationType.Name = request.IndicationType.Name;
                indicationType.Number = request.IndicationType.Number;

                _context.IndicationType.Update(indicationType);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedIndicationType =
                    _mapper.Map<Domain.Observation.IndicationType, IndicationType>((Domain.Observation.IndicationType)indicationType);

                return mappedIndicationType;
            }
        }
    }
}