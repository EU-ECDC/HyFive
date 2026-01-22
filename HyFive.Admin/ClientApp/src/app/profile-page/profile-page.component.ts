import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../_common/services/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { AuthorizedRole } from '../_common/authorization/authorized-role';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { RoleEventService } from '../services/events/role-event.service';

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html'
})
export class ProfilsideComponent implements OnInit {

  user: LoggedInUser = null;
  AuthorizedRoleValues = AuthorizedRole;
  faUser = faUser;

  roleAdministrator = 'Administrator';
  roleCoordinator = 'Coordinator'
  selectedRole = this.roleAdministrator;
  CanChangeRole = false;

  constructor(
    public authorizationService: AuthorizationService,
    private readonly roleEventService: RoleEventService) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    });

    this.authorizationService.getRoles().subscribe((roles) => {
      if (roles.length > 1) {
        this.CanChangeRole = true;
      }
    });

    let selectedRole = this.authorizationService.getSelectedRole();
    if (selectedRole) {
      if (selectedRole === AuthorizedRole.Administrator) {
        this.selectedRole = this.roleAdministrator;
      } else if (selectedRole === AuthorizedRole.Coordinator) {
        this.selectedRole = this.roleCoordinator;
      }
    }
  }

  changeRole(){
    let role: AuthorizedRole;

    if (this.selectedRole === this.roleAdministrator) {
      role = AuthorizedRole.Administrator;
    } else if (this.selectedRole === this.roleCoordinator) {
      role = AuthorizedRole.Coordinator;
    }

    this.roleEventService.switchRoleEvent.emit(role);
    this.authorizationService.saveSelectedRole(role);
  }

  logout() {
    localStorage.clear();
    globalThis.location.href = '/account/logout';
  }
}
