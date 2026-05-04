using HyFive.DataAccess;
using HyFive.Domain.Exceptions;
using HyFive.Domain.User;
using HyFive.Models.V1.Authentication;
using HyFive.Models.V1.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ObserverUser = HyFive.Domain.User.User;

namespace HyFive.Services.Authentication.User
{
    public class UserService : IUserService
    {
        private const string HashSalt = "handhygiene";
        private const string FirstNameInClaims = "given_name";
        private const string LastNameInClaims = "family_name";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private readonly HandHygieneContext _context;


        public UserService(IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger,
            HandHygieneContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _context = context;
        }

        public async Task<LoggedInUser> GetUser()
        {
            var email = _httpContextAccessor.HttpContext?.User
            ?.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var dbUser = await _context.User
                .FirstOrDefaultAsync(u => u.Email == email);

            if (dbUser == null)
                return null;

            var user = new LoggedInUser()
            {
                Name = $"{dbUser.FirstName} {dbUser.LastName}"
            };
            
            var logInName = user?.Name;

            user.Id = CreateHash(email + user.Name + HashSalt);
            user.IsObserver = await IsObserver(email);
            user.IsCoordinator = await IsCoordinator(email);
            user.IsAdmin = await IsAdmin(email);
            var dbUserId = await _context.User.AsNoTracking()
                .Where(HasEmailAndIsActive(email))
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (dbUserId == 0)
            {
                user.OrganisationUnits = new List<OrganisationAccessInfo>();
                return user;
            }
            user.OrganisationUnits = await _context.UserPermission.AsNoTracking()
                .Where(p => p.UserId == dbUserId)
                .Join(
                    _context.OrganisationUnit.AsNoTracking(),
                    p => p.OrganisationUnitId,
                    ou => ou.Id,
                    (p, ou) => new OrganisationAccessInfo
                    {
                        OrganisationUnitId = ou.Id,
                        PermissionLevel = p.PermissionLevel,
                        LevelId = ou.LevelId,
                        Level = ou.LevelRef.Level,
                        Name = ou.Name
                    })
                .ToListAsync();

            return user;
        }

        public bool IsUserLoggedIn()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true;
        }


        public Task<bool> IsCoordinator(string email)
            => IsRole(email, PermissionLevelConstants.Coordinator);

        public Task<bool> IsObserver(string email)
            => IsRole(email, PermissionLevelConstants.Observer);

        public Task<bool> IsCoordinatorForFacility(int facilityOrgUnitId, string email)
            => IsRoleForFacility(email, facilityOrgUnitId, PermissionLevelConstants.Coordinator);

        public Task<bool> IsCoordinatorForFacility(int facilityOrgUnitId)
            => IsRoleForFacility(GetEmail(), facilityOrgUnitId, PermissionLevelConstants.Coordinator);

        public Task<bool> IsCoordinatorForCity(string city)
            => IsCoordinatorForCity(GetEmail(), city, PermissionLevelConstants.Coordinator);

        public Task<bool> IsCoordinatorForFacilities(List<int> facilityOrgUnitIds)
            => IsRoleForFacilities(facilityOrgUnitIds, PermissionLevelConstants.Coordinator);

        public Task<bool> IsCoordinatorForDepartment(int departmentOrgUnitId)
            => IsRoleForDepartment(departmentOrgUnitId, PermissionLevelConstants.Coordinator);

        public Task<bool> IsCoordinatorForUnit(int unitOrgUnitId)
            => IsRoleForUnit(unitOrgUnitId, PermissionLevelConstants.Coordinator);

        public Task<bool> IsCoordinatorForUser(int userID)
            => IsRoleForUser(GetEmail(), userID, PermissionLevelConstants.Coordinator);

        public Task<bool> IsObserverForFacility(string email, int facilityOrgUnitId)
            => IsRoleForFacility(email, facilityOrgUnitId, PermissionLevelConstants.Observer);

        public async Task<bool> IsAdminOrCoordinator(string email)
        {
            return await IsAdmin(email) || await IsCoordinator(email);
        }

        public async Task<bool> IsAdmin(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            bool isAdmin = await _context.User
            .AsNoTracking()
            .Where(HasEmailAndIsActive(email))
            .AnyAsync(u => u.UserPermissions.Any(p =>
                p.PermissionLevel == PermissionLevelConstants.Administrator));
            return isAdmin;
        }

        public async Task<bool> IsAdmin()
            => await IsAdmin(GetEmail());

        public async Task<bool> IsCoordinatorForDepartmentOrAdmin(int departmentOrgUnitId)
        {
            if (await IsAdmin())
                return true;

            return await IsCoordinatorForDepartment(departmentOrgUnitId);
        }

        public Task<bool> IsObserverForFacility(int facilityOrgUnitId)
        {
            return IsObserverForFacility(GetEmail(), facilityOrgUnitId);
        }

