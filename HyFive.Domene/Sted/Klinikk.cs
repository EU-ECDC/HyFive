using System;
using System.Collections.Generic;
using System.Text;

namespace HyFive.Domene.Sted
{
    public class Klinikk
    {
        public int Id { get; set; }
        public string Navn { get; set; }
        public Institusjon Institusjon { get; set; }
        public ICollection<Avdeling> Avdelinger { get; set; }
    }
}
