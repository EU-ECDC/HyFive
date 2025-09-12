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
            user.IsFhiAdmin = IsFhiAdmin(email);
            user.InstitutionIds = await _context.User.AsNoTracking().Include(k => k.Institution)
                .Where(HasEmailAndIsActive<Domain.User.User>(email))
                .Where(k => k.Institution != null)
                .Select(k => k.Institution.Id)
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

        public bool IsCoordinatorForInstitution(int institutionId, string email)
            => IsRoleForInstitution<Coordinator>(email, institutionId);

        public bool IsCoordinatorForInstitution(int institutionId)
            => IsRoleForInstitution<Coordinator>(GetEmail(), institutionId);

        public bool IsCoordinatorForHealthcareProvider(int healthcareProvider)
            => IsCoordinatorForHealthcareProvider(GetEmail(), healthcareProvider);

        public bool IsCoordinatorForInstitutions(List<int> institutionIds)
            => IsRoleForInstitutions<Coordinator>(institutionIds);

        public bool IsCoordinatorForInstitutionsOrAdmin(List<int> institutionIds)
        {
            return IsCoordinatorForInstitutions(institutionIds) || IsFhiAdmin();
        }

        public bool IsCoordinatorForDepartment(int departmentId)
            => IsRoleForDepartment<Coordinator>(departmentId);

        public bool IsCoordinatorForUser(int userId)
            => IsRoleForUser<Coordinator>(GetEmail(), userId);

        public bool IsObserverForInstitution(string email, int institutionId)
            => IsRoleForInstitution<Observer>(email, institutionId);

        public bool IsFhiAdminOrCoordinator(string email)
        {
            return IsFhiAdmin(email) || IsCoordinator(email);
        }

        public bool IsFhiAdmin(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            bool isFhiAdmin = _context.User.AsNoTracking().OfType<Admin>().AsNoTracking()
                    .Where(HasEmailAndIsActive<Admin>(email)).Any();
            return isFhiAdmin;
        }

        public bool IsFhiAdmin()
            => IsFhiAdmin(GetEmail());

        public bool IsCoordinatorForDepartmentOrFhiAdmin(int avdelingId)
        {
            return IsCoordinatorForDepartment(avdelingId) || IsFhiAdmin();
        }

        public bool IsObserverForInstitution(int institutionId)
        {
            return IsObserverForInstitution(GetEmail(), institutionId);
        }

        public bool IsCoordinatorForInstitutionOrFhiAdmin(int institutionId)
        {
            return IsCoordinatorForInstitution(institutionId) || IsFhiAdmin();
        }

        public bool IsCoordinatorForHealthcareProviderOrFhiAdmin(int healthcareProviderId)
        {
            return IsCoordinatorForHealthcareProvider(healthcareProviderId) || IsFhiAdmin();
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
            
            var institutionId = _context.Session.AsNoTracking().Include(s => s.Department).ThenInclude(a => a.Institution)
                .FirstOrDefault(s => s.Id == guidSessionId).Department?.InstitutionId;
            if (institutionId != null)
            {
                return IsRoleForInstitution<Coordinator>(GetEmail(), (int)institutionId);
            }

            return false;
        }

        public int GetObserverIdForInstitution(int institutionId)
        {
            var email = GetEmail();
            return _context.User.AsNoTracking()
                .OfType<Observer>()
                .Include(o => o.Institution)
                .Where(HasEmailAndIsActive<Domain.User.Observer>(email)).First(o => o.Institution.Id == institutionId)?.Id ?? 0;
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
                .Include(r => r.Institution)
                .Any(HasEmailAndIsActive<TRole>(email));
            
            return isRole;
        }

        private bool IsRoleForInstitution<TRole>(string email, int institutionId) where TRole : Domain.User.User
        {
            return _context.User.OfType<TRole>().AsNoTracking().Include(b => b.Institution)
                .Where(HasEmailAndIsActive<TRole>(email))
                .Any(b => b.Institution.Id == institutionId && b.Discriminator == GetDiscriminator<TRole>());
        }

        private bool IsCoordinatorForHealthcareProvider(string email, int healthcareOrganization)
        {
            return _context.User.OfType<Coordinator>().AsNoTracking().Include(b => b.Institution).ThenInclude(i=>i.HealthcareOrganization)
                .Where(HasEmailAndIsActive<Coordinator>(email))
                .Any(b => b.Institution.HealthcareOrganization.Id == healthcareOrganization);
        }

        private bool IsRoleForInstitutions<TRole>(List<int> institutionIds) where TRole : Domain.User.User
        {
            var email = GetEmail();
            var CoordinatorForInstitutionIds = _context
                .User
                .OfType<TRole>()
                .AsNoTracking()
                .Include(k => k.Institution)
                .Where(HasEmailAndIsActive<TRole>(email))
                .Where(k => k.Discriminator == GetDiscriminator<TRole>())
                .Select(k => k.Institution.Id).ToList();

            if (institutionIds == null)
            {
                return CoordinatorForInstitutionIds.Any();
            }

            return CoordinatorForInstitutionIds.Any() && institutionIds.ToList().TrueForAll(iid => CoordinatorForInstitutionIds.Contains(iid));
        }

        private bool IsRoleForUser<TRole>(string email, int userId) where TRole : Domain.User.User
        {
            var institutionId = _context
                .User
                .AsNoTracking()
                .Include(b => b.Institution)
                .Where(b => b.Id == userId)
                .Select(b => b.Institution.Id).First();
            return IsRoleForInstitution<TRole>(email, institutionId);
        }

        private bool IsRoleForSession<TRole>(string email, Guid sessionId) where TRole : Domain.User.User
        {
            var institutionId = _context.Session
                .AsNoTracking()
                .Include(s => s.Department)
                .ThenInclude(a => a.Institution)
                .Where(s => s.Id == sessionId)
                .Select(b => b.Department.Institution.Id).First();
            return IsRoleForInstitution<TRole>(email, institutionId);
        }

        private bool IsRoleForDepartment<TRole>(int departmentId) where TRole : Domain.User.User
        {
            var institutionId = _context.Department.Include(a => a.Institution).First(a => a.Id == departmentId)
                .InstitutionId;

            return IsRoleForInstitution<TRole>(GetEmail(), institutionId);
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
