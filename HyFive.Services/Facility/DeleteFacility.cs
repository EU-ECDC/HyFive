using AutoMapper;
using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.Session;
using HyFive.Models.V1.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyFive.Services.Facility
{
    public class DeleteFacility
    {
        public class Command : IRequest<bool>
        {
            public int FacilityId { get; set; }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly HandHygieneContext _context;

            public Handler(HandHygieneContext context)
            {
                _context = context;
            }


            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                var facility = await GetFacility(command.FacilityId, cancellationToken);

                // 1) Get all Unit ids under this facility (including facility)
                var orgUnitIds = await GetDescendantOrganisationUnitIds(facility.Id, cancellationToken);

                // Capture candidate users BEFORE deleting OUs/permissions
                var candidateUserIds = await GetUserIdsForFacilityTree(orgUnitIds, cancellationToken);

                // 2) Delete all sessions/observations for all these organisation units
                // (Session -> Unit is Restrict, so this is REQUIRED)
                foreach (var orgUnitId in orgUnitIds)
                {
                    DeleteFiveIndicationsSessionsAndObservations(orgUnitId);
                    DeleteHandJewelrySessionsAndObservations(orgUnitId);
                    DeleteGloveSessionsAndObservations(orgUnitId);
                    DeleteProtectiveEquipmentSessionsAndObservations(orgUnitId);
                }

                // 3) PredefinedComment -> Unit is Cascade in the model
                // so you can skip this. Keeping it explicit is OK though.
                DeletePredefinedComments(orgUnitIds);

                // 4) Delete organisation unit tree (children first because Parent->Children is Restrict)
                await DeleteOrganisationUnitsBottomUp(orgUnitIds, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await DeleteOrphanUsers(candidateUserIds, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }

            private async Task<Domain.Place.OrganisationUnit> GetFacility(int facilityId, CancellationToken cancellationToken)
            {
                var facility = await _context.Set<Domain.Place.OrganisationUnit>()
                    .Include(x => x.LevelRef)
                    .FirstOrDefaultAsync(x => x.Id == facilityId, cancellationToken);

                if (facility == null)
                {
                    throw new DomainException("FacilityNotFound", facilityId);
                }

                if (facility.LevelRef?.Level != OrganisationUnitLevels.Facility || facility.ParentId != null)
                    throw new DomainException("OrganisationUnitIsNotFacility", facilityId);

                return facility;
            }
          
            //Returns all descendant OU ids including the root facility id.
            private async Task<List<int>> GetDescendantOrganisationUnitIds(int rootId, CancellationToken cancellationToken)
            {
                var all = new HashSet<int> { rootId };
                var frontier = new List<int> { rootId };

                while (frontier.Count > 0)
                {
                    var children = await _context.Set<Domain.Place.OrganisationUnit>()
                        .AsNoTracking()
                        .Where(x => x.ParentId != null && frontier.Contains(x.ParentId.Value))
                        .Select(x => x.Id)
                        .ToListAsync(cancellationToken);

                    frontier = children.Where(id => all.Add(id)).ToList();
                }

                return all.ToList();
            }

            //Deletes OUs in correct order (deepest children first) because Parent->Children is Restrict.
            private async Task DeleteOrganisationUnitsBottomUp(List<int> organisationUnitIds, CancellationToken ct)
            {
                // Load the nodes we want to delete with their ParentId so we can order by depth.
                var nodes = await _context.OrganisationUnit
                    .Where(x => organisationUnitIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.ParentId })
                    .ToListAsync(ct);

                // Compute depth by walking parents inside the selected set
                var parentMap = nodes.ToDictionary(x => x.Id, x => x.ParentId);

                int Depth(int id)
                {
                    var depth = 0;
                    var current = id;
                    var seen = new HashSet<int>();

                    while (parentMap.TryGetValue(current, out var parentId) && parentId.HasValue)
                    {
                        if (!seen.Add(current)) break; // safety against cycles (shouldn't happen)
                        depth++;
                        current = parentId.Value;
                        if (!parentMap.ContainsKey(current)) break;
                    }

                    return depth;
                }

                var orderedIds = nodes
                    .OrderByDescending(x => Depth(x.Id))
                    .Select(x => x.Id)
                    .ToList();

                // Load entities and delete in that order
                var entities = await _context.OrganisationUnit
                    .Where(x => orderedIds.Contains(x.Id))
                    .ToListAsync(ct);

                // Ensure deletion order is respected
                foreach (var id in orderedIds)
                {
                    var entity = entities.First(e => e.Id == id);
                    _context.Remove(entity);
                }
            }

            private void DeletePredefinedComments(IEnumerable<int> organisationUnitIds)
            {
                var comments = _context.PredefinedComment
                    .Where(p => organisationUnitIds.Contains(p.OrganisationUnitId));

                _context.RemoveRange(comments);
            }

            private void DeleteFiveIndicationsSessionsAndObservations(int organisationUnitId)
            {
                var sessionsForOrgUnit = _context.Session.OfType<FiveIndicationsSession>()
                .Include(s => s.Observations)
                .ThenInclude(o => o.Activity)
                .Where(s => s.OrganisationUnitId == organisationUnitId)
                .ToList();

                foreach (var session in sessionsForOrgUnit)
                {
                    var activities = session.Observations.Select(o => o.Activity).ToList();
                    _context.Activity.RemoveRange(activities);
                    _context.FiveIndicationsObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteHandJewelrySessionsAndObservations(int organisationUnitId)
            {
                var sessionsForOrgUnit = _context.Session.OfType<HandJewelrySession>()
                    .Include(s => s.Observations)
                    .Where(s => s.OrganisationUnitId == organisationUnitId)
                    .ToList();

                foreach (var session in sessionsForOrgUnit)
                {
                    _context.HandJewelryObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteGloveSessionsAndObservations(int organisationUnitId)
            {
                var sessionsForOrgUnit = _context.Session.OfType<GloveSession>()
                    .Include(s => s.Observations)
                    .Where(s => s.OrganisationUnitId == organisationUnitId)
                    .ToList();

                foreach (var session in sessionsForOrgUnit)
                {
                    _context.GloveObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private void DeleteProtectiveEquipmentSessionsAndObservations(int organisationUnitId)
            {
                var sessionsForOrgUnit = _context.Session.OfType<ProtectiveEquipmentSession>()
                .Include(s => s.Observations)
                .ThenInclude(o => o.ProtectiveEquipmentList)
                .Where(s => s.OrganisationUnitId == organisationUnitId)
                .ToList();

                foreach (var session in sessionsForOrgUnit)
                {
                    var protectiveEquipmentList = session.Observations.SelectMany(o => o.ProtectiveEquipmentList).ToList();
                    _context.RemoveRange(protectiveEquipmentList);
                    _context.ProtectiveEquipmentObservation.RemoveRange(session.Observations);
                    _context.Session.Remove(session);
                }
            }

            private async Task<List<int>> GetUserIdsForFacilityTree(List<int> organisationUnitIds, CancellationToken ct)
            {
                return await _context.UserPermission
                .AsNoTracking()
                .Where(up => up.OrganisationUnitId.HasValue
                             && organisationUnitIds.Contains(up.OrganisationUnitId.Value))
                .Select(up => up.UserId)
                .Distinct()
                .ToListAsync(ct);
            }

            private async Task DeleteOrphanUsers(List<int> candidateUserIds, CancellationToken ct)
            {
                if (!candidateUserIds.Any())
                    return;

                // Users that still have at least one permission anywhere
                var stillReferencedUserIds = await _context.UserPermission
                    .AsNoTracking()
                    .Where(up => candidateUserIds.Contains(up.UserId))
                    .Select(up => up.UserId)
                    .Distinct()
                    .ToListAsync(ct);

                var orphanUserIds = candidateUserIds.Except(stillReferencedUserIds).ToList();
                if (!orphanUserIds.Any())
                    return;

                // Delete identifiers first (cascade exists, but explicit is fine)
                var identifiers = _context.UserIdentifier
                    .Where(ui => orphanUserIds.Contains(ui.UserId));
                
                _context.RemoveRange(identifiers);

                var users = await _context.User
                    .Where(u => orphanUserIds.Contains(u.Id))
                    .ToListAsync(ct);

                _context.RemoveRange(users);
            }
        }
    }
}
