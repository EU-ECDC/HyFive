using System;
using HyFive.Domain.User;
using HyFive.Domain.Place;

namespace HyFive.Domain.Session
{

    public class Session
    {
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public Guid Id { get; set; }
        public Department Department { get; set; }
        public Observer Observer { get; set; }
        public string Comment { get; set; }
        public string Discriminator { get; set; }
        public TransferStatusType TransferStatus { get; set; }
    }
}
