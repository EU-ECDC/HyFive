using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Tjenester.FireIndikasjoner
{
    public class OppdaterAktivitetType
    {
        public class Command : IRequest<ActivityType>
        {
            public ActivityType Aktivitettype { get; set; }
        }

        public class Handler : IRequestHandler<Command, ActivityType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<ActivityType> Handle(Command command, CancellationToken cancellationToken)
            {
                var aktivitettype = await _context.ActivityType
                    .FirstOrDefaultAsync(i => i.Id == command.Aktivitettype.Id, cancellationToken);

                if (aktivitettype == null) throw new Exception($"Fant ikke aktivitettype med id {command.Aktivitettype.Id}");

                aktivitettype.Name = command.Aktivitettype.Name;

                _context.ActivityType.Update(aktivitettype);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<ActivityType>(aktivitettype);
                return mapped;
            }
        }
    }
}