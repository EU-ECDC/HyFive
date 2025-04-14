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
    public class OppdaterAvdelingType
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
                var avdelingtype = await _context.SectionType.FirstOrDefaultAsync(a => a.Id == request.AvdelingType.Id);
                if (avdelingtype != default(Domene.Place.SectionType))
                {
                    avdelingtype.Name = request.AvdelingType.Name;

                    _context.Update(avdelingtype);
                    await _context.SaveChangesAsync();

                    return _mapper.Map<Models.V1.Institution.DepartmentType>(avdelingtype);
                }

                throw new ArgumentException($"Kunne ikke finne avdelingtype med id {request.AvdelingType.Id}");
            }
        }
    }
}