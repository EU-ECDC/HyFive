using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Observation.ProtectiveEquipment;
using HyFive.Models.V1.Constants;
using HyFive.Services.Authentication.User;
using HyFive.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        protected async Task ValidateObserverAccessAsync(Domain.Session.Session session, string email, CancellationToken cancellationToken)
        {
            var normalized = email?.Trim();

            if (session.Observer == null
                || session.Observer.IsDeactivated
                || string.IsNullOrWhiteSpace(session.Observer.Email)
                || !string.Equals(session.Observer.Email.Trim(), normalized, StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException("SessionNotLinkedToUser", session.Id, email);
            }

            var facilityId = await PermissionHelper.GetFacilityIdForUnitAsync(
                _context,
                session.OrganisationUnitId,
                cancellationToken);

            if (!facilityId.HasValue)
                throw new DomainException("FacilityNotFoundForUnit", session.OrganisationUnitId);

            var hasAccessToOu = await PermissionHelper.HasObserverPermissionForFacilityAsync(
                _context,
                session.ObserverId!.Value,
                facilityId.Value,
                cancellationToken);

            if (!hasAccessToOu)
                throw new DomainException("FacilityAccessDenied");
        }

        protected async Task<(TSession session, Domain.Place.OrganisationUnit unit)?> InitializeSessionAsync<TModel, TSession>(
            TModel requestSession,
            string email,
            int facilityId,
            int unitId,
            IUserService userService,
            ILogger logger,
            CancellationToken cancellationToken)
            where TSession : Domain.Session.Session
        {
            var unit = await SessionHelper.GetOrganisationUnitAsync(_context, unitId, cancellationToken);
            if (unit == null)
            {
                logger.LogWarning("Did not find unit with ID: {UnitId}", unitId);
                return null;
            }

            var observer = await SessionHelper.GetObserverAsync(
                _context,
                userService,
                email,
                facilityId,
                cancellationToken);

            if (observer == null)
                throw new DomainException("ObserverNotFoundAtOrganisationUnit", email, facilityId);

            var session = _mapper.Map<TSession>(requestSession);
            session.CreatedDate = DateTime.UtcNow;
            session.StartDate = DateTime.UtcNow;
            session.OrganisationUnitId = unit.Id;
            session.OrganisationUnit = unit;
            session.Observer = observer;

            return (session, unit);
        }

        protected static List<Domain.Observation.Role> GetDepartmentRoles(Domain.Place.OrganisationUnit unit)
        {
            var department = unit.Parent;

            return department?.OrganisationUnitRoles?
                .Select(our => our.Role)
                .Where(role => role != null)
                .GroupBy(role => role.Id)
                .Select(g => g.First())
                .ToList()
                ?? new List<Domain.Observation.Role>();
        }

        protected async Task<Domain.Session.TransferStatusType> GetTransferredToCoordinatorStatusAsync(CancellationToken cancellationToken)
        {
            var transferStatuses = await _context.TransferStatusType.ToListAsync(cancellationToken);
            return transferStatuses.First(o => o.Code == TransferStatusTypeConstants.TransferredToCoordinator);
        }

        protected static async Task<bool> IsAncestorAsync(
            HandHygieneContext context,
            int ancestorId,
            int nodeId,
            CancellationToken cancellationToken)
        {
            if (ancestorId == nodeId)
                return true;

            var currentParentId = await context.OrganisationUnit
                .AsNoTracking()
                .Where(x => x.Id == nodeId)
                .Select(x => x.ParentId)
                .FirstOrDefaultAsync(cancellationToken);

            var safety = 0;
            while (currentParentId.HasValue && safety++ < 50)
            {
                if (currentParentId.Value == ancestorId)
                    return true;

                currentParentId = await context.OrganisationUnit
                    .AsNoTracking()
                    .Where(x => x.Id == currentParentId.Value)
                    .Select(x => x.ParentId)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            return false;
        }

    }
}
