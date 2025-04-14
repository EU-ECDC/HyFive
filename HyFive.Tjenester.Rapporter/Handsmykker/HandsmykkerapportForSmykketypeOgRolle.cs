using System;
using System.Collections.Generic;
using HyFive.Domene.Observation;

namespace HyFive.Services.Rapporter.Handsmykker
{
    public class HandsmykkerapportForSmykketypeOgRolle
    {
        public string Avdeling { get; set; }
        public string Institusjon { get; set; }
        public DateTime FraTidspunkt { get; set; }
        public DateTime TilTidspunkt { get; set; }
        public RapportForEnhet RapportForAvdeling { get; set; }
        public RapportForEnhet RapportForInstitusjon { get; set; }
    }

    public class RapportForEnhet
    {
        public List<SmykketypeOgAntallForRolle> SmykketypeOgAntallForRolleListe { get; set; }
        public List<ObservasjonerForRolle> ObservasjonerForRolleListe { get; set; }
    }

    public class SmykketypeOgAntallForRolle
    {
        public HandJewelryType Smykketype { get; set; }
        public List<AntallForRolle> AntallForRolleListe { get; set; }
    }

    public class AntallForRolle
    {
        public int Antall { get; set; }
        public string Rolle { get; set; }
    }

    public class ObservasjonerForRolle
    {
        public int Antall { get; set; }
        public string Rolle { get; set; }
    }
}
