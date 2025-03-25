using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using HyFive.Domene.Sesjon;

namespace HyFive.Domene.Observasjon
{
    public class HandsmykkeObservasjon : Observasjon
    {
        public ICollection<HandsmykkeType> Handsmykker { get; set; }
        public HandsmykkeSesjon HandsmykkeSesjon { get; set; }
    }
}