        public async Task<bool> IsCoordinatorForFacilityOrAdmin(int facilityOrgUnitId)
        {
            if (await IsAdmin())
                return true;

            return await IsCoordinatorForFacility(facilityOrgUnitId);
        }

        public async Task<bool> IsCoordinatorForFacilitiesOrAdmin(List<int>? facilityOrgUnitIds)
        {
            if (await IsAdmin())
                return true;

            return await IsCoordinatorForFacilities(facilityOrgUnitIds);
        }

        public async Task<bool> IsCoordinatorForCityOrAdmin(string city)
        {
            if (await IsAdmin())
                return true;

            return await IsCoordinatorForCity(city);
        }

        public string GetEmail()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            return email;
        }

        public async Task<bool> IsCoordinatorForSession(string sessionId)
        {
            if (!Guid.TryParse(sessionId, out var guidSessionId))
                throw new ArgumentException(
                    $"HealthIdUserService: Error parsing ID: {sessionId}. {sessionId} must be of type Guid");

            // 1) Get the session's FacilityId
            var sessionOrgUnitId = await _context.Session.AsNoTracking()
                .Where(s => s.Id == guidSessionId)
                .Select(s => s.OrganisationUnitId)
                .FirstOrDefaultAsync();

            if (sessionOrgUnitId == 0)
                return false;

            // 2) Resolve the root Unit id from the session org unit (Unit is always root)
            var facilityIdQuery =
                // Case 1: org unit is already a Unit (root)
                (from ou in _context.OrganisationUnit.AsNoTracking()
                 where ou.Id == sessionOrgUnitId && ou.ParentId == null
                 select ou.Id)

                .Union(
                // Case 2: org unit is a Department (parent is root)
                from ou in _context.OrganisationUnit.AsNoTracking()
                join parent in _context.OrganisationUnit.AsNoTracking() on ou.ParentId equals parent.Id
                where ou.Id == sessionOrgUnitId && parent.ParentId == null
                select parent.Id)

                .Union(
                // Case 3: org unit is a Unit (grandparent is root)
                from ou in _context.OrganisationUnit.AsNoTracking()
                join parent in _context.OrganisationUnit.AsNoTracking() on ou.ParentId equals parent.Id
                join grandParent in _context.OrganisationUnit.AsNoTracking() on parent.ParentId equals grandParent.Id
                where ou.Id == sessionOrgUnitId && grandParent.ParentId == null
                select grandParent.Id);

            var facilityOrgUnitId = await facilityIdQuery.FirstOrDefaultAsync();

            if (facilityOrgUnitId == 0)
                return false;

            // 3) Check coordinator access for that facility
            return await IsRoleForFacility(GetEmail(), facilityOrgUnitId, PermissionLevelConstants.Coordinator);
        }

        public async Task<int> GetObserverIdIfHasAccessToFacility(int facilityOrgUnitId)
        {
            var email = GetEmail();
            // 1) resolve observer user id
            var observerId = await _context.User.AsNoTracking()
            .Where(HasEmailAndIsActive(email))
            .Where(u => u.UserPermissions.Any(p => p.PermissionLevel == PermissionLevelConstants.Observer))
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

            if (observerId == 0)
                return 0;

            // 2) check if observer has permission that rolls up to the facility
            var hasAccess = await
                (from p in _context.UserPermission.AsNoTracking()
                 where p.UserId == observerId

                 join ou in _context.OrganisationUnit.AsNoTracking()
                     on p.OrganisationUnitId equals ou.Id

                 join parent in _context.OrganisationUnit.AsNoTracking()
                     on ou.ParentId equals parent.Id into p1
                 from parent in p1.DefaultIfEmpty()

                 join grandParent in _context.OrganisationUnit.AsNoTracking()
                     on parent.ParentId equals grandParent.Id into p2
                 from grandParent in p2.DefaultIfEmpty()

                 where
                 // Case 1: permission directly on Unit
                 ou.Id == facilityOrgUnitId && ou.ParentId == null

                 // Case 2: permission on Department under that Unit
                 || (parent != null && parent.Id == facilityOrgUnitId && parent.ParentId == null)

                 // Case 3: permission on Unit under Department under that Unit
                 || (grandParent != null && grandParent.Id == facilityOrgUnitId && grandParent.ParentId == null)

                 select p.Id
                )
                .AnyAsync();

            return hasAccess ? observerId : 0;
        }

        public Expression<Func<Domain.User.User, bool>> HasEmailAndIsActive(string email)
        {
            var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalized))
                return _ => false; // fail closed if no email

            // Case-insensitive match via ToLower translation
            return b => !b.IsDeactivated
                     && b.Email != null
                     && b.Email.ToLower() == normalized;
        }

        private async Task<bool> IsRole(string email, string permissionLevel)
        {
            return await _context.User
               .AsNoTracking()
               .Where(HasEmailAndIsActive(email))
               .AnyAsync(u => u.UserPermissions.Any(p =>
                   p.PermissionLevel == permissionLevel));
        }

        private async Task<bool> IsRoleForFacility(string email, int facilityOrgUnitId, string level)
        {
            // 1) Find the DB user id for this role (Administrator/Coordinator/Observer etc.)
            var userId = await _context.User.AsNoTracking()
                .Where(HasEmailAndIsActive(email))
                .Where(u => u.UserPermissions.Any(p => p.PermissionLevel == level))
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0)
                return false;

            // 2) Check if the user has ANY permission that rolls up to the facility
            return await GetAccessibleFacilityIdsQuery(userId, level)
                .AnyAsync(id => id == facilityOrgUnitId);
        }

        private async Task<bool> IsCoordinatorForCity(string email, string city, string level)
        {
            // 1) Find Coordinator user id
            var coordinatorId = await _context.User.AsNoTracking()
                .Where(HasEmailAndIsActive(email))
                .Where(u => u.UserPermissions.Any(p => p.PermissionLevel == level))
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (coordinatorId == 0)
                return false;

            // 2) Find facilities (org units) the coordinator can access, and check facility address city
            return await
                (from p in _context.UserPermission.AsNoTracking()
                 where p.UserId == coordinatorId

                 join ou in _context.OrganisationUnit.AsNoTracking()
                     on p.OrganisationUnitId equals ou.Id

                 join parent in _context.OrganisationUnit.AsNoTracking()
                     on ou.ParentId equals parent.Id into p1
                 from parent in p1.DefaultIfEmpty()

                 join grandParent in _context.OrganisationUnit.AsNoTracking()
                     on parent.ParentId equals grandParent.Id into p2
                 from grandParent in p2.DefaultIfEmpty()

                     // Join address for each possible "root facility"
                 join addrOu in _context.Address.AsNoTracking()
                     on ou.AddressId equals addrOu.Id into a1
                 from addrOu in a1.DefaultIfEmpty()

                 join addrParent in _context.Address.AsNoTracking()
                     on parent.AddressId equals addrParent.Id into a2
                 from addrParent in a2.DefaultIfEmpty()

                 join addrGrand in _context.Address.AsNoTracking()
                     on grandParent.AddressId equals addrGrand.Id into a3
                 from addrGrand in a3.DefaultIfEmpty()

                 where
                 // Case 1: permission is directly on root Unit (ou is root)
                 (ou.ParentId == null
                     && addrOu != null
                     && EF.Functions.ILike(addrOu.City!, city))

                 // Case 2: permission on Department (parent is root Unit)
                 || (parent != null && parent.ParentId == null
                     && addrParent != null
                     && EF.Functions.ILike(addrParent.City!, city))

                 // Case 3: permission on Unit (grandParent is root Unit)
                 || (grandParent != null && grandParent.ParentId == null
                     && addrGrand != null
                     && EF.Functions.ILike(addrGrand.City!, city))

                 select p.Id
                )
                .AnyAsync();
        }

        private async Task<bool> IsRoleForFacilities(List<int> facilityIds, string level)
        {
            var email = GetEmail();
            // 1) Get DB user id for this role
            var userId = await _context.User.AsNoTracking()
                .Where(HasEmailAndIsActive(email))
                .Where(u => u.UserPermissions.Any(p => p.PermissionLevel == level))
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0)
                return false;

            // 2) Get all facility (root OU) ids the user can access
            var accessibleFacilityIds = await GetAccessibleFacilityIdsQuery(userId, level)
               .Distinct()
               .ToListAsync();

            // 3) Original behavior: if no facilityIds provided, just check any access exists
            if (facilityIds == null || facilityIds.Count == 0)
                return accessibleFacilityIds.Any();

            // 4) Must have access to ALL requested facilities
            return facilityIds.All(id => accessibleFacilityIds.Contains(id));
        }

        private async Task<bool> IsRoleForUser(string email, int targetUserId, string level)
        {
            // 1) Resolve the current user's DB id for the role
            var roleUserId = await _context.User.AsNoTracking()
                .Where(HasEmailAndIsActive(email))
                .Where(u => u.UserPermissions.Any(p => p.PermissionLevel == level))
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (roleUserId == 0)
                return false;

            // 2) Facilities (root Unit ids) the TARGET user can access
            var targetFacilityIds = await GetFacilityIdsForUser(targetUserId);
            if (targetFacilityIds.Count == 0)
                return false;

            // 3) Facilities (root Unit ids) the ROLE user can access
            var roleUserFacilityIds = await GetFacilityIdsForUser(roleUserId);
            if (roleUserFacilityIds.Count == 0)
                return false;

            // 4) Closest to old behavior: any overlap
            return targetFacilityIds.Any(id => roleUserFacilityIds.Contains(id));
        }

        /// <summary>
        /// Returns facility/root Unit ids for a user based on permissions at any level.
        /// Unit is always root (ParentId == null). Only 3 levels exist.
        /// No nested ternaries.
        /// </summary>
        private async Task<List<int>> GetFacilityIdsForUser(int userId)
        {
            return await
                (from p in _context.UserPermission.AsNoTracking()
                where p.UserId == userId
                join ou in _context.OrganisationUnit.AsNoTracking()
                    on p.OrganisationUnitId equals ou.Id
                where ou.ParentId == null
                select ou.Id)

            .Union(
                from p in _context.UserPermission.AsNoTracking()
                where p.UserId == userId
                join ou in _context.OrganisationUnit.AsNoTracking()
                    on p.OrganisationUnitId equals ou.Id
                join parent in _context.OrganisationUnit.AsNoTracking()
                    on ou.ParentId equals parent.Id
                where parent.ParentId == null
                select parent.Id)

            .Union(
                from p in _context.UserPermission.AsNoTracking()
                where p.UserId == userId
                join ou in _context.OrganisationUnit.AsNoTracking()
                    on p.OrganisationUnitId equals ou.Id
                join parent in _context.OrganisationUnit.AsNoTracking()
                    on ou.ParentId equals parent.Id
                join grandParent in _context.OrganisationUnit.AsNoTracking()
                    on parent.ParentId equals grandParent.Id
                where grandParent.ParentId == null
                select grandParent.Id)
            .Distinct()
            .ToListAsync();
        }

        private async Task<bool> IsRoleForDepartment(int departmentOrgUnitId, string level)
        {
            // Unit is root => Department.ParentId is the facility id
            var facilityOrgUnitId = await _context.OrganisationUnit.AsNoTracking()
                .Where(ou => ou.Id == departmentOrgUnitId)
                .Select(ou => ou.ParentId)
                .FirstOrDefaultAsync();

            if (facilityOrgUnitId == null)
                return false; // not found OR it's a root facility OR invalid tree

            return await IsRoleForFacility(GetEmail(), facilityOrgUnitId.Value, level);
        }

        private async Task<bool> IsRoleForUnit(int unitOrgUnitId, string level)
        {
            var facilityOrgUnitId = await _context.OrganisationUnit
            .AsNoTracking()
            .Where(ou => ou.Id == unitOrgUnitId)
            .Select(ou => new
            {
                FacilityId = ou.Parent.ParentId
            })
            .Select(x => x.FacilityId)
            .FirstOrDefaultAsync();

                if (!facilityOrgUnitId.HasValue)
                    return false;

            return await IsRoleForFacility(GetEmail(), facilityOrgUnitId.Value, level);
        }

        /// <summary>
        /// From https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha512?view=net-5.0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CreateHash(string str)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(str);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }


        public string GetPseudonym()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            return user?.IdentityPseudonym;
        }

        private string GetDiscriminator<T>() where T : class
            => typeof(T).Name;
       
        public string GetFirstName()
        {
            var firstName = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == FirstNameInClaims)?.Value;
            return firstName;
        }

        public string GetLastName()
        {
            var lastName = _httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == LastNameInClaims)?.Value;
            return lastName;
        }

        private IQueryable<int> GetAccessibleFacilityIdsQuery(int userId, string permissionLevel)
        {
            return
                (from p in _context.UserPermission.AsNoTracking()
                 where p.UserId == userId && p.PermissionLevel == permissionLevel
                 join ou in _context.OrganisationUnit.AsNoTracking()
                     on p.OrganisationUnitId equals ou.Id
                 where ou.ParentId == null
                 select ou.Id)

                .Union(
                 from p in _context.UserPermission.AsNoTracking()
                 where p.UserId == userId && p.PermissionLevel == permissionLevel
                 join ou in _context.OrganisationUnit.AsNoTracking()
                     on p.OrganisationUnitId equals ou.Id
                 join parent in _context.OrganisationUnit.AsNoTracking()
                     on ou.ParentId equals parent.Id
                 where parent.ParentId == null
                 select parent.Id)

                .Union(
                 from p in _context.UserPermission.AsNoTracking()
                 where p.UserId == userId && p.PermissionLevel == permissionLevel
                 join ou in _context.OrganisationUnit.AsNoTracking()
                     on p.OrganisationUnitId equals ou.Id
                 join parent in _context.OrganisationUnit.AsNoTracking()
                     on ou.ParentId equals parent.Id
                 join grandParent in _context.OrganisationUnit.AsNoTracking()
                     on parent.ParentId equals grandParent.Id
                 where grandParent.ParentId == null
                 select grandParent.Id);
        }
    }
}
