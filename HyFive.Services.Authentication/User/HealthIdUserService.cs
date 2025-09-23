using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Authentication;
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
using Observer = HyFive.Domain.User.Observer;

namespace HyFive.Services.Authentication.User
{
    public class HealthIdUserService : IUserService
    {
        private const string HashSalt = "handhygiene";
        private const string FirstNameInClaims = "given_name";
        private const string LastNameInClaims = "family_name";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HealthIdUserService> _logger;
        private readonly HandHygieneContext _context;



        public HealthIdUserService(IHttpContextAccessor httpContextAccessor,
            ILogger<HealthIdUserService> logger,
            HandHygieneContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _context = context;
            if (_httpContextAccessor.HttpContext.User == null)
                _logger.LogInformation("TI03: User is null");
        }

        public async Task<LoggedInUser> GetUser()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            //var userNameClaim = _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.WindowsAccountName);
            //var loginNameClaim = _httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            var user = new LoggedInUser()
            {
                Name = userClaim?.FindFirst("sub")?.Value ?? "",
            };
            var logInName = userClaim.Identity?.Name ?? "(UserNull)";
            var email = GetEmail();

            user.Id = CreateHash(email + user.Name + HashSalt);
            user.IsObserver = IsObserver(email);
            user.IsCoordinator = IsCoordinator(email);
            user.IsFhiAdmin = IsAdmin(email);
            user.FacilityIds = await _context.User.AsNoTracking().Include(k => k.Facility)
                .Where(HasEmailAndIsActive<Domain.User.User>(email))
                .Where(k => k.Facility != null)
                .Select(k => k.Facility.Id)
                .ToListAsync();
            user.FirstName = GetFirstName();
            user.LastName = GetLastName();
            
            return user;
        }

