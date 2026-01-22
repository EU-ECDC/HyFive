import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../../_common/services/authorization.service';
import { LoggedInUser } from '../../models/api/LoggedInUser';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { FacilityService } from '../../services/data/facility.service';
import { FacilityReport } from '../../models/api/FacilityReport';
import { RoleEventService } from '../../services/events/role-event.service';
import { AuthorizedRole } from '../../_common/authorization/authorized-role';
import { FacilityForCoordinatorEventService } from '../../services/events/facility-for-coordinator-event.service';
@Component({
  selector: 'app-change-facility',
  templateUrl: './change-facility.component.html'
})
export class ChangeFacilityComponent implements OnInit {

  user: LoggedInUser = null;
  roles: string;
  faUser = faUser;
  selectedFacility: FacilityReport = null;
  showChangeFacilityBox = false;
  
  shownFacilities: FacilityReport[] = [];
  facilities: FacilityReport[] = [];

  constructor(
    private readonly authorizationService: AuthorizationService,
    private readonly facilityService: FacilityService,
    private readonly roleEventService: RoleEventService,
    private readonly facilityForCoordinatorEventService: FacilityForCoordinatorEventService
  ) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user: LoggedInUser) => {
      this.user = user;
      let selectedRole = this.authorizationService.getSelectedRole();
      this.initialize(selectedRole);
    });

    this.roleEventService.switchRoleEvent.subscribe(
      (selectedRole) => {
        this.initialize(selectedRole);
      });

    this.facilityForCoordinatorEventService.updateFacilityList.subscribe(
      () => {
        let selectedRole = this.authorizationService.getSelectedRole();
        this.initialize(selectedRole);
      });
  }

  private initialize(selectedRole: AuthorizedRole) {
    if (selectedRole === AuthorizedRole.Administrator) {
      this.showChangeFacilityBox = false;
    } else if (selectedRole === AuthorizedRole.Coordinator) {
      this.facilityService.getFacilitiesForCoordinator().subscribe((facilities) => {
        if (facilities.length > 0) {
          this.showChangeFacilityBox = true;
        }

        this.facilities = facilities;
        let selectedFacilityId = this.facilityService.getSelectedFacilityId()
        this.selectedFacility = this.facilities.find(x => x.id === selectedFacilityId);

        if (!this.selectedFacility) {
          this.selectedFacility = this.facilities[0];
          this.facilityService.updateSelectedFacilityId(this.selectedFacility.id);
        }

        this.shownFacilities = this.facilities.filter(x => x.id !== this.selectedFacility.id);
      });
    }
  }

  changeFacility(facility: FacilityReport) {  
    this.selectedFacility = facility;
    this.facilityService.updateSelectedFacilityId(this.selectedFacility.id);
    globalThis.location.reload();
  }
}
