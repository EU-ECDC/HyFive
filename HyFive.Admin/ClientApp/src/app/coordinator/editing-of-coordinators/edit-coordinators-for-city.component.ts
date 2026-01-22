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
import { TranslateService } from '@ngx-translate/core';
import { SortHelper } from 'src/app/utils/sort-helper';


@Component({
  selector: 'app-edit-coordinators-for-city',
  templateUrl: './edit-coordinators-for-city.component.html'
})
export class EditCoordinatorsForCityComponent implements OnInit, OnDestroy {

  @Input() facility: FacilityReport;
  coordinators: CoordinatorForCity[];
  facilitiesCity: FacilityReport[];

  coordinatorAsChanged: CoordinatorForCity = null;

  dropdownSettings: IDropdownSettings;
  selectedFacilities: FacilityReport[] = [];
  user: LoggedInUser = null;
  keyword: string = '';
  filteredCoordinators: CoordinatorForCity[];
  mailValidatorHelper;
  showCreateForm: boolean = false;

  constructor(
    private readonly cityService: CityService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly facilityForCoordinatorEventService: FacilityForCoordinatorEventService,
    private readonly authorizationService: AuthorizationService,
    private readonly observationService: ObservationService,
    private readonly translate: TranslateService

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
      selectAllText: this.translate.instant('Select all'),
      unSelectAllText: this.translate.instant('Select all'),
      noDataAvailablePlaceholderText: this.translate.instant('No data available'),
      itemsShowLimit: 3
    };

  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadCoordinators(): void {
    this.cityService.getCoordinators(this.facility.city.id).subscribe({
      next: (coordinators) => {
        this.coordinators = coordinators;
        this.filteredCoordinators = this.coordinators;
      },
      error: (error) => {
        this.toastrService.error(
          this.translate.instant('An error occurred while loading coordinators: ') + error?.message,
          '',
          { disableTimeOut: true }
        );
      }
    });
  }

  loadFacilities() {
    this.cityService.getFacilities(this.facility.city.id).subscribe(
      (facilities) => {
        this.facilitiesCity = facilities
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading facilities: ') + error?.message, '', { disableTimeOut: true }),
    );
  }

  toggleCreateForm() {
    this.showCreateForm = !this.showCreateForm;
    this.cancelEdit();
    this.resetSelectedFacilitys();
  }


  createCoordinator(newCoordinator: CoordinatorForCity): void {
    if (newCoordinator.modifiedPseudonym === '') {
      newCoordinator.modifiedPseudonym = null;
    }

    this.cityService
      .createCoordinator(this.facility.city.id, newCoordinator)
      .subscribe({
        next: (status) => {
          this.showCreateForm = false;

          if (status.success) {
            this.toastrService.success(
              this.translate.instant('Coordinator(s) and observer(s) created')
            );
            this.loadCoordinators();
          } else {
            this.toastrService.error(
              status.errorMessage,
              '',
              { disableTimeOut: true }
            );
          }
        },
        error: (error) => {
          this.toastrService.error(
            this.translate.instant(
              'An error occurred while creating coordinator(s) and/or observer(s): '
            ) + error?.message,
            '',
            { disableTimeOut: true }
          );
        }
      });
  }

  setCoordinatorAsChanged(coordinator: CoordinatorForCity) {
    this.showCreateForm = false;
    if (this.isCoordinatorAsChanged(coordinator)) return;

    this.resetSelectedFacilitys();
    let me = this;
    for (const facility of coordinator.facilities) {
      me.selectedFacilities.push(facility);
    };

    coordinator.modifiedHPRNumber = coordinator.hprNumber;
    coordinator.modifiedPseudonym = coordinator.identityPseudonym

    this.coordinatorAsChanged = structuredClone(coordinator);
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
          this.toastrService.success(this.translate.instant('Coordinator updated'));

          if (isCoordinatorAsChangedLikeLoggedInUser) {
            if (coordinator.isDisabled)
              this.authorizationService.logout();

            if (CurrentFacilityIsStillSelected)
              this.facilityForCoordinatorEventService.updateFacilityList.emit();
          }

          if (isCoordinatorAsChangedLikeLoggedInUser && !CurrentFacilityIsStillSelected)
            globalThis.location.reload();
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
      (error) => this.toastrService.error(this.translate.instant('An error occurred while updating coordinator: ') + error?.message, '', { disableTimeOut: true })
    );
  }

  isCoordinatorAsChangedLikeLoggedInUser(coordinator: CoordinatorForCity) {
    if (this.user.id == coordinator.id.toString()
      ) {
      return true;
    } 
    return false;
  }

  canChange(coordinator: CoordinatorForCity) {
    return coordinator.firstName.length > 0
      && coordinator.lastName.length > 0
      && coordinator.email.length > 0
      && this.mailValidatorHelper.validateMail(coordinator.email)
      && !this.coordinators
                      .filter(coord => coord.id !== coordinator.id)
                      .some(coord => coord?.email == coordinator?.email)
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
  }

  cancelEditEmitted() {
    this.showCreateForm = false;
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
    const userSortConfig = {
      [this.translate.instant("First name")]: (x: CoordinatorForCity) => x.firstName,
      [this.translate.instant("Last name")]: (x: CoordinatorForCity) => x.lastName,
    };
    this.filteredCoordinators = SortHelper.sort(this.filteredCoordinators, $event, userSortConfig);
  }
}
