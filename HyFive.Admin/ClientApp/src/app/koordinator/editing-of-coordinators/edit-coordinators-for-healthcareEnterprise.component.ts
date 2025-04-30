import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { LoggedInUser } from '../../models/api/LoggedInUser';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareEnterprises } from '../../models/api/CoordinatorForHealthcareOrganization';
import { UserService } from '../../services/data/user.service';
import { HealthcareEnterpriseService } from '../../services/data/healthcareOrganization.service';
import { InstitutionForCoordinatorEventService } from '../../services/events/instittution-for-coordinator-event.service';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { ObservationService } from 'src/app/services/data/observation.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';


@Component({
  selector: 'app-edit-coordinators-for-healthcareOrganization',
  templateUrl: './edit-coordinators-for-healthcareOrganization.component.html'
})
export class EditCoordinatorsForHealthEnterprisesComponent implements OnInit, OnDestroy {

  @Input() institution: InstitutionReport;
  coordinators: CoordinatorForHealthcareEnterprises[];
  institutionsHealthcareEnterprise: InstitutionReport[];

  coordinatorAsChanged: CoordinatorForHealthcareEnterprises = null;
  newCoordinator: CoordinatorForHealthcareEnterprises = null;

  dropdownSettings: IDropdownSettings;
  selectedInstitutions: InstitutionReport[] = [];
  user: LoggedInUser = null;
  keyword: string = '';
  filteredCoordinators: CoordinatorForHealthcareEnterprises[];

