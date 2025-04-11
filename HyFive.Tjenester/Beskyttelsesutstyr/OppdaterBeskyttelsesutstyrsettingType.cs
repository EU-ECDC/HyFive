using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HyFive.DataAccess;
using HyFive.Modeller.V1.Observasjon.Beskyttelsesutstyr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HyFive.Tjenester.Beskyttelsesutstyr
{
    public class OppdaterBeskyttelsesutstyrsettingType
    {
        public class Command : IRequest<ProtectiveEquipmentSettingType>
        {
            public ProtectiveEquipmentSettingType SettingType { get; set; }
        }

        public class Handler : IRequestHandler<Command, ProtectiveEquipmentSettingType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ProtectiveEquipmentSettingType> Handle(Command request, CancellationToken cancellationToken)
            {
                var settingType = await _context.ProtectiveEquipmentSettingType
                    .FirstOrDefaultAsync(bust => bust.Id == request.SettingType.Id, cancellationToken);

                if (settingType == null) throw new Exception($"Fant ikke beskyttelsesutstyrsettingType med id {request.SettingType.Id}");

                settingType.Name = request.SettingType.Name;

                _context.ProtectiveEquipmentSettingType.Update(settingType);
                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<ProtectiveEquipmentSettingType>(settingType);
                return mapped;
            }
        }
    }
}