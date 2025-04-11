using System;
using HyFive.Domain.User;
using HyFive.Domain.Place;

namespace HyFive.Domain.Session
{

    public class Session
    {
        public DateTime CreatedTime { get; set; }
        public DateTime StartTime { get; set; }
        public Guid Id { get; set; }
        public Department Department { get; set; }
        public Observer Observer { get; set; }
        public string Comment { get; set; }
        public string Discriminator { get; set; }
        public TransmissionStatusType TransmissionStatus { get; set; }
    }
}
