import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { faBars, faTimes } from '@fortawesome/free-solid-svg-icons';
import { MainMenuItem } from './main-menu-item.model';
import { UrlPaths } from '../../_felles/konstanter/url-paths';
import { AuthorizedRole } from '../../_felles/authorization/authorized-role';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { RolleEventService } from 'src/app/services/events/rolle-event.service';
@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html'
})
export class MainMenuComponent implements OnInit, OnDestroy {

  @Input() prosjektnavn: string;

  profilRoute = `/${UrlPaths.profil}`;
  faBars = faBars;
  faTimes = faTimes;
  mainMenuIsOpen = false;
  authorizedRoles: AuthorizedRole[] = [];
  AuthorizedRoleValues = AuthorizedRole;

  alleMenyvalg: MainMenuItem[];
  gjeldendeMenyvalg: MainMenuItem[];

  constructor(private authorizationService: AuthorizationService,
    private rolleEventService: RolleEventService) {
    this.lagAlleMenyvalg();
  }

  ngOnInit() {
    this.authorizationService.getRoller().subscribe((roller) => {
      this.authorizedRoles = roller;
      this.setValgtRolle();
      this.lagGjeldendeMenyvalg();
    });

    this.rolleEventService.byttRolleEvent.subscribe(
      (valgtRolle: AuthorizedRole) => {
        this.byttRolle(valgtRolle);
      });
  }

  setValgtRolle() {
    var valgtRolle = this.authorizationService.hentValgtRolle();
    if (valgtRolle != null) {
      let authorizedrolle = this.authorizedRoles.find(p => p === valgtRolle);
      if (authorizedrolle != null) {
        this.authorizedRoles = [authorizedrolle];
      }
    }
  }

  ngOnDestroy() {
    this.rolleEventService.byttRolleEvent.unsubscribe();
  }

  mainMenuClose(): void {
    this.mainMenuIsOpen = false;
  }

  mainMenuToggle(): void {
    this.mainMenuIsOpen = !this.mainMenuIsOpen;
  }

  private byttRolle(valgtRolle: AuthorizedRole) {
    this.authorizedRoles = [valgtRolle];
    this.lagGjeldendeMenyvalg();
  }

  // Vis menyvalg avhengig av rolle.
  // Dersom Observatør: vis bare Forside
  private lagGjeldendeMenyvalg() {

    let erAdmin = this.authorizedRoles.find(p => p === AuthorizedRole.Administrator);
    let erKoordinator = this.authorizedRoles.find(p => p === AuthorizedRole.Koordinator);
    let valgtRolle: AuthorizedRole = null;

    if (erAdmin) {
      valgtRolle = AuthorizedRole.Administrator;
    } else if (erKoordinator) {
      valgtRolle = AuthorizedRole.Koordinator;
    } else {
      valgtRolle = AuthorizedRole.Observator;
    }

    this.gjeldendeMenyvalg = this.alleMenyvalg.filter(menyvalg => {
      for (const role of menyvalg.roles) {
        if (valgtRolle === role) {
          return true;
        }
      }
      return false;
    });
  }

  private lagAlleMenyvalg() {
    this.alleMenyvalg = [
      {
        name: 'Forside',
        routerLink: `/${UrlPaths.forside}`,
        roles: [AuthorizedRole.Administrator, AuthorizedRole.Koordinator, AuthorizedRole.Observator]
      },
      {
        name: 'Observasjoner',
        routerLink: `/${UrlPaths.observasjoner}`,
        roles: [AuthorizedRole.Administrator, AuthorizedRole.Koordinator]
      },
      {
        name: 'Overfør sesjoner til FHI',
        routerLink: `/${UrlPaths.overforSesjoner}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Institusjoner',
        routerLink: `/${UrlPaths.redigeringAvInstitusjoner}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'Avdelinger',
        routerLink: `/${UrlPaths.redigeringAvAvdelinger}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Klinikker',
        routerLink: `/${UrlPaths.redigeringAvKlinikker}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Koordinatorer',
        routerLink: `/${UrlPaths.redigeringAvKoordinatorer}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Observatører',
        routerLink: `/${UrlPaths.redigeringAvObservatorer}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Kodeverk',
        routerLink: `/${UrlPaths.redigeringAvKodeverk}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'FHI Administratorer',
        routerLink: `/${UrlPaths.fhiAdminOversikt}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'Forespørsler',
        routerLink: `/${UrlPaths.foresporsel}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Predefinert kommentarer',
        routerLink: `/${UrlPaths.redigeringAvPredefinertkommentarer}`,
        roles: [AuthorizedRole.Koordinator]
      },
      {
        name: 'Helseforetak',
        routerLink: `/${UrlPaths.helseforetak}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'Epost',
        routerLink: `/${UrlPaths.epost}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
       name: 'Rapporter',
       routerLink: `/${UrlPaths.rapporter}`,
       roles: [AuthorizedRole.Administrator, AuthorizedRole.Koordinator]
      },
    ];
  }
}

