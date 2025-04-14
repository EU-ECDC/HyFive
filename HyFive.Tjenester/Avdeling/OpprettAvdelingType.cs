using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Avdeling
{
    public class OpprettAvdelingType
    {
        public class Command : IRequest<DepartmentType>
        {
            public DepartmentType AvdelingType { get; set; }
        }

        public class Handler : IRequestHandler<Command, DepartmentType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<DepartmentType> Handle(Command request, CancellationToken cancellationToken)
            {
                var exists = await _context.SectionType.AnyAsync(r => r.Code == request.AvdelingType.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"Kode {request.AvdelingType.Code} er allerede i bruk. Vennligst prøv med en annen kode.");

                var avdelingtype = new Domene.Place.SectionType()
                {
                    Code = request.AvdelingType.Code,
                    Name = request.AvdelingType.Name
                };

                _context.SectionType.Add(avdelingtype);
                _context.SaveChanges();

                return _mapper.Map<DepartmentType>(avdelingtype);
            }
        }
    }
}