import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { LoggedInUser } from '../../models/api/LoggedInUser';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { CoordinatorForHealthcareOrganization } from '../../models/api/CoordinatorForHealthcareOrganization';
import { UserService } from '../../services/data/user.service';
import { HealthcareOrganizationService } from '../../services/data/healthcareOrganization.service';
import { InstitutionForCoordinatorEventService } from '../../services/events/instittution-for-coordinator-event.service';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../../_common/services/authorization.service';
import { ObservationService } from 'src/app/services/data/observation.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';


@Component({
  selector: 'app-edit-coordinators-for-healthcareOrganization',
  templateUrl: './edit-coordinators-for-healthcareOrganization.component.html'
})
export class EditCoordinatorsForHealthOrganizationComponent implements OnInit, OnDestroy {

  @Input() institution: InstitutionReport;
  coordinators: CoordinatorForHealthcareOrganization[];
  institutionsHealthcareOrganization: InstitutionReport[];

  coordinatorAsChanged: CoordinatorForHealthcareOrganization = null;
  newCoordinator: CoordinatorForHealthcareOrganization = null;

  dropdownSettings: IDropdownSettings;
  selectedInstitutions: InstitutionReport[] = [];
  user: LoggedInUser = null;
  keyword: string = '';
  filteredCoordinators: CoordinatorForHealthcareOrganization[];
  mailValidatorHelper;

  constructor(
    private HealthcareOrganizationService: HealthcareOrganizationService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private institutionForCoordinatorEventService: InstitutionForCoordinatorEventService,
    private authorizationService: AuthorizationService,
    private observationService: ObservationService
  ) {
    this.mailValidatorHelper = MailValidatorHelper;
   }

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
    this.HealthcareOrganizationService.getCoordinators(this.institution.healthcareOrganization.id).subscribe(
      (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators
      },
      (error) => this.toastrService.error('An error occurred while loading coordinators: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  loadInstitutions() {
    this.HealthcareOrganizationService.getInstitutions(this.institution.healthcareOrganization.id).subscribe(
      (institutions) => {
        this.institutionsHealthcareOrganization = institutions
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
    if (this.newCoordinator.identityPseudonym == "") {
        this.newCoordinator.identityPseudonym = null;
    }
    this.newCoordinator.institutions = this.selectedInstitutions;
    this.HealthcareOrganizationService.createCoordinator(this.institution.healthcareOrganization.id, this.newCoordinator).subscribe(
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

  setCoordinatorAsChanged(coordinator: CoordinatorForHealthcareOrganization) {
    if (this.isCoordinatorAsChanged(coordinator)) return;

    this.newCoordinator = null;
    this.resetSelectedInstitutions();
    let me = this;
    coordinator.institutions.forEach(function (institution) {
      me.selectedInstitutions.push(institution);
    });

    coordinator.modifiedHPRNumber = coordinator.hprNumber;
    coordinator.modifiedPseudonym = coordinator.identityPseudonym

    this.coordinatorAsChanged = JSON.parse(JSON.stringify(coordinator));
  }

  isCoordinatorAsChanged(coordinator: CoordinatorForHealthcareOrganization) {
    // if (this.coordinatorAsChanged?.hprNumber?.length > 0 &&
    //   this.coordinatorAsChanged.hprNumber === coordinator.hprNumber)
    //   return true;
    // if (this.coordinatorAsChanged?.identityPseudonym?.length > 0 &&
    //   this.coordinatorAsChanged.identityPseudonym === coordinator.identityPseudonym)
    //   return true;

    // return false;
    if (this.coordinatorAsChanged 
      // && this.coordinatorAsChanged?.id == coordinator.id
      ) {
      return true;
    } 
    return false;
  }

  updateCoordinator(coordinator: CoordinatorForHealthcareOrganization) {
    if (coordinator.identityPseudonym == "") {
        coordinator.identityPseudonym = null;
    }
    coordinator.institutions = this.selectedInstitutions;
    let CurrentInstitutionIsStillSelected = this.selectedInstitutions.some(i => i.id == this.institution.id);
    let isCoordinatorAsChangedLikeLoggedInUser = this.isCoordinatorAsChangedLikeLoggedInUser(coordinator);
    this.HealthcareOrganizationService.updateCoordinator(this.institution.healthcareOrganization.id, coordinator).subscribe(
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

  isCoordinatorAsChangedLikeLoggedInUser(coordinator: CoordinatorForHealthcareOrganization) {
    // if (this.user.hprNumber && this.user.hprNumber === coordinator.hprNumber)
    //   return true;
    // if (this.user.identityPseudonym && this.user.identityPseudonym === coordinator.identityPseudonym)
    //   return true;

    // return false;

    if (this.coordinatorAsChanged
      // && this.coordinatorAsChanged?.id == coordinator.id
      ) {
      return true;
    } 
    return false;
  }

  canCreate() {
    return this.newCoordinator.firstName.length > 0
      && this.newCoordinator.lastName.length > 0
      && this.newCoordinator.email.length > 0
      && this.mailValidatorHelper.validateMail(this.newCoordinator.email)
      && this.coordinators.find(coord => coord?.email == this.newCoordinator?.email) == undefined
      && this.userService.isValidPseudonym(this.newCoordinator.identityPseudonym)
      && this.selectedInstitutions?.length > 0;
  }

  canChange(coordinator: CoordinatorForHealthcareOrganization) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
      && coordinator.email.length > 0
      && this.mailValidatorHelper.validateMail(coordinator.email)
      && this.coordinators
                      // .filter(coord => coord.id !== coordinator.id)
                      .find(coord => coord?.email == coordinator?.email) == undefined
      && this.userService.isValidPseudonym(coordinator.identityPseudonym)
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

  showInstitutionsForCoordinator(coordinator: CoordinatorForHealthcareOrganization): string {
    const institutions = coordinator.institutions.map(institution => institution.name);
    return institutions.toString();
  }

  resetSelectedInstitutions() {
    this.selectedInstitutions.splice(0, this.selectedInstitutions.length);
  }

  identityPseudonymChanged(coordinator: CoordinatorForHealthcareOrganization, identityPseudonym: string) {
    coordinator.modifiedPseudonym = identityPseudonym;
  }

  filterCoordinators(): void {
    if (this.keyword.length >= 2)
    {
      this.filteredCoordinators = this.coordinators.filter(k => 
                                    k.firstName?.toLowerCase().includes(this.keyword.toLowerCase()) || 
                                    k.lastName?.toLocaleLowerCase().includes(this.keyword.toLowerCase()) ||
                                    // k.hprNumber?.includes(this.keyword) ||
                                    k.institutions?.some(i => i.name.toLowerCase().includes(this.keyword.toLowerCase())));
    }
    else if (this.keyword.length === 0)
      this.filteredCoordinators = this.coordinators;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: CoordinatorForHealthcareOrganization) => any;
    switch ($event.columnName) {
      case "Firstname":
        propertyOf = (x: CoordinatorForHealthcareOrganization) => x.firstName;
        break;
      case "Lastname":
        propertyOf = (x: CoordinatorForHealthcareOrganization) => x.lastName;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: CoordinatorForHealthcareOrganization, b: CoordinatorForHealthcareOrganization) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filteredCoordinators = this.filteredCoordinators.sort(sortFunc);
  }
}
