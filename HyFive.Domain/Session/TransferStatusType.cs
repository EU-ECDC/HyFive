using System.Collections.Generic;

namespace HyFive.Domain.Session
{
    public class TransferStatusType
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ICollection<Session> Sessions { get; set; }
    }
}