using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Models.V1.Observation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.HandJewelry
{
    public class UpdateHandJewelryType
    {
        public class Command : IRequest<HandJewelryType>
        {
            public HandJewelryType HandJewelryType { get; set; }
        }

        public class Handler : IRequestHandler<Command, HandJewelryType>
        {
            private readonly HandHygieneContext _context;
            private readonly IMapper _mapper;

            public Handler(HandHygieneContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }


            public async Task<HandJewelryType> Handle(Command command, CancellationToken cancellationToken)
            {
                var handJewelryType = await _context.HandJewelryType
                    .FirstOrDefaultAsync(i => i.Id == command.HandJewelryType.Id, cancellationToken);

                if (handJewelryType == null) throw new DomainException("HandJewelryTypeNotFound", command.HandJewelryType.Id);

                handJewelryType.Name = command.HandJewelryType.Name;

                _context.HandJewelryType.Update(handJewelryType);

                await _context.SaveChangesAsync(cancellationToken);

                var mapped = _mapper.Map<HandJewelryType>(handJewelryType);
                return mapped;
            }
        }
    }
}