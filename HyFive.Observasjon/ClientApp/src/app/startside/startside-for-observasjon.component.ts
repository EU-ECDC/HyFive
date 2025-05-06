import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FourIndicationsSessionService } from "../services/data/four-indications-session.service";
import { InstitutionService } from "../services/data/InstitutionService";
import { Institution } from "../models/api/Institution";
import { RoleSelected } from "../models/registration/roleSelected.model";
import { Urls } from "../constants/urls";
import { faUserNurse, faCheck, faCircle } from "@fortawesome/free-solid-svg-icons";
import { Colors } from "../utils/colors";
import { HandJewelrySessionService } from "../services/data/hand-Jewelry-session.service";
import { Department } from "../models/api/Department";
import { SessionType } from "../models/api/SessionType";
import { GloveSessionService } from "../services/data/glove-session.service";
import { AuthorizationService } from "../services/data/authorization.service";
import { LoggedInUser } from "../models/api/LoggedInUser";

@Component({
  selector: "app-startsideforobservasjon",
  templateUrl: "./startside-for-observasjon.component.html",
})
export class StartsideForObservasjonComponent implements OnInit {
  SessionType = SessionType;
  valgtSesjonType: SessionType;
  timekeeping: boolean;
  hanskebruk: boolean;
  roleSelected: RoleSelected[];
  valgtAvdelingId: string = null;
  colors = Colors;
  visStartside: boolean;
  visBeskyttelsesutstyr: boolean;
  user: LoggedInUser;
  institusjonAlternativer: Institution[];
  valgtInstitusjonAlternativId: number;
  institution: Institution;

  faCircle = faCircle;
  faUserNurse = faUserNurse;
  faCheck = faCheck;

  constructor(
    private router: Router,
    private fourIndicationsSessionService: FourIndicationsSessionService,
    private handJewelrySessionService: HandJewelrySessionService,
    private gloveSessionService: GloveSessionService,
    private institutionService: InstitutionService,
    private authorizationService: AuthorizationService
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
    this.timekeeping = false;
    this.hanskebruk = false;
    this.roleSelected = [];
    this.valgtAvdelingId = null;
    this.visStartside = true;
    this.visBeskyttelsesutstyr = false;
    this.institusjonAlternativer = [];
    this.valgtInstitusjonAlternativId = 0;
    this.institution = null;
    this.institutionService
      .getInstitutions()
      .subscribe((institutions: Institution[]) => {
        this.institusjonAlternativer = institutions;
        let enesteInstitusjon: Institution = null;
        if (this.institusjonAlternativer?.length == 1) {
          enesteInstitusjon = this.institusjonAlternativer[0];
        }

        this.institutionService
          .getSelectedInstitution()
          .subscribe((selectedInstitution) => {
            if (selectedInstitution) {
              this.institution = selectedInstitution;
            }
            if (!selectedInstitution && enesteInstitusjon) {
              this.institution = enesteInstitusjon;
              this.institutionService.updateSelectedInstitutionId(
                enesteInstitusjon.id
              );
            }
            this.valgtInstitusjonAlternativId = this.institution
              ? this.institution.id
              : 0;
          });
      });
    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    });
  }

  startObservation() {
    if (!this.valgtAvdelingId) {
      alert("Select en department");
      return;
    }

    if (!this.roleSelected.filter((r) => r.isSelected).length) {
      alert("Select en eller flere roles");
      return;
    }

    switch (this.valgtSesjonType) {
      case SessionType.NotSelected:
        alert("Select sesjonstypen du ønsker å start");
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
    let sessionId = this.fourIndicationsSessionService.createSessionView(
      this.hanskebruk,
      this.timekeeping,
      this.roleSelected.filter((r) => r.isSelected).map((r) => r.role),
      this.hentValgtAvdeling()
    );

    this.router.navigate([Urls.RegisterFourndicationsUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  startHandsmykkeSesjon() {
    let sessionId = this.handJewelrySessionService.createSessionView(
      this.roleSelected.filter((r) => r.isSelected).map((r) => r.role),
      this.hentValgtAvdeling()
    );
    this.router.navigate([Urls.RegisterHandJewelryUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  startHanskeSesjon() {
    let sessionId = this.gloveSessionService.createSessionView(
      this.hanskebruk,
      this.roleSelected.filter((r) => r.isSelected).map((r) => r.role),
      this.hentValgtAvdeling()
    );

    this.router.navigate([Urls.RegisterGloveUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  valgtInstitusjonEndret() {
    // bytt institution
    this.institutionService.updateSelectedInstitutionId(
      this.valgtInstitusjonAlternativId
    );

    // endre selectedInstitution
    this.institution = this.institusjonAlternativer.find(
      (x) => x.id === this.valgtInstitusjonAlternativId
    );
    this.valgtAvdelingId = null;
    this.valgtAvdelingEndret();
  }

  hentValgtAvdeling(): Department {
    return this.institution.departments.find(
      (x) => x.id === parseInt(this.valgtAvdelingId)
    );
  }

  valgtAvdelingEndret() {
    this.roleSelected = this.institution.departments
      .find((x) => x.id === parseInt(this.valgtAvdelingId))
      ?.roles.map((role) => {
        return { role: role, isSelected: false } as RoleSelected;
      });
  }

  kanIkkeStarteObservasjon(): boolean {
    return (
      this.valgtAvdelingId === null ||
      this.roleSelected.filter((r) => r.isSelected).length === 0 ||
      this.valgtSesjonType === SessionType.NotSelected
    );
  }
}
