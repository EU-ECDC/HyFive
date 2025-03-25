using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.Dataaksess;
using HyFive.Modeller.V1.Institusjon;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Institusjon
{
    public class OppdaterInstitusjonstype
    {
        public class Command : IRequest<InstitusjonType>
        {
            public InstitusjonType Institusjonstype { get; set; }
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
                var institusjonstype = await _context.InstitusjonType
                    .FirstOrDefaultAsync(i => i.Id == request.Institusjonstype.Id, cancellationToken);

                institusjonstype.Navn = request.Institusjonstype.Navn;

                _context.InstitusjonType.Update(institusjonstype);
                await _context.SaveChangesAsync(cancellationToken);

                var mappedInstitusjonstype =
                    _mapper.Map<Domene.Sted.InstitusjonType, InstitusjonType>(institusjonstype);

                return mappedInstitusjonstype;
            }
        }
    }
}