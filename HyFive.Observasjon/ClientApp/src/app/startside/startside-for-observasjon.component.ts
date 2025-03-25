import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FireIndikasjonerSesjonService } from "../services/data/fire-indikasjoner-sesjon.service";
import { InstitusjonService } from "../services/data/institusjon.service";
import { Institusjon } from "../models/api/Institusjon";
import { Rollevalg } from "../models/registrering/rollevalg.model";
import { Urls } from "../konstanter/urls";
import { faUserNurse, faCheck, faCircle } from "@fortawesome/free-solid-svg-icons";
import { Farger } from "../utils/farger";
import { HandsmykkeSesjonService } from "../services/data/handsmykke-sesjon.service";
import { Avdeling } from "../models/api/Avdeling";
import { SesjonType } from "../models/api/SesjonType";
import { HanskeSesjonService } from "../services/data/hansker-sesjon.service";
import { AutoriseringService } from "../services/data/autorisering.service";
import { InnloggetBruker } from "../models/api/InnloggetBruker";

@Component({
  selector: "app-startsideforobservasjon",
  templateUrl: "./startside-for-observasjon.component.html",
})
export class StartsideForObservasjonComponent implements OnInit {
  SesjonType = SesjonType;
  valgtSesjonType: SesjonType;
  tidtaking: boolean;
  hanskebruk: boolean;
  rollevalg: Rollevalg[];
  valgtAvdelingId: string = null;
  farger = Farger;
  visStartside: boolean;
  visBeskyttelsesutstyr: boolean;
  bruker: InnloggetBruker;
  institusjonAlternativer: Institusjon[];
  valgtInstitusjonAlternativId: number;
  institusjon: Institusjon;

  faCircle = faCircle;
  faUserNurse = faUserNurse;
  faCheck = faCheck;

  constructor(
    private router: Router,
    private fireIndikasjonerSesjonService: FireIndikasjonerSesjonService,
    private handsmykkeSesjonService: HandsmykkeSesjonService,
    private hanskeSesjonService: HanskeSesjonService,
    private institusjonService: InstitusjonService,
    private autoriseringService: AutoriseringService
  ) {}

  ngOnInit() {
    this.resetState();
    this.router.events.subscribe((e: any) => {
      if (e instanceof NavigationEnd) {
        this.resetState();
      }
    });
  }

  resetState() {
    this.valgtSesjonType = SesjonType.IkkeValgt;
    this.tidtaking = false;
    this.hanskebruk = false;
    this.rollevalg = [];
    this.valgtAvdelingId = null;
    this.visStartside = true;
    this.visBeskyttelsesutstyr = false;
    this.institusjonAlternativer = [];
    this.valgtInstitusjonAlternativId = 0;
    this.institusjon = null;
    this.institusjonService
      .getInstitusjoner()
      .subscribe((institusjoner: Institusjon[]) => {
        this.institusjonAlternativer = institusjoner;
        let enesteInstitusjon: Institusjon = null;
        if (this.institusjonAlternativer?.length == 1) {
          enesteInstitusjon = this.institusjonAlternativer[0];
        }

        this.institusjonService
          .getValgtInstitusjon()
          .subscribe((valgtInstitusjon) => {
            if (valgtInstitusjon) {
              this.institusjon = valgtInstitusjon;
            }
            if (!valgtInstitusjon && enesteInstitusjon) {
              this.institusjon = enesteInstitusjon;
              this.institusjonService.oppdaterValgtInstitusjonId(
                enesteInstitusjon.id
              );
            }
            this.valgtInstitusjonAlternativId = this.institusjon
              ? this.institusjon.id
              : 0;
          });
      });
    this.autoriseringService.getBruker().subscribe((bruker) => {
      this.bruker = bruker;
    });
  }

  startObservasjon() {
    if (!this.valgtAvdelingId) {
      alert("Velg en avdeling");
      return;
    }

    if (!this.rollevalg.filter((r) => r.erValgt).length) {
      alert("Velg en eller flere roller");
      return;
    }

    switch (this.valgtSesjonType) {
      case SesjonType.IkkeValgt:
        alert("Velg sesjonstypen du ønsker å starte");
        break;
      case SesjonType.FireIndikasjoner:
        this.startFireIndikasjonerSesjon();
        break;
      case SesjonType.Handsmykker:
        this.startHandsmykkeSesjon();
        break;
      case SesjonType.Hansker:
        this.startHanskeSesjon();
        break;
      case SesjonType.Beskyttelsesutstyr:
        this.visStartside = false;
        this.visBeskyttelsesutstyr = true;
        break;
      default:
        alert(
          `Observasjon av ${
            Object.values(SesjonType)[this.valgtSesjonType]
          } er ikke støttet enda`
        );
        break;
    }
  }

  startFireIndikasjonerSesjon() {
    let sesjonId = this.fireIndikasjonerSesjonService.lagSesjonsvisning(
      this.hanskebruk,
      this.tidtaking,
      this.rollevalg.filter((r) => r.erValgt).map((r) => r.rolle),
      this.hentValgtAvdeling()
    );

    this.router.navigate([Urls.RegistrereFireIndikasjonerUrl], {
      queryParams: { sesjonId: sesjonId },
    });
  }

  startHandsmykkeSesjon() {
    let sesjonId = this.handsmykkeSesjonService.lagSesjonsvisning(
      this.rollevalg.filter((r) => r.erValgt).map((r) => r.rolle),
      this.hentValgtAvdeling()
    );
    this.router.navigate([Urls.RegistrereHandsmykkerUrl], {
      queryParams: { sesjonId: sesjonId },
    });
  }

  startHanskeSesjon() {
    let sesjonId = this.hanskeSesjonService.lagSesjonsvisning(
      this.hanskebruk,
      this.rollevalg.filter((r) => r.erValgt).map((r) => r.rolle),
      this.hentValgtAvdeling()
    );

    this.router.navigate([Urls.RegistrereHanskeUrl], {
      queryParams: { sesjonId: sesjonId },
    });
  }

  valgtInstitusjonEndret() {
    // bytt institusjon
    this.institusjonService.oppdaterValgtInstitusjonId(
      this.valgtInstitusjonAlternativId
    );

    // endre valgtInstitusjon
    this.institusjon = this.institusjonAlternativer.find(
      (x) => x.id === this.valgtInstitusjonAlternativId
    );
    this.valgtAvdelingId = null;
    this.valgtAvdelingEndret();
  }

  hentValgtAvdeling(): Avdeling {
    return this.institusjon.avdelinger.find(
      (x) => x.id === parseInt(this.valgtAvdelingId)
    );
  }

  valgtAvdelingEndret() {
    this.rollevalg = this.institusjon.avdelinger
      .find((x) => x.id === parseInt(this.valgtAvdelingId))
      ?.roller.map((rolle) => {
        return { rolle: rolle, erValgt: false } as Rollevalg;
      });
  }

  kanIkkeStarteObservasjon(): boolean {
    return (
      this.valgtAvdelingId === null ||
      this.rollevalg.filter((r) => r.erValgt).length === 0 ||
      this.valgtSesjonType === SesjonType.IkkeValgt
    );
  }
}
