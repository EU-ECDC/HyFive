import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { LoggedinUser } from '../../models/api/LoggedinUser';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { RoleEventService } from '../../services/events/role-event.service';
import { AuthorizedRole } from '../../_felles/authorization/authorized-role';
import { InstitutionForCoordinatorEventService } from '../../services/events/instittution-for-coordinator-event.service';
@Component({
  selector: 'app-change-institution',
  templateUrl: './change-institution.component.html'
})
export class ChangeInstitutionComponent implements OnInit {

  user: LoggedinUser = null;
  roles: string;
  faUser = faUser;
  selectedInstitution: InstitutionReport = null;
  selectedInstitutionTemp: InstitutionReport = null;
  showSelectInstitution = false;
  showChangeInstitutionBox = false;
  
  shownInstitutions: InstitutionReport[] = [];
  institutions: InstitutionReport[] = [];

  constructor(
    private authorizationService: AuthorizationService,
    private institutionService: InstitutionService,
    private roleEventService: RoleEventService,
    private institutionForCoordinatorEventService: InstitutionForCoordinatorEventService
  ) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user: LoggedinUser) => {
      this.user = user;
      let selectedRole = this.authorizationService.getSelectedRole();
      this.initialize(selectedRole);
    });

    this.roleEventService.switchRoleEvent.subscribe(
      (selectedRole) => {
        this.initialize(selectedRole);
      });

    this.institutionForCoordinatorEventService.updateInstitutionList.subscribe(
      () => {
        let selectedRole = this.authorizationService.getSelectedRole();
        this.initialize(selectedRole);
      });
  }

  private initialize(selectedRole: AuthorizedRole) {
    if (selectedRole === AuthorizedRole.Administrator) {
      this.showChangeInstitutionBox = false;
    } else if (selectedRole === AuthorizedRole.Coordinator) {
      this.institutionService.getInstitutionsForCoordinator().subscribe((institutions) => {
        if (institutions.length > 0) {
          this.showChangeInstitutionBox = true;
        }

        this.institutions = institutions;
        let selectedInstitutionId = this.institutionService.getSelectedInstitutionId()
        this.selectedInstitution = this.institutions.find(x => x.id === selectedInstitutionId);

        if (!this.selectedInstitution) {
          this.selectedInstitution = this.institutions[0];
          this.institutionService.updateSelectedInstitutionId(this.selectedInstitution.id);
        }

        this.shownInstitutions = this.institutions.filter(x => x.id !== this.selectedInstitution.id);
      });
    }
  }

  changeInstitution(institution: InstitutionReport) {  
    this.selectedInstitution = institution;
    this.institutionService.updateSelectedInstitutionId(this.selectedInstitution.id);
    window.location.reload();
  }
}
