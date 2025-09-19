using System;

namespace HyFive.Domain.User
{
    public class UserAccessRequest
    {
        public int Id { get; set; }
        public int? FacilityId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string IdentityPseudonym { get; set; }
        public string HPRNumber { get; set; }
        public string Email { get; set; }
        public UserAccessRequestStatus Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ProcessedTime { get; set; }
        public int? ProcessedByUserID { get; set; }
        public string ProcessedByUsername { get; set; }
    }

    public enum UserAccessRequestStatus
    {
        Registered,
        Approved,
        Rejected
    }
}
