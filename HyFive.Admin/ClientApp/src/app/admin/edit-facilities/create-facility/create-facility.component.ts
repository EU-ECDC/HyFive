import {Component, EventEmitter, OnInit, Output, OnDestroy, Input} from '@angular/core';
import { FacilityType } from '../../../models/api/FacilityType';
import { CreateFacilityRequest } from '../../../models/api/CreateFacilityRequest';
import { FacilityService } from '../../../services/data/facility.service';
import { ToastrService } from 'ngx-toastr';
import { Facility } from '../../../models/api/Facility';
import { City } from 'src/app/models/api/City';
import { CityService } from 'src/app/services/data/City.service';
import { FacilityTypeConstants } from 'src/app/models/api/FacilityTypeConstants';
import { User } from 'src/app/models/api/User';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';
import { TranslateService } from '@ngx-translate/core';
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';

@Component({
  selector: 'app-create-facility',
  templateUrl: './create-facility.component.html'
})
export class CreateFacilityComponent implements OnInit, OnDestroy {

  facilityTypes: FacilityType[] = [];
  newfacility: CreateFacilityRequest = null;
  listOfCities: City[] = [];
  mailValidatorHelper;

  @Input() facilities: Facility[] = [];
  @Input() coordinators: User[] = [];
  @Output() facilityCreatedEvent: EventEmitter<Facility> = new EventEmitter<Facility>();
  @Output() resetFormEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(private readonly facilityService: FacilityService,
              private readonly toastrService: ToastrService,
              private readonly cityService: CityService,
              private readonly translate: TranslateService,
              private readonly dialogMessageService: DialogMessageService) {
                this.mailValidatorHelper = MailValidatorHelper;
               }

  ngOnInit(): void {
    this.facilityService.getFacilityTypes().subscribe((result) => {
      this.facilityTypes = result;
      this.newfacility = this.createDefaultFacility();
    });

    this.cityService.getAllCities().subscribe(
      (allCities) => {
        this.listOfCities = allCities;
      }
    );
  }

  ngOnDestroy(): void {
    this.newfacility = this.createDefaultFacility();
    this.toastrService.clear();
  }

  findDefaultFacilityType() {
    return this.facilityTypes.find(p => p.code === FacilityTypeConstants.PrimaryCare);
  }

  createFacility() {
    this.facilityService.createFacility(this.newfacility).subscribe((result) => {
        this.toastrService.success(this.translate.instant('Facility created'), `${this.translate.instant('Facility with ID:')} ${result.id} ${this.translate.instant('created')}`);
        this.facilityCreatedEvent.emit(result);
        this.cancelForm();
      },
        (err) => this.toastrService.error(`${ this.translate.instant('An error occurred while creating the healthcare facility. Error message from server:')} ${err.error.message}`, this.translate.instant('Error creating healthcare facility'), { disableTimeOut: true }),
      () => {
        this.newfacility = this.createDefaultFacility();

      }
    );
  }

 private createDefaultFacility(): CreateFacilityRequest {
    let defaultFacilityType = this.findDefaultFacilityType();
    return {
      facilityName: null,
      facilityTypeId: defaultFacilityType.id,
      coordinatorLastName: null,
      coordinatorFirstName: null,
      coordinatorHPRNumber: null,
      coordinatorEmail: null,
      coordinatorPseudonym: null,
      herId: null,
      abbreviation: null,
      cityId: 0
    };    
  }

  canNotCreateFacility(): boolean {
    return this.canCreateFacility() === false;
  }

  canCreateFacility(): boolean{
    return this.newfacility?.facilityName?.length > 0
      && this.newfacility.cityId > 0
      && this.newfacility?.coordinatorFirstName?.length > 0
      && this.newfacility?.coordinatorLastName?.length > 0
      && !this.coordinators.some(fc => fc.email == this.newfacility?.coordinatorEmail)
      && this.newfacility?.coordinatorEmail?.length > 0
      && this.mailValidatorHelper.validateMail(this.newfacility?.coordinatorEmail)
      && !this.facilities.some(i => i.name === this.newfacility.facilityName);
  }

  omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  cancelForm() {
    this.resetFormEvent.emit(true);
  }
}
