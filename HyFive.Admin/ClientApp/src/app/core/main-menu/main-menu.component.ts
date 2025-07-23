import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { faBars, faTimes } from '@fortawesome/free-solid-svg-icons';
import { MainMenuItem } from './main-menu-item.model';
import { UrlPaths } from '../../_common/konstanter/url-paths';
import { AuthorizedRole } from '../../_common/authorization/authorized-role';
import { AuthorizationService } from '../../_common/services/authorization.service';
import { RoleEventService } from 'src/app/services/events/role-event.service';
@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html'
})
export class MainMenuComponent implements OnInit, OnDestroy {

  @Input() projectName: string;

  profilRoute = `/${UrlPaths.profile}`;
  faBars = faBars;
  faTimes = faTimes;
  mainMenuIsOpen = false;
  authorizedRoles: AuthorizedRole[] = [];
  AuthorizedRoleValues = AuthorizedRole;

  allMenuOptions: MainMenuItem[];
  currentMenuSelection: MainMenuItem[];

  constructor(private authorizationService: AuthorizationService,
    private roleEventService: RoleEventService) {
    this.createAllMenuOptions();
  }

  ngOnInit() {
    this.authorizationService.getRoles().subscribe((roles) => {
      this.authorizedRoles = roles;
      this.setSelectedRole();
      this.createCurrentMenuOptions();
    });

    this.roleEventService.switchRoleEvent.subscribe(
      (selectedRole: AuthorizedRole) => {
        this.switchRole(selectedRole);
      });
  }

  setSelectedRole() {
    var selectedRole = this.authorizationService.getSelectedRole();
    if (selectedRole != null) {
      let authorizedrolle = this.authorizedRoles.find(p => p === selectedRole);
      if (authorizedrolle != null) {
        this.authorizedRoles = [authorizedrolle];
      }
    }
  }

  ngOnDestroy() {
    this.roleEventService.switchRoleEvent.unsubscribe();
  }

  mainMenuClose(): void {
    this.mainMenuIsOpen = false;
  }

  mainMenuToggle(): void {
    this.mainMenuIsOpen = !this.mainMenuIsOpen;
  }

  private switchRole(selectedRole: AuthorizedRole) {
    this.authorizedRoles = [selectedRole];
    this.createCurrentMenuOptions();
  }

  // Show menuOption depending on role.
  // If Observer: show only Home Page
  private createCurrentMenuOptions() {

    let isAdmin = this.authorizedRoles.find(p => p === AuthorizedRole.Administrator);
    let isCoordinator = this.authorizedRoles.find(p => p === AuthorizedRole.Coordinator);
    let selectedRole: AuthorizedRole = null;

    if (isAdmin) {
      selectedRole = AuthorizedRole.Administrator;
    } else if (isCoordinator) {
      selectedRole = AuthorizedRole.Coordinator;
    } else {
      selectedRole = AuthorizedRole.Observer;
    }

    this.currentMenuSelection = this.allMenuOptions.filter(menuOption => {
      for (const role of menuOption.roles) {
        if (selectedRole === role) {
          return true;
        }
      }
      return false;
    });
  }

  private createAllMenuOptions() {
    this.allMenuOptions = [
      {
        name: 'Home Page',
        routerLink: `/${UrlPaths.homePage}`,
        roles: [AuthorizedRole.Administrator, AuthorizedRole.Coordinator, AuthorizedRole.Observer]
      },
      {
        name: 'Observations',
        routerLink: `/${UrlPaths.observations}`,
        roles: [AuthorizedRole.Administrator, AuthorizedRole.Coordinator]
      },
      {
        name: 'Transfer sessions',
        routerLink: `/${UrlPaths.transferSessions}`,
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
        name: 'Administrators',
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
        name: 'Health Care Organization',
        routerLink: `/${UrlPaths.healthcareOrganization}`,
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