        public bool IsUserLoggedIn()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true;
        }


        public bool IsCoordinator(string email)
            => IsRole<Coordinator>(email);

        public bool IsObserver(string email)
            => IsRole<Coordinator>(email);

        public bool IsCoordinatorForFacility(int facilityId, string email)
            => IsRoleForFacility<Coordinator>(email, facilityId);

        public bool IsCoordinatorForFacility(int facilityId)
            => IsRoleForFacility<Coordinator>(GetEmail(), facilityId);

        public bool IsCoordinatorForCity(int cityId)
            => IsCoordinatorForCity(GetEmail(), cityId);

        public bool IsCoordinatorForFacilities(List<int> facilityIds)
            => IsRoleForFacilities<Coordinator>(facilityIds);

        public bool IsCoordinatorForFacilitiesOrAdmin(List<int> facilityIds)
        {
            return IsCoordinatorForFacilities(facilityIds) || IsAdmin();
        }

        public bool IsCoordinatorForDepartment(int departmentId)
            => IsRoleForDepartment<Coordinator>(departmentId);

        public bool IsCoordinatorForUser(int userId)
            => IsRoleForUser<Coordinator>(GetEmail(), userId);

        public bool IsObserverForFacility(string email, int facilityId)
            => IsRoleForFacility<Observer>(email, facilityId);

        public bool IsAdminOrCoordinator(string email)
        {
            return IsAdmin(email) || IsCoordinator(email);
        }

        public bool IsAdmin(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            bool isFhiAdmin = _context.User.AsNoTracking().OfType<Admin>().AsNoTracking()
                    .Where(HasEmailAndIsActive<Admin>(email)).Any();
            return isFhiAdmin;
        }

        public bool IsAdmin()
            => IsAdmin(GetEmail());

        public bool IsCoordinatorForDepartmentOrAdmin(int departmentId)
        {
            return IsCoordinatorForDepartment(departmentId) || IsAdmin();
        }

        public bool IsObserverForFacility(int facilityId)
        {
            return IsObserverForFacility(GetEmail(), facilityId);
        }

        public bool IsCoordinatorForFacilityOrAdmin(int facilityId)
        {
            return IsCoordinatorForFacility(facilityId) || IsAdmin();
        }

        public bool IsCoordinatorForCityOrAdmin(int cityId)
        {
            return IsCoordinatorForCity(cityId) || IsAdmin();
        }

        public string GetHprNumber()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = _context.User.FirstOrDefault(u => u.Email == email);

            return user?.HPRNumber;
        }

        public string GetEmail()
        {
            var email = _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            return email;
        }

        public bool IsCoordinatorForSession(string sessionId)
        {
            var isGuid = Guid.TryParse(sessionId, out Guid guidSessionId);

            if (!isGuid)
            {
                throw new ArgumentException($"HealthIdUSerService: Error parsing ID: {sessionId}. {sessionId} must be of type Guid");
            }
            
            var facilityId = _context.Session.AsNoTracking().Include(s => s.Department).ThenInclude(a => a.Facility)
                .FirstOrDefault(s => s.Id == guidSessionId).Department?.FacilityId;
            if (facilityId != null)
            {
                return IsRoleForFacility<Coordinator>(GetEmail(), (int)facilityId);
            }

            return false;
        }

        public int GetObserverIdForFacility(int facilityId)
        {
            var email = GetEmail();
            return _context.User.AsNoTracking()
                .OfType<Observer>()
                .Include(o => o.Facility)
                .Where(HasEmailAndIsActive<Domain.User.Observer>(email)).First(o => o.Facility.Id == facilityId)?.Id ?? 0;
        }

        public Expression<Func<TUser, bool>> HasEmailAndIsActive<TUser>(string email) where TUser : Domain.User.User
        {
            var normalized = (email ?? string.Empty).Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalized))
                return _ => false; // fail closed if no email

            // Case-insensitive match via ToLower translation
            return b => !b.IsDeactivated
                     && b.Email != null
                     && b.Email.ToLower() == normalized;
        }

        private bool IsRole<TRole>(string email) where TRole : Domain.User.User
        {
            var isRole = _context.User.AsNoTracking().OfType<TRole>()
                .Include(r => r.Facility)
                .Any(HasEmailAndIsActive<TRole>(email));
            
            return isRole;
        }

        private bool IsRoleForFacility<TRole>(string email, int facilityId) where TRole : Domain.User.User
        {
            return _context.User.OfType<TRole>().AsNoTracking().Include(b => b.Facility)
                .Where(HasEmailAndIsActive<TRole>(email))
                .Any(b => b.Facility.Id == facilityId && b.Discriminator == GetDiscriminator<TRole>());
        }

        private bool IsCoordinatorForCity(string email, int city)
        {
            return _context.User.OfType<Coordinator>().AsNoTracking().Include(b => b.Facility).ThenInclude(i=>i.City)
                .Where(HasEmailAndIsActive<Coordinator>(email))
                .Any(b => b.Facility.City.Id == city);
        }

        private bool IsRoleForFacilities<TRole>(List<int> facilityIds) where TRole : Domain.User.User
        {
            var email = GetEmail();
            var CoordinatorForFacilityIds = _context
                .User
                .OfType<TRole>()
                .AsNoTracking()
                .Include(k => k.Facility)
                .Where(HasEmailAndIsActive<TRole>(email))
                .Where(k => k.Discriminator == GetDiscriminator<TRole>())
                .Select(k => k.Facility.Id).ToList();

            if (facilityIds == null)
            {
                return CoordinatorForFacilityIds.Any();
            }

            return CoordinatorForFacilityIds.Any() && facilityIds.ToList().TrueForAll(iid => CoordinatorForFacilityIds.Contains(iid));
        }

        private bool IsRoleForUser<TRole>(string email, int userId) where TRole : Domain.User.User
        {
            var facilityId = _context
                .User
                .AsNoTracking()
                .Include(b => b.Facility)
                .Where(b => b.Id == userId)
                .Select(b => b.Facility.Id).First();
            return IsRoleForFacility<TRole>(email, facilityId);
        }

        private bool IsRoleForSession<TRole>(string email, Guid sessionId) where TRole : Domain.User.User
        {
            var facilityId = _context.Session
                .AsNoTracking()
                .Include(s => s.Department)
                .ThenInclude(a => a.Facility)
                .Where(s => s.Id == sessionId)
                .Select(b => b.Department.Facility.Id).First();
            return IsRoleForFacility<TRole>(email, facilityId);
        }

        private bool IsRoleForDepartment<TRole>(int departmentId) where TRole : Domain.User.User
        {
            var facilityId = _context.Department.Include(a => a.Facility).First(a => a.Id == departmentId)
                .FacilityId;

            return IsRoleForFacility<TRole>(GetEmail(), facilityId);
        }

        /// <summary>
        /// Stjålet fra https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha512?view=net-5.0
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

        private static Expression<Func<Domain.User.User, bool>> UserWithHprNumberWithoutIdentityPseudonym(string hprNumber)
            => b => (b.IdentityPseudonym == null || b.IdentityPseudonym == "") && b.HPRNumber == hprNumber;

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
    }
}
