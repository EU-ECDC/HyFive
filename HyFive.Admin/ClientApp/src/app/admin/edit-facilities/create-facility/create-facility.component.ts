import {Component, EventEmitter, OnInit, Output, OnDestroy, Input} from '@angular/core';
import { CreateFacilityRequest } from '../../../models/api/CreateFacilityRequest';
import { FacilityService } from '../../../services/data/facility.service';
import { ToastrService } from 'ngx-toastr';
import { CityService } from 'src/app/services/data/City.service';
import { FacilityTypeConstants } from 'src/app/models/api/FacilityTypeConstants';
import { User } from 'src/app/models/api/User';
import { MailValidatorHelper } from 'src/app/utils/mail-validator-helper';
import { TranslateService } from '@ngx-translate/core';
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { City } from 'src/app/models/api/City';


@Component({
  selector: 'app-create-facility',
  templateUrl: './create-facility.component.html'
})
export class CreateFacilityComponent implements OnInit, OnDestroy {

  facilityTypes: OrganisationUnitType[] = [];
  newfacility: CreateFacilityRequest = null;
  listOfCities: City[] = [];
  // listOfCities: {id: string,name: string}[] = [];

  mailValidatorHelper;

  @Input() facilities: OrganisationUnit[] = [];
  @Input() coordinators: User[] = [];
  @Output() facilityCreatedEvent: EventEmitter<OrganisationUnit> = new EventEmitter<OrganisationUnit>();
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
        // this.listOfCities = allCities;
        allCities.forEach(item => {
          this.listOfCities.push({id: item.id, name: item.name});
        });
      }
    );
  }

  ngOnDestroy(): void {
    this.newfacility = this.createDefaultFacility();
    this.toastrService.clear();
  }

  findDefaultFacilityType() {
    // return this.facilityTypes.find(p => p.code === FacilityTypeConstants.PrimaryCare);
    return this.facilityTypes[0];
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
      name: null,
      abbreviation: null,
      description: null,
      organisationUnitTypeId: defaultFacilityType.id,
      cityId: null,
	    street: null,
	    postalCode: null,
      firstName: null,
      lastName: null,
      email: null,
      pseudonym: null
    };    
  }

  canNotCreateFacility(): boolean {
    return this.canCreateFacility() === false;
  }

  canCreateFacility(): boolean{
    return this.newfacility?.name?.length > 0
      && this.newfacility.cityId !== null
      && this.newfacility?.firstName?.length > 0
      && this.newfacility?.lastName?.length > 0
      // && !this.coordinators.some(fc => fc.email == this.newfacility?.email)
      && this.newfacility?.email?.length > 0
      && this.mailValidatorHelper.validateMail(this.newfacility?.email)
      && !this.facilities.some(i => i.name === this.newfacility.name);
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
