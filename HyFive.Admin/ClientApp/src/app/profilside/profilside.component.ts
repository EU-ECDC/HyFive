import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../_felles/services/authorization.service';
import { InnloggetBruker } from '../models/api/InnloggetBruker';
import { AuthorizedRole } from '../_felles/authorization/authorized-role';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { RolleEventService } from '../services/events/rolle-event.service';

@Component({
  selector: 'app-profilside',
  templateUrl: './profilside.component.html'
})
export class ProfilsideComponent implements OnInit {

  bruker: InnloggetBruker = null;
  AuthorizedRoleValues = AuthorizedRole;
  faUser = faUser;

  rolleAdministrator = 'Administrator';
  rolleKoordinator = 'Koordinator'
  valgtRolle = this.rolleAdministrator;
  kanBytteRolle = false;

  constructor(
    public authorizationService: AuthorizationService,
    private rolleEventService: RolleEventService) { }

  ngOnInit(): void {
    this.authorizationService.getBruker().subscribe((bruker) => {
      this.bruker = bruker;
    });

    this.authorizationService.getRoller().subscribe((roller) => {
      if (roller.length > 1) {
        this.kanBytteRolle = true;
      }
    });

    let valgtRolle = this.authorizationService.hentValgtRolle();
    if (valgtRolle) {
      if (valgtRolle === AuthorizedRole.Administrator) {
        this.valgtRolle = this.rolleAdministrator;
      } else if (valgtRolle === AuthorizedRole.Koordinator) {
        this.valgtRolle = this.rolleKoordinator;
      }
    }
  }

  byttRolle(){
    let rolle: AuthorizedRole;

    if (this.valgtRolle === this.rolleAdministrator) {
      rolle = AuthorizedRole.Administrator;
    } else if (this.valgtRolle === this.rolleKoordinator) {
      rolle = AuthorizedRole.Koordinator;
    }

    this.rolleEventService.byttRolleEvent.emit(rolle);
    this.authorizationService.lagreValgtRolle(rolle);
  }
}
