using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Models.V1.Institution;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Services.Institusjon
{
    public class OppdaterInstitusjonstype
    {
        public class Command : IRequest<InstitutionType>
        {
            public InstitutionType Institusjonstype { get; set; }
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
                var institusjonstype = await _context.InstitutionType
                    .FirstOrDefaultAsync(i => i.Id == request.Institusjonstype.Id, cancellationToken);

                institusjonstype.Name = request.Institusjonstype.Name;

                _context.InstitutionType.Update(institusjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedInstitusjonstype =
                    _mapper.Map<Domene.Place.InstitutionType, InstitutionType>(institusjonstype);

                return mappedInstitusjonstype;
            }
        }
    }
}