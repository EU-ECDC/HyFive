import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FiveIndicationsSessionService } from "../services/data/five-indications-session.service";
import { Facility } from "../models/api/Facility";
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
import { FacilityService } from "../services/data/FacilityService";

@Component({
  selector: "app-home-page-observation",
  templateUrl: "./home-page-observation.component.html",
})
export class HomePageForObservationComponent implements OnInit {
  SessionType = SessionType;
  selectedSessionType: SessionType;
  time: boolean;
  gloveUse: boolean;
  roleSelected: RoleSelected[];
  selectedDepartmentId: string = null;
  colors = Colors;
  showHomePage: boolean;
  showProtectiveEquipment: boolean;
  user: LoggedInUser;
  facilityOptions: Facility[];
  selectedFacilityOptionId: number;
  facility: Facility;

  faCircle = faCircle;
  faUserNurse = faUserNurse;
  faCheck = faCheck;

  constructor(
    private readonly router: Router,
    private readonly fiveIndicationsSessionService: FiveIndicationsSessionService,
    private readonly handJewelrySessionService: HandJewelrySessionService,
    private readonly gloveSessionService: GloveSessionService,
    private readonly facilityService: FacilityService,
    private readonly authorizationService: AuthorizationService
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
    this.time = false;
    this.gloveUse = false;
    this.roleSelected = [];
    this.selectedDepartmentId = null;
    this.showHomePage = true;
    this.showProtectiveEquipment = false;
    this.facilityOptions = [];
    this.selectedFacilityOptionId = null;
    this.facility = null;
    this.facilityService
      .getFacilities()
      .subscribe((facilities: Facility[]) => {
        this.facilityOptions = facilities;
        let onlyFacility: Facility = null;
        if (this.facilityOptions?.length == 1) {
          onlyFacility = this.facilityOptions[0];
        }

        this.facilityService
          .getSelectedFacility()
          .subscribe((selectedFacility) => {
            if (selectedFacility) {
              this.facility = selectedFacility;
            }
            if (!selectedFacility && onlyFacility) {
              this.facility = onlyFacility;
              this.facilityService.updateSelectedFacilityId(
                onlyFacility.id
              );
            }
            this.selectedFacilityOptionId = this.facility
              ? this.facility.id
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
      // case SessionType.ProtectiveEquipment:
      //   this.showHomePage = false;
      //   this.showProtectiveEquipment = true;
      //   break;
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
      this.time,
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

  selectedFacilityChanged() {
    // change facility
    this.facilityService.updateSelectedFacilityId(
      this.selectedFacilityOptionId
    );

    // change selectedFacility
    this.facility = this.facilityOptions.find(
      (x) => x.id === this.selectedFacilityOptionId
    );
    this.selectedDepartmentId = null;
    this.selectedSessionType = SessionType.NotSelected;
    this.selectedDepartmentChanged();
  }

  getSelectedDepartment(): Department {
    return this.facility.departments.find(
      (x) => x.id === Number.parseInt(this.selectedDepartmentId)
    );
  }

  selectedDepartmentChanged() {
    this.roleSelected = this.facility?.departments
      .find((x) => x.id === Number.parseInt(this.selectedDepartmentId))
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
