using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Institution
{
    public class CreateInstitutionType
    {
        public class Command : IRequest<InstitutionType>
        {
            public CreateInstitutionTypeRequest InstitutionType { get; set; }
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
                var exists = await _context.InstitutionType.AnyAsync(r => r.Code == request.InstitutionType.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"\"Code {{request.InstitutionType.Code}} is already in use. Please try with a different code.");

                var institusjonstype = new Domain.Place.InstitutionType
                {
                    Code = request.InstitutionType.Code,
                    Name = request.InstitutionType.Name
                };

                _context.InstitutionType.Add(institusjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<InstitutionType>(institusjonstype);
                return mapped;
            }
        }
    }
}