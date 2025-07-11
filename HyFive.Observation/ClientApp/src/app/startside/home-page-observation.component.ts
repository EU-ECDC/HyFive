import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FiveIndicationsSessionService } from "../services/data/five-indications-session.service";
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
  selector: "app-home-page-observation",
  templateUrl: "./home-page-observation.component.html",
})
export class HomePageForObservationComponent implements OnInit {
  SessionType = SessionType;
  selectedSessionType: SessionType;
  timekeeping: boolean;
  gloveUse: boolean;
  roleSelected: RoleSelected[];
  selectedDepartmentId: string = null;
  colors = Colors;
  showHomePage: boolean;
  showProtectiveEquipment: boolean;
  user: LoggedInUser;
  institutionOptions: Institution[];
  selectedInstitutionOptionId: number;
  institution: Institution;

  faCircle = faCircle;
  faUserNurse = faUserNurse;
  faCheck = faCheck;

  constructor(
    private router: Router,
    private fiveIndicationsSessionService: FiveIndicationsSessionService,
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
    this.selectedSessionType = SessionType.NotSelected;
    this.timekeeping = false;
    this.gloveUse = false;
    this.roleSelected = [];
    this.selectedDepartmentId = null;
    this.showHomePage = true;
    this.showProtectiveEquipment = false;
    this.institutionOptions = [];
    this.selectedInstitutionOptionId = null;
    this.institution = null;
    this.institutionService
      .getInstitutions()
      .subscribe((institutions: Institution[]) => {
        this.institutionOptions = institutions;
        let onlyInstitution: Institution = null;
        if (this.institutionOptions?.length == 1) {
          onlyInstitution = this.institutionOptions[0];
        }

        this.institutionService
          .getSelectedInstitution()
          .subscribe((selectedInstitution) => {
            if (selectedInstitution) {
              this.institution = selectedInstitution;
            }
            if (!selectedInstitution && onlyInstitution) {
              this.institution = onlyInstitution;
              this.institutionService.updateSelectedInstitutionId(
                onlyInstitution.id
              );
            }
            this.selectedInstitutionOptionId = this.institution
              ? this.institution.id
              : null;
          });
      });
    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    });
  }

  startObservation() {
    if (!this.selectedDepartmentId) {
      alert("Select a department");
      return;
    }

    if (!this.roleSelected.filter((r) => r.isSelected).length) {
      alert("Select one or more roles");
      return;
    }

    switch (this.selectedSessionType) {
      case SessionType.NotSelected:
        alert("Select the sessionType you want to start");
        break;
      case SessionType.FiveIndications:
        this.startFiveIndicationsSession();
        break;
      case SessionType.HandJewelry:
        this.startHandJewelrySession();
        break;
      case SessionType.Gloves:
        this.startGloveSession();
        break;
      case SessionType.ProtectiveEquipment:
        this.showHomePage = false;
        this.showProtectiveEquipment = true;
        break;
      default:
        alert(
          `Observation of ${
            Object.values(SessionType)[this.selectedSessionType]
          } is not supported yet`
        );
        break;
    }
  }

  startFiveIndicationsSession() {
    let sessionId = this.fiveIndicationsSessionService.createSessionView(
      this.gloveUse,
      this.timekeeping,
      this.roleSelected.filter((r) => r.isSelected).map((r) => r.role),
      this.getSelectedDepartment()
    );

    this.router.navigate([Urls.RegisterFiveIndicationsUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  startHandJewelrySession() {
    let sessionId = this.handJewelrySessionService.createSessionView(
      this.roleSelected.filter((r) => r.isSelected).map((r) => r.role),
      this.getSelectedDepartment()
    );
    this.router.navigate([Urls.RegisterHandJewelryUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  startGloveSession() {
    let sessionId = this.gloveSessionService.createSessionView(
      this.gloveUse,
      this.roleSelected.filter((r) => r.isSelected).map((r) => r.role),
      this.getSelectedDepartment()
    );

    this.router.navigate([Urls.RegisterGloveUrl], {
      queryParams: { sessionId: sessionId },
    });
  }

  selectedInstitutionChanged() {
    // change institution
    this.institutionService.updateSelectedInstitutionId(
      this.selectedInstitutionOptionId
    );

    // change selectedInstitution
    this.institution = this.institutionOptions.find(
      (x) => x.id === this.selectedInstitutionOptionId
    );
    this.selectedDepartmentId = null;
    this.selectedSessionType = SessionType.NotSelected;
    this.selectedDepartmentChanged();
  }

  getSelectedDepartment(): Department {
    return this.institution.departments.find(
      (x) => x.id === parseInt(this.selectedDepartmentId)
    );
  }

  selectedDepartmentChanged() {
    this.roleSelected = this.institution?.departments
      .find((x) => x.id === parseInt(this.selectedDepartmentId))
      ?.roles.map((role) => {
        return { role: role, isSelected: false } as RoleSelected;
      });
  }

  canNotStartObservation(): boolean {
    return (
      this.selectedDepartmentId === null ||
      this.roleSelected.filter((r) => r.isSelected).length === 0 ||
      this.selectedSessionType === SessionType.NotSelected
    );
  }
}
