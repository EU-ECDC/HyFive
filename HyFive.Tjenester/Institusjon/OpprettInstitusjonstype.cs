using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class OpprettInstitusjonstype
    {
        public class Command : IRequest<InstitutionType>
        {
            public CreateInstitutionTypeRequest Institusjonstype { get; set; }
        }

        public class Handler : IRequestHandler<Command, InstitutionType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<InstitutionType> Handle(Command request, CancellationToken cancellationToken)
            {
                var exists = await _context.InstitutionType.AnyAsync(r => r.Code == request.Institusjonstype.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"Kode {request.Institusjonstype.Code} er allerede i bruk. Vennligst prøv med en annen kode.");

                var institusjonstype = new Domene.Place.InstitutionType
                {
                    Code = request.Institusjonstype.Code,
                    Name = request.Institusjonstype.Name
                };

                _context.InstitutionType.Add(institusjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<InstitutionType>(institusjonstype);
                return mapped;
            }
        }
    }
}