using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Observation.Gloves;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Glove
{
    public class UpdateGloveWithIndicationType
    {
        public class Command : IRequest<IndicatedGloveType>
        {
            public IndicatedGloveType GloveWithIndicationType { get; set; }
        }

        public class Handler : IRequestHandler<Command, IndicatedGloveType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<IndicatedGloveType> Handle(Command request, CancellationToken cancellationToken)
            {
                var gloveWithIndicationType = await _context.IndicatedGloveType
                    .FirstOrDefaultAsync(x => x.Id == request.GloveWithIndicationType.Id);

                if (gloveWithIndicationType == null) throw new Exception($"Could not find GloveWithIndicationType with ID: {request.GloveWithIndicationType.Id}");

                gloveWithIndicationType.Name = request.GloveWithIndicationType.Name;

                _context.Update(gloveWithIndicationType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<IndicatedGloveType>(gloveWithIndicationType);
                return mapped;
            }
        }
    }
}
