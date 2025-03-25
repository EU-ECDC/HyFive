using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class OppdaterBeskyttelsesutstyrsettingType
    {
        public class Command : IRequest<BeskyttelsesutstyrsettingType>
        {
            public BeskyttelsesutstyrsettingType SettingType { get; set; }
        }

        public class Handler : IRequestHandler<Command, BeskyttelsesutstyrsettingType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<BeskyttelsesutstyrsettingType> Handle(Command request, CancellationToken cancellationToken)
            {
                var settingType = await _context.BeskyttelsesutstyrsettingType
                    .FirstOrDefaultAsync(bust => bust.Id == request.SettingType.Id, cancellationToken);

                if (settingType == null) throw new Exception($"Fant ikke beskyttelsesutstyrsettingType med id {request.SettingType.Id}");

                settingType.Navn = request.SettingType.Navn;

                _context.BeskyttelsesutstyrsettingType.Update(settingType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<BeskyttelsesutstyrsettingType>(settingType);
                return mapped;
            }
        }
    }
}