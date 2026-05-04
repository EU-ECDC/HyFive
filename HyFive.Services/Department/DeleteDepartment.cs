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

namespace HyFive.Services.Department
{
    public class DeleteDepartment
    {
        public class Command : IRequest<bool>
        {
            public int DepartmentId { get; set; }
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
                var departmentOrgUnit = await GetDepartmentOrgUnit(command.DepartmentId, cancellationToken);

                // 1) Get full subtree ids (department + all descendants like units, etc.)
                var orgUnitIds = await GetDescendantOrganisationUnitIds(departmentOrgUnit.Id, cancellationToken);

                // 2) Delete sessions/observations for all these OUs (Session -> OU is Restrict)
                foreach (var orgUnitId in orgUnitIds)
                {
                    DeleteFiveIndicationsSessionsAndObservations(orgUnitId);
                    DeleteHandJewelrySessionsAndObservations(orgUnitId);
                    DeleteGloveSessionsAndObservations(orgUnitId);
                    DeleteProtectiveEquipmentSessionsAndObservations(orgUnitId);
                }
                // 3) Delete OUs bottom-up because Parent->Children is Restrict
                await DeleteOrganisationUnitsBottomUp(orgUnitIds, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            private async Task<Domain.Place.OrganisationUnit> GetDepartmentOrgUnit(int departmentId, CancellationToken ct)
            {
                var ou = await _context.OrganisationUnit
                .Include(x => x.LevelRef)
                .FirstOrDefaultAsync(x => x.Id == departmentId, ct);

                if (ou == null)
                    throw new DomainException("DepartmentNotFound", departmentId);

                if (ou.LevelRef?.Level != OrganisationUnitLevels.Department)
                    throw new DomainException("OrganisationUnitIsNotDepartment", departmentId);

                return ou;
            }

            // BFS over Unit.ParentId to get full subtree ids (includes root).
            private async Task<List<int>> GetDescendantOrganisationUnitIds(int rootId, CancellationToken ct)
            {
                var all = new HashSet<int> { rootId };
                var frontier = new List<int> { rootId };

                while (frontier.Count > 0)
                {
                    var children = await _context.OrganisationUnit
                        .AsNoTracking()
                        .Where(x => x.ParentId != null && frontier.Contains(x.ParentId.Value))
                        .Select(x => x.Id)
                        .ToListAsync(ct);

                    frontier = children.Where(id => all.Add(id)).ToList();
                }

                return all.ToList();
            }

            // Deletes OrganisationUnits in depth-desc order (children first) to satisfy Restrict FK.
            private async Task DeleteOrganisationUnitsBottomUp(List<int> organisationUnitIds, CancellationToken ct)
            {
                var nodes = await _context.OrganisationUnit
                .AsNoTracking()
                .Where(x => organisationUnitIds.Contains(x.Id))
                .Select(x => new { x.Id, x.ParentId })
                .ToListAsync(ct);

                var parentMap = nodes.ToDictionary(x => x.Id, x => x.ParentId);

                int Depth(int id)
                {
                    var depth = 0;
                    var current = id;
                    var seen = new HashSet<int>();

                    while (parentMap.TryGetValue(current, out var parentId) && parentId.HasValue)
                    {
                        if (!seen.Add(current)) break; // safety
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

                // Load tracked entities and remove in the required order
                var entities = await _context.OrganisationUnit
                    .Where(x => orderedIds.Contains(x.Id))
                    .ToListAsync(ct);

                foreach (var id in orderedIds)
                {
                    var entity = entities.First(e => e.Id == id);
                    _context.OrganisationUnit.Remove(entity);
                }
            }

            private void DeleteFiveIndicationsSessionsAndObservations(int departmentId)
            {
                DeleteSessionsAndObservations<FiveIndicationsSession>(
                   departmentId,
                   query => query
                       .Include(s => s.Observations)
                       .ThenInclude(o => o.Activity),
                   session =>
                   {
                       var activities = session.Observations.Select(o => o.Activity).ToList();
                       _context.Activity.RemoveRange(activities);
                       _context.FiveIndicationsObservation.RemoveRange(session.Observations);
                   });
            }

            private void DeleteHandJewelrySessionsAndObservations(int departmentId)
            {
                DeleteSessionsAndObservations<HandJewelrySession>(
                departmentId,
                query => query.Include(s => s.Observations),
                session =>
                {
                    _context.HandJewelryObservation.RemoveRange(session.Observations);
                });
            }

            private void DeleteGloveSessionsAndObservations(int departmentId)
            {
                DeleteSessionsAndObservations<GloveSession>(
                departmentId,
                query => query.Include(s => s.Observations),
                session =>
                {
                    _context.GloveObservation.RemoveRange(session.Observations);
                });
            }

            private void DeleteProtectiveEquipmentSessionsAndObservations(int departmentId)
            {
                DeleteSessionsAndObservations<ProtectiveEquipmentSession>(
                departmentId,
                query => query
                    .Include(s => s.Observations)
                    .ThenInclude(o => o.ProtectiveEquipmentList),
                session =>
                {
                    var protectiveEquipmentList = session.Observations
                        .SelectMany(o => o.ProtectiveEquipmentList)
                        .ToList();

                    _context.RemoveRange(protectiveEquipmentList);
                    _context.ProtectiveEquipmentObservation.RemoveRange(session.Observations);
                });
            }

            private void DeleteSessionsAndObservations<TSession>(
                int organisationUnitId,
                Func<IQueryable<TSession>, IQueryable<TSession>> include,
                Action<TSession> deleteChildren)
                where TSession : Domain.Session.Session
                        {
                            var sessions = include(
                                    _context.Session.OfType<TSession>()
                                        .Where(s => s.OrganisationUnitId == organisationUnitId))
                                .ToList();

                foreach (var session in sessions)
                {
                    deleteChildren(session);
                    _context.Session.Remove(session);
                }
            }

        }
    }
}