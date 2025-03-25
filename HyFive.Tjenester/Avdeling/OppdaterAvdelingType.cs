using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Avdeling
{
    public class OppdaterAvdelingType
    {
        public class Command : IRequest<AvdelingType>
        {
            public AvdelingType AvdelingType { get; set; }
        }

        public class Handler : IRequestHandler<Command, AvdelingType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<AvdelingType> Handle(Command request, CancellationToken cancellationToken)
            {
                var avdelingtype = await _context.AvdelingType.FirstOrDefaultAsync(a => a.Id == request.AvdelingType.Id);
                if (avdelingtype != default(Domene.Sted.AvdelingType))
                {
                    avdelingtype.Navn = request.AvdelingType.Navn;

                    _context.Update(avdelingtype);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<Modeller.V1.Institusjon.AvdelingType>(avdelingtype);
                }

                throw new ArgumentException($"Kunne ikke finne avdelingtype med id {request.AvdelingType.Id}");
            }
        }
    }
}