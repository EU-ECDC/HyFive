using HyFive.DataAccess;
using HyFive.Domain.User;
using HyFive.Models.V1.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
            var user = new LoggedInUser()
            {
                Name = "Vits Grønn",
            };
            
            var logInName = user?.Name;
            var hrpNumber = GetHprNumber();
            var pseudonym = GetPseudonym();

            user.Id = CreateHash(pseudonym + user.Name + HashSalt);
            user.IsObserver = IsObserver(hrpNumber, pseudonym);
            user.IsCoordinator = IsCoordinator(hrpNumber, pseudonym);
            user.IsFhiAdmin = IsFhiAdmin(pseudonym, hrpNumber);
            user.HPRNumber = hrpNumber;
            user.IdentityPseudonym = pseudonym;
            user.InstitutionIds = await _context.User.AsNoTracking().Include(k => k.Institution)
                .Where(HasHprOrPseudonymAndIsActive<Domain.User.User>(hrpNumber, pseudonym))
                .Where(k => k.Institution != null)
                .Select(k => k.Institution.Id)
                .ToListAsync();
            user.FirstName = GetFirstName();
            user.LastName = GetLastName();

            if (string.IsNullOrEmpty(user?.HPRNumber))
            {
                _logger.LogInformation("TI02: User: {logInName} lacks hprNumber (i HealthId)", logInName);
            }
            return user;
        }

        public bool IsUserLoggedIn()
        {
            return true;
        }


        public bool IsCoordinator(string hprNumber, string pseudonym)
            => IsRole<Coordinator>(hprNumber, pseudonym);

        public bool IsObserver(string hprNumber, string identityPseudonym)
            => IsRole<Observer>(hprNumber, identityPseudonym);

        public bool IsCoordinatorForInstitution(int institutionId, string identityPseudonym, string hprNumber)
            => IsRoleForInstitution<Coordinator>(hprNumber, identityPseudonym, institutionId);

        public bool IsCoordinatorForInstitution(int institutionId)
            => IsRoleForInstitution<Coordinator>(GetHprNumber(), GetPseudonym(), institutionId);

        public bool IsCoordinatorForHealthcareProvider(int healthcareProvider)
            => IsCoordinatorForHealthcareProvider(GetHprNumber(), GetPseudonym(), healthcareProvider);

        public bool IsCoordinatorForInstitutions(int[] institutionIds)
            => IsRoleForInstitutions<Coordinator>(institutionIds);

        public bool IsCoordinatorForDepartment(int departmentId)
            => IsRoleForDepartment<Coordinator>(departmentId);

        public bool IsCoordinatorForUser(int userID)
            => IsRoleForUser<Coordinator>(GetHprNumber(), GetPseudonym(), userID);

        public bool IsObserverForInstitution(string hprNumber, string identityPseudonym, int institutionId)
            => IsRoleForInstitution<Observer>(hprNumber, identityPseudonym, institutionId);

        public bool IsFhiAdminOrCoordinator(string pseudonym, string hprNumber)
        {
            return IsFhiAdmin(pseudonym, hprNumber) || IsCoordinator(hprNumber, pseudonym);
        }

        public bool IsFhiAdmin(string identityPseudonym, string hprNumber)
        {
            if (string.IsNullOrEmpty(hprNumber) && string.IsNullOrEmpty(identityPseudonym))
                return false;

            bool erFhiAdmin = _context.User.AsNoTracking().OfType<FhiAdmin>().AsNoTracking()
                    .Where(HasHprOrPseudonymAndIsActive<FhiAdmin>(hprNumber, identityPseudonym)).Any();
            return erFhiAdmin;
        }

        public bool IsFhiAdmin()
            => IsFhiAdmin(GetPseudonym(), GetHprNumber());

        public bool IsCoordinatorForDepartmentOrFhiAdmin(int departmentId)
        {
            return IsCoordinatorForDepartment(departmentId) || IsFhiAdmin();
        }

        public bool IsObserverForInstitution(int institutionId)
        {
            return IsObserverForInstitution(GetHprNumber(), GetPseudonym(), institutionId);
        }

        public bool IsCoordinatorForInstitutionOrFhiAdmin(int institutionId)
        {
            return IsCoordinatorForInstitution(institutionId) || IsFhiAdmin();
        }

        public bool IsCoordinatorForHealthcareProviderOrFhiAdmin(int healthcareProvider)
        {
            return IsCoordinatorForHealthcareProvider(healthcareProvider) || IsFhiAdmin();
        }

        public string GetHprNumber()
        {
            return "4909402";
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
                return IsRoleForInstitution<Coordinator>(GetHprNumber(), GetPseudonym(), (int)institutionId);
            }

            return false;
        }

        public int GetObserverIdForInstitution(int institutionId)
        {
            var hprNumber = GetHprNumber();
            var pseudonym = GetPseudonym();
            return _context.User.AsNoTracking()
                .OfType<Observer>()
                .Include(o => o.Institution)
                .Where(HasHprOrPseudonymAndIsActive<Domain.User.User>(hprNumber, pseudonym)).First(o => o.Institution.Id == institutionId)?.Id ?? 0;
        }

        public Expression<Func<TUser, bool>> HasHprOrPseudonymAndIsActive<TUser>(string hprNumber, string identityPseudonym) where TUser : Domain.User.User
        {
            return b => ((!string.IsNullOrEmpty(hprNumber) && b.HPRNumber == hprNumber) || (!string.IsNullOrEmpty(b.IdentityPseudonym) && b.IdentityPseudonym == identityPseudonym)) && b.IsDeactivated == false;
        }

        private bool IsRole<TRole>(string hprNumber, string pseudonym) where TRole : Domain.User.User
        {
            var isRole = _context.User.AsNoTracking().OfType<TRole>()
                .Include(r => r.Institution)
                .Any(HasHprOrPseudonymAndIsActive<TRole>(hprNumber, pseudonym));

            if (isRole)
            {
                // Update all users with an IdentityPseudonym if they don't have one.
                UpdateUserWithPseudonym<TRole>(hprNumber).GetAwaiter().GetResult();
            }
            return isRole;
        }

        private bool IsRoleForInstitution<TRole>(string hprNumber, string identityPseudonym, int institutionId) where TRole : Domain.User.User
        {
            return _context.User.OfType<TRole>().AsNoTracking().Include(b => b.Institution)
                .Where(HasHprOrPseudonymAndIsActive<TRole>(hprNumber, identityPseudonym))
                .Any(b => b.Institution.Id == institutionId && b.Discriminator == GetDiscriminator<TRole>());
        }

        private bool IsCoordinatorForHealthcareProvider(string hprNumber, string identityPseudonym, int healthcareOrganization)
        {
            return _context.User.OfType<Coordinator>().AsNoTracking().Include(b => b.Institution).ThenInclude(i => i.HealthcareOrganization)
                .Where(HasHprOrPseudonymAndIsActive<Coordinator>(hprNumber, identityPseudonym))
                .Any(b => b.Institution.HealthcareOrganization.Id == healthcareOrganization);
        }

        private bool IsRoleForInstitutions<TRole>(int[] InstitutionIds) where TRole : Domain.User.User
        {
            var hprNumber = GetHprNumber();
            var pseudonym = GetPseudonym();
            var CoordinatorForInstitutionIds = _context
                .User
                .OfType<TRole>()
                .AsNoTracking()
                .Include(k => k.Institution)
                .Where(HasHprOrPseudonymAndIsActive<TRole>(hprNumber, pseudonym))
                .Where(k => k.Discriminator == GetDiscriminator<TRole>())
                .Select(k => k.Institution.Id).ToList();

            if (InstitutionIds == null)
            {
                return CoordinatorForInstitutionIds.Any();
            }

            return CoordinatorForInstitutionIds.Any() && InstitutionIds.ToList().TrueForAll(iid => CoordinatorForInstitutionIds.Contains(iid));
        }

        private bool IsRoleForUser<TRole>(string hprNumber, string pseudonym, int userId) where TRole : Domain.User.User
        {
            var institutionId = _context
                .User
                .AsNoTracking()
                .Include(b => b.Institution)
                .Where(b => b.Id == userId)
                .Select(b => b.Institution.Id).First();
            return IsRoleForInstitution<TRole>(hprNumber, pseudonym, institutionId);
        }

        private bool IsRoleForSession<TRole>(string hprNumber, string pseudonym, Guid sessionId) where TRole : Domain.User.User
        {
            var institutionId = _context.Session
                .AsNoTracking()
                .Include(s => s.Department)
                .ThenInclude(a => a.Institution)
                .Where(s => s.Id == sessionId)
                .Select(b => b.Department.Institution.Id).First();
            return IsRoleForInstitution<TRole>(hprNumber, pseudonym, institutionId);
        }

        private bool IsRoleForDepartment<TRole>(int departmentId) where TRole : Domain.User.User
        {
            var institutionId = _context.Department.Include(a => a.Institution).First(a => a.Id == departmentId)
                .InstitutionId;

            return IsRoleForInstitution<TRole>(GetHprNumber(), GetPseudonym(), institutionId);
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
            return "OCW6BpVN57vnbxBUE8WOOTM9FrkCaBixlD2y8FgYCag=";
        }

        private string GetDiscriminator<T>() where T : class
            => typeof(T).Name;

        private static Expression<Func<Domain.User.User, bool>> UserWithHprNumberWithoutIdentityPseudonym(string hprNumber)
            => b => (b.IdentityPseudonym == null || b.IdentityPseudonym == "") && b.HPRNumber == hprNumber;

        private async Task UpdateUserWithPseudonym<TRole>(string hprNumber) where TRole : Domain.User.User
        {
            try
            {
                var userIdsToBeUpdated = _context
                    .User
                    .OfType<TRole>()
                    .AsNoTracking()
                    .Where(UserWithHprNumberWithoutIdentityPseudonym(hprNumber))
                    .ToList();

                if (userIdsToBeUpdated.Any())
                {
                    _logger.LogInformation(
                        $"TI01: {nameof(UpdateUserWithPseudonym)}: Updating users! IDs: {string.Join(',', userIdsToBeUpdated.Select(b => b.Id))}");

                    var pseudonym = GetPseudonym();
                    foreach (var user in userIdsToBeUpdated)
                    {
                        user.IdentityPseudonym = pseudonym;
                        _context.Entry(user).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "TE01: Error during the update of the user who had not set a Pseudonym");
            }

        }

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
