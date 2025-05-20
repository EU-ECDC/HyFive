using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Institution
{
    public class UpdateInstitutionType
    {
        public class Command : IRequest<InstitutionType>
        {
            public InstitutionType InstitutionType { get; set; }
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
                var institutionType = await _context.InstitutionType
                    .FirstOrDefaultAsync(i => i.Id == request.InstitutionType.Id, cancellationToken);

                institutionType.Name = request.InstitutionType.Name;

                _context.InstitutionType.Update(institutionType);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedInstitutionType =
                    _mapper.Map<Domain.Place.InstitutionType, InstitutionType>(institutionType);

                return mappedInstitutionType;
            }
        }
    }
}