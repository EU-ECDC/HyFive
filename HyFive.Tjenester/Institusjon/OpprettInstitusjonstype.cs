using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class OpprettInstitusjonstype
    {
        public class Command : IRequest<InstitusjonType>
        {
            public OpprettInstitusjonstypeRequest Institusjonstype { get; set; }
        }

        public class Handler : IRequestHandler<Command, InstitusjonType>
        {
            private readonly HandhygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandhygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<InstitusjonType> Handle(Command request, CancellationToken cancellationToken)
            {
                var exists = await _context.InstitusjonType.AnyAsync(r => r.Kode == request.Institusjonstype.Kode);
                if (exists)
                    throw new InvalidOperationException(
                        $"Kode {request.Institusjonstype.Kode} er allerede i bruk. Vennligst prøv med en annen kode.");

                var institusjonstype = new Domene.Sted.InstitusjonType
                {
                    Kode = request.Institusjonstype.Kode,
                    Navn = request.Institusjonstype.Navn
                };

                _context.InstitusjonType.Add(institusjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<InstitusjonType>(institusjonstype);
                return mapped;
            }
        }
    }
}