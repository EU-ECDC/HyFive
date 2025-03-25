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
    public class OpprettAvdelingType
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
                var exists = await _context.AvdelingType.AnyAsync(r => r.Kode == request.AvdelingType.Kode);
                if (exists)
                    throw new InvalidOperationException(
                        $"Kode {request.AvdelingType.Kode} er allerede i bruk. Vennligst prøv med en annen kode.");

                var avdelingtype = new Domene.Sted.AvdelingType()
                {
                    Kode = request.AvdelingType.Kode,
                    Navn = request.AvdelingType.Navn
                };

                _context.AvdelingType.Add(avdelingtype);
                _context.SaveChanges();

                return _mapper.Map<AvdelingType>(avdelingtype);
            }
        }
    }
}