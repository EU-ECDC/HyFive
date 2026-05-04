import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import { IDropdownSettings } from "ng-multiselect-dropdown";
import { User } from "src/app/models/api/User";
import { MailValidatorHelper } from "src/app/utils/mail-validator-helper";

@Component({
  selector: 'app-create-coordinator-form',
  templateUrl: './create-coordinator-form.component.html'
})
export class CreateCoordinatorFormComponent implements OnInit, OnDestroy {

  @Input() coordinators: User[];
  @Input() facilityId: number = 0;
  @Output() CreateCoordinatorEvent: EventEmitter<User> = new EventEmitter<User>();
  @Output() CancelCreateEvent = new EventEmitter();
  mailValidatorHelper;
  newCoordinator: User = null;
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
    this.CancelCreateEvent.emit();
  }

  createEmptyCoordinator() {
    this.newCoordinator = {
      id: 0,
      // facilityId: this.facilityId,
      lastName: '',
      firstName: '',
      email: '',
      identityPseudonym: null,
      createdTime: new Date(),
      isDeactivated: false,
      userPermissions: null,
      userIdentifiers: null
    };
  }


  canBeCreated() {
    return this.newCoordinator.firstName.length > 0
      && this.newCoordinator.lastName.length > 0
      //&& this.coordinators.find(fc => fc.firstName == this.newCoordinator?.firstName && fc.lastName == this.newCoordinator?.lastName) == undefined
      && !this.coordinators.some(fc => fc.email == this.newCoordinator?.email)
      && this.newCoordinator.email?.length > 0
      && this.mailValidatorHelper.validateMail(this.newCoordinator.email);
  }

  omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  createCoordinator() {
    this.CreateCoordinatorEvent.emit(this.newCoordinator);
  } 
}