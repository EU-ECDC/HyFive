using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Gloves;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Hanske
{
    public class OppdaterHandhygieneEtterHanskebrukType
    {
        public class Command : IRequest<PostGloveHandHygieneType>
        {
            public PostGloveHandHygieneType HandhygieneEtterHanskebrukType { get; set; }
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
                var handhygieneEtterHanskebrukType = await _context.PostGloveHandHygiene
                    .FirstOrDefaultAsync(x => x.Id == request.HandhygieneEtterHanskebrukType.Id);

                if (handhygieneEtterHanskebrukType == null) throw new Exception($"Fant ikke handhygieneEtterHanskebrukType med id {request.HandhygieneEtterHanskebrukType.Id}");

                handhygieneEtterHanskebrukType.Name = request.HandhygieneEtterHanskebrukType.Name;

                _context.Update(handhygieneEtterHanskebrukType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<PostGloveHandHygieneType>(handhygieneEtterHanskebrukType);
                return mapped;
            }
        }
    }
}
