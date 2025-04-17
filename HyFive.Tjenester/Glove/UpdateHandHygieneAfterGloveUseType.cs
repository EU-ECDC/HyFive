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
    public class UpdateHandHygieneAfterGloveUseType
    {
        public class Command : IRequest<PostGloveHandHygieneType>
        {
            public PostGloveHandHygieneType HandHygieneAfterGloveUseType { get; set; }
        }

        public class Handler : IRequestHandler<Command, PostGloveHandHygieneType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<PostGloveHandHygieneType> Handle(Command request, CancellationToken cancellationToken)
            {
                var handHygieneAfterGloveUseType = await _context.PostGloveHandHygiene
                    .FirstOrDefaultAsync(x => x.Id == request.HandHygieneAfterGloveUseType.Id);

                if (handHygieneAfterGloveUseType == null) throw new Exception($"Could not find handHygieneAfterGloveUseType with ID: {request.HandHygieneAfterGloveUseType.Id}");

                handHygieneAfterGloveUseType.Name = request.HandHygieneAfterGloveUseType.Name;

                _context.Update(handHygieneAfterGloveUseType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<PostGloveHandHygieneType>(handHygieneAfterGloveUseType);
                return mapped;
            }
        }
    }
}
