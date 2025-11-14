import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { LoggedInUser } from '../../models/api/LoggedInUser';
import { FacilityReport } from '../../models/api/FacilityReport';
import { CoordinatorForCity } from '../../models/api/CoordinatorForCity';
import { CityService } from '../../services/data/City.service';
import { FacilityForCoordinatorEventService } from '../../services/events/facility-for-coordinator-event.service';
import { KeyEventService } from '../../services/events/key-event.service';
import { AuthorizationService } from '../../_common/services/authorization.service';
import { ObservationService } from 'src/app/services/data/observation.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';


@Component({
  selector: 'app-edit-coordinators-for-city',
  templateUrl: './edit-coordinators-for-city.component.html'
})
export class EditCoordinatorsForCityComponent implements OnInit, OnDestroy {

  @Input() facility: FacilityReport;
  coordinators: CoordinatorForCity[];
  facilitiesCity: FacilityReport[];

  coordinatorAsChanged: CoordinatorForCity = null;
  newCoordinator: CoordinatorForCity = null;

  dropdownSettings: IDropdownSettings;
  selectedFacilities: FacilityReport[] = [];
  user: LoggedInUser = null;
  keyword: string = '';
  filteredCoordinators: CoordinatorForCity[];
  mailValidatorHelper;

  constructor(
    private cityService: CityService,
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
    this.cityService.getCoordinators(this.facility.city.id).subscribe(
      (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators
      },
      (error) => this.toastrService.error('An error occurred while loading coordinators: ' + error?.message, '', { disableTimeOut: true }),
    );
  }

  loadFacilities() {
    this.cityService.getFacilities(this.facility.city.id).subscribe(
      (facilities) => {
        this.facilitiesCity = facilities
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
    this.cityService.createCoordinator(this.facility.city.id, this.newCoordinator).subscribe(
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

  setCoordinatorAsChanged(coordinator: CoordinatorForCity) {
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

  isCoordinatorAsChanged(coordinator: CoordinatorForCity) {
    if (this.coordinatorAsChanged 
      && this.coordinatorAsChanged?.id == coordinator.id
      ) {
      return true;
    } 
    return false;
  }

  updateCoordinator(coordinator: CoordinatorForCity) {
    if (coordinator.modifiedPseudonym == "") {
        coordinator.modifiedPseudonym = null;
    }
    coordinator.facilities = this.selectedFacilities;
    let CurrentFacilityIsStillSelected = this.selectedFacilities.some(i => i.id == this.facility.id);
    let isCoordinatorAsChangedLikeLoggedInUser = this.isCoordinatorAsChangedLikeLoggedInUser(coordinator);
    this.cityService.updateCoordinator(this.facility.city.id, coordinator).subscribe(
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

  isCoordinatorAsChangedLikeLoggedInUser(coordinator: CoordinatorForCity) {
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

  canChange(coordinator: CoordinatorForCity) {
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
    let k;  
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

  showfacilitiesForCoordinator(coordinator: CoordinatorForCity): string {
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
    let propertyOf: (x: CoordinatorForCity) => any;
    switch ($event.columnName) {
      case "First name":
        propertyOf = (x: CoordinatorForCity) => x.firstName;
        break;
      case "Last name":
        propertyOf = (x: CoordinatorForCity) => x.lastName;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: CoordinatorForCity, b: CoordinatorForCity) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filteredCoordinators = this.filteredCoordinators.sort(sortFunc);
  }
}
