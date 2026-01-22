using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Observation.Gloves;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Glove
{
    public class UpdateGloveWithIndicationType
    {
        public class Command : IRequest<GloveWithIndicationType>
        {
            public GloveWithIndicationType GloveWithIndicationType { get; set; }
        }

        public class Handler : IRequestHandler<Command, GloveWithIndicationType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<GloveWithIndicationType> Handle(Command request, CancellationToken cancellationToken)
            {
                var gloveWithIndicationType = await _context.GloveWithIndicationType
                    .FirstOrDefaultAsync(x => x.Id == request.GloveWithIndicationType.Id);

                if (gloveWithIndicationType == null) throw new DomainException("GloveWithIndicationTypeNotFound",request.GloveWithIndicationType.Id);

                gloveWithIndicationType.Name = request.GloveWithIndicationType.Name;

                _context.Update(gloveWithIndicationType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<GloveWithIndicationType>(gloveWithIndicationType);
                return mapped;
            }
        }
    }
}
