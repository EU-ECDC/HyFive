import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../_felles/services/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { AuthorizedRole } from '../_felles/authorization/authorized-role';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { RoleEventService } from '../services/events/role-event.service';

@Component({
  selector: 'app-profilside',
  templateUrl: './profilside.component.html'
})
export class ProfilsideComponent implements OnInit {

  user: LoggedInUser = null;
  AuthorizedRoleValues = AuthorizedRole;
  faUser = faUser;

  rolleAdministrator = 'Administrator';
  rolleKoordinator = 'Coordinator'
  selectedRole = this.rolleAdministrator;
  kanBytteRolle = false;

  constructor(
    public authorizationService: AuthorizationService,
    private roleEventService: RoleEventService) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    });

    this.authorizationService.getRoles().subscribe((roles) => {
      if (roles.length > 1) {
        this.kanBytteRolle = true;
      }
    });

    let selectedRole = this.authorizationService.getSelectedRole();
    if (selectedRole) {
      if (selectedRole === AuthorizedRole.Administrator) {
        this.selectedRole = this.rolleAdministrator;
      } else if (selectedRole === AuthorizedRole.Coordinator) {
        this.selectedRole = this.rolleKoordinator;
      }
    }
  }

  byttRolle(){
    let role: AuthorizedRole;

    if (this.selectedRole === this.rolleAdministrator) {
      role = AuthorizedRole.Administrator;
    } else if (this.selectedRole === this.rolleKoordinator) {
      role = AuthorizedRole.Coordinator;
    }

    this.roleEventService.switchRoleEvent.emit(role);
    this.authorizationService.saveSelectedRole(role);
  }
}
