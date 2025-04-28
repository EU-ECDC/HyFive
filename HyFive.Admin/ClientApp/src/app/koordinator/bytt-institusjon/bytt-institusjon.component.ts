import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { LoggedinUser } from '../../models/api/LoggedinUser';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { RoleEventService } from '../../services/events/role-event.service';
import { AuthorizedRole } from '../../_felles/authorization/authorized-role';
import { InstitusjonForKoordinatorEventService } from '../../services/events/institusjon-for-koordinator-event.service';
@Component({
  selector: 'app-bytt-institusjon',
  templateUrl: './bytt-institusjon.component.html'
})
export class ByttInstitusjonComponent implements OnInit {

  user: LoggedinUser = null;
  roles: string;
  faUser = faUser;
  valgtInstitusjon: InstitutionReport = null;
  valgtInstitusjonTemp: InstitutionReport = null;
  visVelgInstitusjon = false;
  visByttInstitusjonBoks = false;
  
  visteInstitusjoner: InstitutionReport[] = [];
  institutions: InstitutionReport[] = [];

  constructor(
    private authorizationService: AuthorizationService,
    private institutionService: InstitutionService,
    private roleEventService: RoleEventService,
    private institusjonForKoordinatorEventService: InstitusjonForKoordinatorEventService
  ) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user: LoggedinUser) => {
      this.user = user;
      let selectedRole = this.authorizationService.getSelectedRole();
      this.initialiser(selectedRole);
    });

    this.roleEventService.switchRoleEvent.subscribe(
      (selectedRole) => {
        this.initialiser(selectedRole);
      });

    this.institusjonForKoordinatorEventService.oppdaterInstitusjonsListe.subscribe(
      () => {
        let selectedRole = this.authorizationService.getSelectedRole();
        this.initialiser(selectedRole);
      });
  }

  private initialiser(selectedRole: AuthorizedRole) {
    if (selectedRole === AuthorizedRole.Administrator) {
      this.visByttInstitusjonBoks = false;
    } else if (selectedRole === AuthorizedRole.Coordinator) {
      this.institutionService.getInstitutionsForCoordinator().subscribe((institutions) => {
        if (institutions.length > 0) {
          this.visByttInstitusjonBoks = true;
        }

        this.institutions = institutions;
        let selectedInstitutionId = this.institutionService.getSelectedInstitutionId()
        this.valgtInstitusjon = this.institutions.find(x => x.id === selectedInstitutionId);

        if (!this.valgtInstitusjon) {
          this.valgtInstitusjon = this.institutions[0];
          this.institutionService.updateSelectedInstitutionId(this.valgtInstitusjon.id);
        }

        this.visteInstitusjoner = this.institutions.filter(x => x.id !== this.valgtInstitusjon.id);
      });
    }
  }

  byttInstitusjon(institusjon: InstitutionReport) {  
    this.valgtInstitusjon = institusjon;
    this.institutionService.updateSelectedInstitutionId(this.valgtInstitusjon.id);
    window.location.reload();
  }
}
