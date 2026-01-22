import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { IDropdownSettings } from "ng-multiselect-dropdown";
import { CoordinatorForCity } from "src/app/models/api/CoordinatorForCity";
import { FacilityReport } from "src/app/models/api/FacilityReport";
import { MailValidatorHelper } from "src/app/utils/mail-validator-helper";

@Component({
  selector: 'app-create-coordinator-for-city-form',
  templateUrl: './create-coordinator-for-city-form.component.html'
})
export class CreateCoordinatorForCityFormComponent implements OnInit, OnDestroy {

  @Input() coordinators: CoordinatorForCity[];
  @Input() facility: FacilityReport;
  @Input() facilitiesCity: FacilityReport[];
  @Output() CreateCoordinatorEvent: EventEmitter<CoordinatorForCity> = new EventEmitter<CoordinatorForCity>();
  @Output() CancelEditEvent = new EventEmitter();
  mailValidatorHelper;
  newCoordinator: CoordinatorForCity = null;
  dropdownSettings: IDropdownSettings;


  constructor(private readonly translate: TranslateService) {
    this.mailValidatorHelper = MailValidatorHelper;
  }

  ngOnInit(): void {
    this.createEmptyCoordinator();
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
    this.newCoordinator = null;
  }

  cancelEdit() {
    this.CancelEditEvent.emit();
  }

  createEmptyCoordinator() {
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


  canCreate() {
    return this.newCoordinator?.firstName.length > 0
      && this.newCoordinator?.lastName.length > 0
      && this.newCoordinator?.email.length > 0
      && this.mailValidatorHelper.validateMail(this.newCoordinator?.email)
      && !this.coordinators.some(coord => coord?.email == this.newCoordinator?.email)
      && this.newCoordinator?.facilities?.length > 0;
  }

  omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  createCoordinator() {
    this.CreateCoordinatorEvent.emit(this.newCoordinator);
    // console.log(this.newCoordinator)
  } 
}