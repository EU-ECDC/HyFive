using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Facility;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Department
{
    public class CreateDepartmentType
    {
        public class Command : IRequest<DepartmentType>
        {
            public DepartmentType DepartmentType { get; set; }
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
                var exists = await _context.DepartmentType.AnyAsync(r => r.Code == request.DepartmentType.Code);
                if (exists)
                    throw new InvalidOperationException(
                        $"Code {request.DepartmentType.Code} is already in use. Please try with another code.");

                var avdelingtype = new Domain.Place.DepartmentType()
                {
                    Code = request.DepartmentType.Code,
                    Name = request.DepartmentType.Name
                };

                _context.DepartmentType.Add(avdelingtype);
                await _context.SaveChangesAsync(cancellationToken);

                return _mapper.Map<DepartmentType>(avdelingtype);
            }
        }
    }
}