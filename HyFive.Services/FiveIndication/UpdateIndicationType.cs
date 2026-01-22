using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HyFive.Models.V1.Observation;
using System;
using HyFive.Domain.Exceptions;

namespace HyFive.Services.FiveIndication
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
                var indicationTypes = await _context.IndicationTypes
                    .FirstOrDefaultAsync(i => i.Id == request.IndicationType.Id, cancellationToken);

                if (indicationTypes == null) throw new DomainException("IndicationTypeNotFound", request.IndicationType.Id);

                indicationTypes.Name = request.IndicationType.Name;
                indicationTypes.Number = request.IndicationType.Number;

                _context.IndicationTypes.Update(indicationTypes);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedIndicationType =
                    _mapper.Map<Domain.Observation.IndicationTypes, IndicationType>(indicationTypes);

                return mappedIndicationType;
            }
        }
    }
}