import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { LoggedInUser } from '../../models/api/LoggedInUser';
import { FacilityReport } from '../../models/api/FacilityReport';
import { CoordinatorForHealthcareOrganization } from '../../models/api/CoordinatorForHealthcareOrganization';
import { UserService } from '../../services/data/user.service';
import { HealthcareOrganizationService } from '../../services/data/healthcareOrganization.service';
import { FacilityForCoordinatorEventService } from '../../services/events/facility-for-coordinator-event.service';
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

  @Input() facility: FacilityReport;
  coordinators: CoordinatorForHealthcareOrganization[];
  facilitiesHealthcareOrganization: FacilityReport[];

  coordinatorAsChanged: CoordinatorForHealthcareOrganization = null;
  newCoordinator: CoordinatorForHealthcareOrganization = null;

  dropdownSettings: IDropdownSettings;
  selectedFacilities: FacilityReport[] = [];
  user: LoggedInUser = null;
  keyword: string = '';
  filteredCoordinators: CoordinatorForHealthcareOrganization[];
  mailValidatorHelper;

  constructor(
    private HealthcareOrganizationService: HealthcareOrganizationService,
    private userService: UserService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private facilityForCoordinatorEventService: FacilityForCoordinatorEventService,
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
    this.loadFacilities();

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
    this.HealthcareOrganizationService.getCoordinators(this.facility.healthcareOrganization.id).subscribe(
      (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators
      },
      (error) => this.toastrService.error('An error occurred while loading coordinators: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  loadFacilities() {
    this.HealthcareOrganizationService.getFacilities(this.facility.healthcareOrganization.id).subscribe(
      (facilities) => {
        this.facilitiesHealthcareOrganization = facilities
      },
      (error) => this.toastrService.error('An error occurred while loading facilities: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  createEmptyCoordinator() {
    this.cancelEdit();
    this.resetSelectedFacilitys();

    this.newCoordinator = {
      lastName: '',
      firstName: '',
      email: '',
      hprNumber: null,
      identityPseudonym: null,
      createdTime: new Date(),
      isDisabled: false,
      facilities: [this.facility]
    };
  }

  createCoordinator() {
    if (this.newCoordinator.modifiedPseudonym == "") {
        this.newCoordinator.modifiedPseudonym = null;
    }
    this.newCoordinator.facilities = this.selectedFacilities;
    this.HealthcareOrganizationService.createCoordinator(this.facility.healthcareOrganization.id, this.newCoordinator).subscribe(
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
    this.resetSelectedFacilitys();
    let me = this;
    coordinator.facilities.forEach(function (facility) {
      me.selectedFacilities.push(facility);
    });

    coordinator.modifiedHPRNumber = coordinator.hprNumber;
    coordinator.modifiedPseudonym = coordinator.identityPseudonym

    this.coordinatorAsChanged = JSON.parse(JSON.stringify(coordinator));
  }

  isCoordinatorAsChanged(coordinator: CoordinatorForHealthcareOrganization) {
    if (this.coordinatorAsChanged 
      && this.coordinatorAsChanged?.id == coordinator.id
      ) {
      return true;
    } 
    return false;
  }

  updateCoordinator(coordinator: CoordinatorForHealthcareOrganization) {
    if (coordinator.modifiedPseudonym == "") {
        coordinator.modifiedPseudonym = null;
    }
    coordinator.facilities = this.selectedFacilities;
    let CurrentFacilityIsStillSelected = this.selectedFacilities.some(i => i.id == this.facility.id);
    let isCoordinatorAsChangedLikeLoggedInUser = this.isCoordinatorAsChangedLikeLoggedInUser(coordinator);
    this.HealthcareOrganizationService.updateCoordinator(this.facility.healthcareOrganization.id, coordinator).subscribe(
      (status) => {
        if (status.success) {
          this.toastrService.success('Coordinator updated');

          if (isCoordinatorAsChangedLikeLoggedInUser) {
            if (coordinator.isDisabled)
              this.authorizationService.logout();

            if (CurrentFacilityIsStillSelected)
              this.facilityForCoordinatorEventService.updateFacilityList.emit();
          }

          if (isCoordinatorAsChangedLikeLoggedInUser && !CurrentFacilityIsStillSelected)
            window.location.reload();
          else {
            this.coordinatorAsChanged = null;
            this.loadCoordinators();
          }

          this.facilityForCoordinatorEventService.updateFacilityList.emit();
        }
        else {
          this.toastrService.error(status.errorMessage, '', { disableTimeOut: true });
        }
      },
      (error) => this.toastrService.error('An error occurred while updating coordinator: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  isCoordinatorAsChangedLikeLoggedInUser(coordinator: CoordinatorForHealthcareOrganization) {
    if (this.user.id == coordinator.id.toString()
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
      && this.selectedFacilities?.length > 0;
  }

  canChange(coordinator: CoordinatorForHealthcareOrganization) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
      && coordinator.email.length > 0
      && this.mailValidatorHelper.validateMail(coordinator.email)
      && this.coordinators
                      .filter(coord => coord.id !== coordinator.id)
                      .find(coord => coord?.email == coordinator?.email) == undefined
      && this.selectedFacilities?.length > 0;
  }

  omitSpecialChar(event) {   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  cancelEdit($event: Event = null) {
    if ($event) {
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.coordinatorAsChanged = null;
    this.newCoordinator = null;
  }

  showfacilitiesForCoordinator(coordinator: CoordinatorForHealthcareOrganization): string {
    const facilities = coordinator.facilities.map(facility => facility.name);
    return facilities.toString();
  }

  resetSelectedFacilitys() {
    this.selectedFacilities.splice(0, this.selectedFacilities.length);
  }

  filterCoordinators(): void {
    if (this.keyword.length >= 2)
    {
      this.filteredCoordinators = this.coordinators.filter(k => 
                                    k.firstName?.toLowerCase().includes(this.keyword.toLowerCase()) || 
                                    k.lastName?.toLocaleLowerCase().includes(this.keyword.toLowerCase()) ||
                                    // k.hprNumber?.includes(this.keyword) ||
                                    k.facilities?.some(i => i.name.toLowerCase().includes(this.keyword.toLowerCase())));
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