  constructor(
    private healthcareEnterpriseService: HealthcareEnterpriseService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private institutionForCoordinatorEventService: InstitutionForCoordinatorEventService,
    private authorizationService: AuthorizationService,
    private observationService: ObservationService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    });

    this.loadCoordinators();
    this.loadInstitutions();

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select all',
      unSelectAllText: 'Select all',
      itemsShowLimit: 3
    };

  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadCoordinators() {
    this.healthcareEnterpriseService.getCoordinators(this.institution.healthcareOrganization.id).subscribe(
      (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators
      },
      (error) => this.toastrService.error('An error occurred while loading coordinators: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  loadInstitutions() {
    this.healthcareEnterpriseService.getInstitutions(this.institution.healthcareOrganization.id).subscribe(
      (institutions) => {
        this.institutionsHealthcareEnterprise = institutions
      },
      (error) => this.toastrService.error('An error occurred while loading institutions: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  createEmptyCoordinator() {
    this.cancelEdit();
    this.resetSelectedInstitutions();

    this.newCoordinator = {
      lastName: '',
      firstName: '',
      email: '',
      hprNumber: null,
      identityPseudonym: null,
      createdTime: new Date(),
      isDisabled: false,
      institutions: [this.institution]
    };
  }

  createCoordinator() {
    this.newCoordinator.institutions = this.selectedInstitutions;
    this.healthcareEnterpriseService.createCoordinator(this.institution.healthcareOrganization.id, this.newCoordinator).subscribe(
      (status) => {
        if (status.success) {
          this.toastrService.success('Coordinator(s) and observer(s) created');
          this.newCoordinator = null;
          this.loadCoordinators();
        }
        else {
          this.toastrService.error(status.errorMessage, '', { disableTimeOut: true });
        }
      },
      (error) => this.toastrService.error('An error occurred while creating coordinator(s) and/or observer(s): ' + error?.message, '', { disableTimeOut: true })
    );
  }

  setCoordinatorAsChanged(coordinator: CoordinatorForHealthcareEnterprises) {
    if (this.isCoordinatorAsChanged(coordinator)) return;

    this.newCoordinator = null;
    this.resetSelectedInstitutions();
    let me = this;
    coordinator.institutions.forEach(function (institution) {
      me.selectedInstitutions.push(institution);
    });

    coordinator.changedHPRNumber = coordinator.hprNumber;
    coordinator.changedIdentityPseudonym = coordinator.identityPseudonym

    this.coordinatorAsChanged = JSON.parse(JSON.stringify(coordinator));
  }

  isCoordinatorAsChanged(coordinator: CoordinatorForHealthcareEnterprises) {
    if (this.coordinatorAsChanged?.hprNumber?.length > 0 &&
      this.coordinatorAsChanged.hprNumber === coordinator.hprNumber)
      return true;
    if (this.coordinatorAsChanged?.identityPseudonym?.length > 0 &&
      this.coordinatorAsChanged.identityPseudonym === coordinator.identityPseudonym)
      return true;

    return false;
  }

  updateCoordinator(coordinator: CoordinatorForHealthcareEnterprises) {
    coordinator.institutions = this.selectedInstitutions;
    let CurrentInstitutionIsStillSelected = this.selectedInstitutions.some(i => i.id == this.institution.id);
    let isCoordinatorAsChangedLikeLoggedInUser = this.isCoordinatorAsChangedLikeLoggedInUser(coordinator);
    this.healthcareEnterpriseService.updateCoordinator(this.institution.healthcareOrganization.id, coordinator).subscribe(
      (status) => {
        if (status.success) {
          this.toastrService.success('Coordinator updated');

          if (isCoordinatorAsChangedLikeLoggedInUser) {
            if (coordinator.isDisabled)
              this.authorizationService.logout();

            if (CurrentInstitutionIsStillSelected)
              this.institutionForCoordinatorEventService.updateInstitutionList.emit();
          }

          if (isCoordinatorAsChangedLikeLoggedInUser && !CurrentInstitutionIsStillSelected)
            window.location.reload();
          else {
            this.coordinatorAsChanged = null;
            this.loadCoordinators();
          }

          this.institutionForCoordinatorEventService.updateInstitutionList.emit();
        }
        else {
          this.toastrService.error(status.errorMessage, '', { disableTimeOut: true });
        }
      },
      (error) => this.toastrService.error('An error occurred while updating coordinator: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  isCoordinatorAsChangedLikeLoggedInUser(coordinator: CoordinatorForHealthcareEnterprises) {
    if (this.user.hprNumber && this.user.hprNumber === coordinator.hprNumber)
      return true;
    if (this.user.identityPseudonym && this.user.identityPseudonym === coordinator.identityPseudonym)
      return true;

    return false;
  }

  canCreate() {
    return this.newCoordinator.firstName.length > 0
      && this.newCoordinator.lastName.length > 0
      && this.userService.hasCoordinatorValidHprnumberOrPseudonym(this.newCoordinator)
      && this.selectedInstitutions?.length > 0;
  }

  canChange(coordinator: CoordinatorForHealthcareEnterprises) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
      && this.userService.hasCoordinatorValidHprnumberOrPseudonym(coordinator)
      && this.selectedInstitutions?.length > 0;
  }

  cancelEdit($event: Event = null) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.coordinatorAsChanged = null;
    this.newCoordinator = null;
  }

  showInstitutionsForCoordinator(coordinator: CoordinatorForHealthcareEnterprises): string {
    const institutions = coordinator.institutions.map(institution => institution.name);
    return institutions.toString();
  }

  resetSelectedInstitutions() {
    this.selectedInstitutions.splice(0, this.selectedInstitutions.length);
  }

  identityPseudonymChanged(coordinator: CoordinatorForHealthcareEnterprises, identityPseudonym: string) {
    coordinator.changedIdentityPseudonym = identityPseudonym;
  }

  filterCoordinators(): void {
    if (this.keyword.length >= 2)
    {
      this.filteredCoordinators = this.coordinators.filter(k => 
                                    k.firstName?.toLowerCase().includes(this.keyword.toLowerCase()) || 
                                    k.lastName?.toLocaleLowerCase().includes(this.keyword.toLowerCase()) ||
                                    k.hprNumber?.includes(this.keyword) ||
                                    k.institutions?.some(i => i.name.toLowerCase().includes(this.keyword.toLowerCase())));
    }
    else if (this.keyword.length === 0)
      this.filteredCoordinators = this.coordinators;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: CoordinatorForHealthcareEnterprises) => any;
    switch ($event.columnName) {
      case "Firstname":
        propertyOf = (x: CoordinatorForHealthcareEnterprises) => x.firstName;
        break;
      case "Lastname":
        propertyOf = (x: CoordinatorForHealthcareEnterprises) => x.lastName;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: CoordinatorForHealthcareEnterprises, b: CoordinatorForHealthcareEnterprises) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filteredCoordinators = this.filteredCoordinators.sort(sortFunc);
  }
}
