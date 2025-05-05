import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FireIndikasjonerSesjonService } from "../services/data/fire-indikasjoner-sesjon.service";
import { InstitusjonService } from "../services/data/institusjon.service";
import { Institution } from "../models/api/Institution";
import { Rollevalg } from "../models/registrering/rollevalg.model";
import { Urls } from "../konstanter/urls";
import { faUserNurse, faCheck, faCircle } from "@fortawesome/free-solid-svg-icons";
import { Farger } from "../utils/farger";
import { HandsmykkeSesjonService } from "../services/data/handsmykke-sesjon.service";
import { Department } from "../models/api/Department";
import { SessionType } from "../models/api/SessionType";
import { HanskeSesjonService } from "../services/data/hansker-sesjon.service";
import { AutoriseringService } from "../services/data/autorisering.service";
import { LoggedInUser } from "../models/api/LoggedInUser";

@Component({
  selector: "app-startsideforobservasjon",
  templateUrl: "./startside-for-observasjon.component.html",
})
export class StartsideForObservasjonComponent implements OnInit {
  SessionType = SessionType;
  valgtSesjonType: SessionType;
  tidtaking: boolean;
  hanskebruk: boolean;
  rollevalg: Rollevalg[];
  valgtAvdelingId: string = null;
  farger = Farger;
  visStartside: boolean;
  visBeskyttelsesutstyr: boolean;
  bruker: LoggedInUser;
  institusjonAlternativer: Institution[];
  valgtInstitusjonAlternativId: number;
  institusjon: Institution;

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
    this.valgtSesjonType = SessionType.NotSelected;
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
      .subscribe((institusjoner: Institution[]) => {
        this.institusjonAlternativer = institusjoner;
        let enesteInstitusjon: Institution = null;
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
      alert("Velg en department");
      return;
    }

    if (!this.rollevalg.filter((r) => r.isSelected).length) {
      alert("Velg en eller flere roles");
      return;
    }

    switch (this.valgtSesjonType) {
      case SessionType.NotSelected:
        alert("Velg sesjonstypen du ønsker å starte");
        break;
      case SessionType.FourIndications:
        this.startFireIndikasjonerSesjon();
        break;
      case SessionType.HandJewelry:
        this.startHandsmykkeSesjon();
        break;
      case SessionType.Gloves:
        this.startHanskeSesjon();
        break;
      case SessionType.ProtectiveEquipment:
        this.visStartside = false;
        this.visBeskyttelsesutstyr = true;
        break;
      default:
        alert(
          `Observation av ${
            Object.values(SessionType)[this.valgtSesjonType]
          } er ikke støttet enda`
        );
        break;
    }
  }

  startFireIndikasjonerSesjon() {
    let sessionId = this.fireIndikasjonerSesjonService.lagSesjonsvisning(
      this.hanskebruk,
      this.tidtaking,
      this.rollevalg.filter((r) => r.isSelected).map((r) => r.role),
      this.hentValgtAvdeling()
    );

    this.router.navigate([Urls.RegistrereFireIndikasjonerUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  startHandsmykkeSesjon() {
    let sessionId = this.handsmykkeSesjonService.lagSesjonsvisning(
      this.rollevalg.filter((r) => r.isSelected).map((r) => r.role),
      this.hentValgtAvdeling()
    );
    this.router.navigate([Urls.RegistrereHandsmykkerUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  startHanskeSesjon() {
    let sessionId = this.hanskeSesjonService.lagSesjonsvisning(
      this.hanskebruk,
      this.rollevalg.filter((r) => r.isSelected).map((r) => r.role),
      this.hentValgtAvdeling()
    );

    this.router.navigate([Urls.RegistrereHanskeUrl], {
      queryParams: { sessionId: sessionId },
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

  hentValgtAvdeling(): Department {
    return this.institusjon.departments.find(
      (x) => x.id === parseInt(this.valgtAvdelingId)
    );
  }

  valgtAvdelingEndret() {
    this.rollevalg = this.institusjon.departments
      .find((x) => x.id === parseInt(this.valgtAvdelingId))
      ?.roles.map((role) => {
        return { role: role, isSelected: false } as Rollevalg;
      });
  }

  kanIkkeStarteObservasjon(): boolean {
    return (
      this.valgtAvdelingId === null ||
      this.rollevalg.filter((r) => r.isSelected).length === 0 ||
      this.valgtSesjonType === SessionType.NotSelected
    );
  }
}
