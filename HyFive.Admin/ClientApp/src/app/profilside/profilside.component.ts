import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../_felles/services/authorization.service';
import { LoggedinUser } from '../models/api/LoggedinUser';
import { AuthorizedRole } from '../_felles/authorization/authorized-role';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { RoleEventService } from '../services/events/role-event.service';

@Component({
  selector: 'app-profilside',
  templateUrl: './profilside.component.html'
})
export class ProfilsideComponent implements OnInit {

  user: LoggedinUser = null;
  AuthorizedRoleValues = AuthorizedRole;
  faUser = faUser;

  rolleAdministrator = 'Administrator';
  rolleKoordinator = 'Koordinator'
  valgtRolle = this.rolleAdministrator;
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

    let valgtRolle = this.authorizationService.getSelectedRole();
    if (valgtRolle) {
      if (valgtRolle === AuthorizedRole.Administrator) {
        this.valgtRolle = this.rolleAdministrator;
      } else if (valgtRolle === AuthorizedRole.Coordinator) {
        this.valgtRolle = this.rolleKoordinator;
      }
    }
  }

  byttRolle(){
    let rolle: AuthorizedRole;

    if (this.valgtRolle === this.rolleAdministrator) {
      rolle = AuthorizedRole.Administrator;
    } else if (this.valgtRolle === this.rolleKoordinator) {
      rolle = AuthorizedRole.Coordinator;
    }

    this.roleEventService.switchRoleEvent.emit(rolle);
    this.authorizationService.saveSelectedRole(rolle);
  }
}
