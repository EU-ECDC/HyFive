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

  profilRoute = `/${UrlPaths.profile}`;
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
    this.authorizationService.getRoller().subscribe((roles) => {
      this.authorizedRoles = roles;
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

    let isAdmin = this.authorizedRoles.find(p => p === AuthorizedRole.Administrator);
    let isCoordinator = this.authorizedRoles.find(p => p === AuthorizedRole.Coordinator);
    let valgtRolle: AuthorizedRole = null;

    if (isAdmin) {
      valgtRolle = AuthorizedRole.Administrator;
    } else if (isCoordinator) {
      valgtRolle = AuthorizedRole.Coordinator;
    } else {
      valgtRolle = AuthorizedRole.Observer;
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
        name: 'FrontPage',
        routerLink: `/${UrlPaths.frontPage}`,
        roles: [AuthorizedRole.Administrator, AuthorizedRole.Coordinator, AuthorizedRole.Observer]
      },
      {
        name: 'Observations',
        routerLink: `/${UrlPaths.observations}`,
        roles: [AuthorizedRole.Administrator, AuthorizedRole.Coordinator]
      },
      {
        name: 'Transfer sessions to FHI',
        routerLink: `/${UrlPaths.oppositeSessions}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Institutions',
        routerLink: `/${UrlPaths.editingByInstitutions}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'Departments',
        routerLink: `/${UrlPaths.editingOfDepartments}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Clinics',
        routerLink: `/${UrlPaths.editingByClinics}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Coordinators',
        routerLink: `/${UrlPaths.editingByCoordinators}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Observers',
        routerLink: `/${UrlPaths.editingByObservers}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Coding works',
        routerLink: `/${UrlPaths.editingByCodeworks}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'FHI Administrators',
        routerLink: `/${UrlPaths.fhiAdminOverview}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'Requests',
        routerLink: `/${UrlPaths.request}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Predefined comments',
        routerLink: `/${UrlPaths.editingPredefinedComments}`,
        roles: [AuthorizedRole.Coordinator]
      },
      {
        name: 'Health company',
        routerLink: `/${UrlPaths.healthcareCompany}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
        name: 'Email',
        routerLink: `/${UrlPaths.email}`,
        roles: [AuthorizedRole.Administrator]
      },
      {
       name: 'Reports',
       routerLink: `/${UrlPaths.reports}`,
       roles: [AuthorizedRole.Administrator, AuthorizedRole.Coordinator]
      },
    ];
  }
}

