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
    public class UpdateGloveWithoutIndicationType
    {
        public class Command : IRequest<GeneralPurposeGloveType>
        {
            public GeneralPurposeGloveType GloveWithoutIndicationType { get; set; }
        }

        public class Handler : IRequestHandler<Command, GeneralPurposeGloveType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<GeneralPurposeGloveType> Handle(Command request, CancellationToken cancellationToken)
            {
                var gloveWithoutIndicationType = await _context.GeneralPurposeGloveType
                    .FirstOrDefaultAsync(x => x.Id == request.GloveWithoutIndicationType.Id);

                if (gloveWithoutIndicationType == null) throw new Exception($"Could not find gloveWithoutIndicationType with ID: {request.GloveWithoutIndicationType.Id}");

                gloveWithoutIndicationType.Name = request.GloveWithoutIndicationType.Name;

                _context.Update(gloveWithoutIndicationType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<GeneralPurposeGloveType>(gloveWithoutIndicationType);
                return mapped;
            }
        }
    }
}
