using System;
using ObserverUser = HyFive.Domain.User.User;
using HyFive.Domain.Place;

namespace HyFive.Domain.Session
{

    public class Session
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public int OrganisationUnitId { get; set; }
        public OrganisationUnit OrganisationUnit { get; set; }
        public int? ObserverId { get; set; }
        public ObserverUser Observer { get; set; }
        public string Comment { get; set; }
        public string Discriminator { get; set; }
        public int? TransferStatusId { get; set; }
        public TransferStatusType TransferStatus { get; set; }
    }
}
