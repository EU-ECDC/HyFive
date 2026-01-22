using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation.ProtectiveEquipment;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Common
{
    public abstract class BaseHandler
    {
        protected readonly HandHygieneContext _context;
        protected readonly IMapper _mapper;

        protected BaseHandler(HandHygieneContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Loads a ProtectiveEquipmentType and its related MisuseTypes by ID.
        /// Throws if not found.
        /// </summary>
        protected async Task<ProtectiveEquipmentType> GetEquipmentTypeWithMisuseTypesAsync(
            int equipmentTypeId,
            CancellationToken cancellationToken)
        {
            var equipmentType = await _context.ProtectiveEquipmentType
                .Include(e => e.MisuseTypes)
                .FirstOrDefaultAsync(e => e.Id == equipmentTypeId, cancellationToken);

            if (equipmentType == null)
            {
                throw new DomainException("EquipmentTypeNotFound", equipmentTypeId);

            }

            return equipmentType;
        }
    }
}
