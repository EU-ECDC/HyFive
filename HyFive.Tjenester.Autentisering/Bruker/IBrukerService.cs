using HyFive.Modeller.V1.Autentisering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HyFive.Tjenester.Autentisering.Bruker
{
    public interface IBrukerService
    {
        Task<InnloggetBruker> HentBruker();

        bool ErBrukerLoggetInn();

        bool ErKoordinator(string hprnummer, string pseudonym);

        bool ErObservator(string hprnummer, string identPseudonym);

        bool ErKoordinatorForInstitusjon(int institusjonId);

        bool ErKoordinatorForAvdeling(int avdelingId);

        bool ErFhiAdminEllerKoordinator(string pseudonym, string hprnummer);

        bool ErFhiAdmin(string identPseudonym, string hprnummer);

        bool ErFhiAdmin();

        bool ErKoordinatorForAvdelingEllerFhiAdmin(int avdelingId);

        bool ErObservatorForInstitusjon(int institusjonId);

        bool ErKoordinatorForInstitusjonEllerFhiAdmin(int institusjonId);

        bool ErKoordinatorForHelseforetakEllerFhiAdmin(int helseforetakId);

        string HentHprnummer();

        bool ErKoordinatorForSesjon(string sesjonId);

        int HentObservatorIdForInstitusjon(int institusjonId);

        string HentPseudonym();

        Expression<Func<TBruker, bool>> HarHprEllerPseudonymOgErAktiv<TBruker>(string hprnummer, string identPseudonym) where TBruker : Domene.Bruker.Bruker;
    }
}
