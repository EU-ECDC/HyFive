using System;
using System.Collections.Generic;

namespace HyFive.Services.Rapporter.FireIndikasjoner
{
    public class FireIndikasjonerRapportForAvdeling
    {
        public DateTime FraTidspunkt { get; set; }
        public DateTime TilTidspunkt { get; set; }
        public FireIndikasjonerRapport Avdeling { get; set; } = new FireIndikasjonerRapport();
        public FireIndikasjonerRapport Institusjon { get; set; } = new FireIndikasjonerRapport();
        public FireIndikasjonerRapport SammenlignbareAvdelinger { get; set; } = new FireIndikasjonerRapport();
        public List<FireIndikasjonerRapport> Klinikker { get; set; } = new List<FireIndikasjonerRapport>();

        public void SettVisningstidspunkter(DateTime fraTidspunkt, DateTime tilTidspunkt)
        {
            FraTidspunkt = fraTidspunkt;
            TilTidspunkt = tilTidspunkt;
            Avdeling.FraTidspunkt = fraTidspunkt;
            Avdeling.TilTidspunkt = tilTidspunkt;
            Institusjon.FraTidspunkt = fraTidspunkt;
            Institusjon.TilTidspunkt = tilTidspunkt;
            SammenlignbareAvdelinger.FraTidspunkt = fraTidspunkt;
            SammenlignbareAvdelinger.TilTidspunkt = tilTidspunkt;
            Klinikker.ForEach(k =>
            {
                k.FraTidspunkt = fraTidspunkt;
                k.TilTidspunkt = tilTidspunkt;
            });
        }
    }
    


    public class FireIndikasjonerRapport
    {
        public string Enhet { get; set; }
        public string Navn { get; set; }
        public DateTime FraTidspunkt { get; set; }
        public DateTime TilTidspunkt { get; set; }
        public List<RolleMedKombinasjonerRapport> Roller { get; set; } = new List<RolleMedKombinasjonerRapport>();
        public string[] DebugObservasjonerStringList { get; set; }
        public int AntallObservasjoner { get; set; }
    }

    public class RolleMedKombinasjonerRapport
    {
        public string Navn { get; set; } // Sykepleier
        public int TotaltAntallObservasjoner { get; set; }
        public List<Kombinasjon> Kombinasjoner { get; set; } = new List<Kombinasjon>();
        public string SnittidHandvask { get; set; }
        public string SnittidHanddesinfeksjon { get; set; }
        public byte[] Chart { get; set; }
    }

    public class Kombinasjon
    {
        public string Rolle { get; set; }
        public string Navn { get; set; }
        public int AntallObservasjoner { get; set; }
        public double ProsentEtterlevd {get;set; }
        public double ProsentIkkeEtterlevd { get; set; }
    }
}
