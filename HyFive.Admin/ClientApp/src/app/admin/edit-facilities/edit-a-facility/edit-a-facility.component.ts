import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FacilityService } from '../../../services/data/facility.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { CityService } from 'src/app/services/data/City.service';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';
import { UpdateFacilityRequest } from 'src/app/models/api/UpdateFacilityRequest';
import { City } from 'src/app/models/api/City';

@Component({
  selector: 'app-edit-a-facility',
  templateUrl: './edit-a-facility.component.html'
})
export class EditFacilityComponent implements OnInit {

  constructor(private readonly facilityService: FacilityService,
              private readonly toastrService: ToastrService,
              private readonly cityService: CityService,
              private readonly translate: TranslateService) { }

  facility: OrganisationUnit = null;
  facilityTypes: OrganisationUnitType[] = [];
  facilitytypeId = 0;
  listOfCities: City[] = [];

  UrlPaths = UrlPaths;

  @Input() facilityId: number;
  @Input() facilities: OrganisationUnit[] = [];
  @Output() facilityDeletedEvent: EventEmitter<number> = new EventEmitter<number>();
  @Output() facilityUpdatedEvent: EventEmitter<OrganisationUnit> = new EventEmitter<OrganisationUnit>();

  ngOnInit(): void {
    if (this.facilityId === 0) {
      this.facility = null;
      return;
    }
    this.facilityService.getFacility(this.facilityId).subscribe((facility) => {
      this.facility = facility;
      this.facilitytypeId = facility.type.id;
      
      this.facilityService.getFacilityTypes().subscribe((types) => {
        this.facilityTypes = types;
      });
    });

        this.cityService.getAllCities().subscribe(
      (allCities) => {
        // this.listOfCities = [ { id: 0, name: null }, ...allCities];
        allCities.forEach(item => {
        this.listOfCities.push({id: item.id, name: item.name})
        });
    });
  }

  deleteFacility() {
      if (this.facilityId > 0) {
        this.facilityService.deleteFacility(this.facilityId).subscribe(() => {
          this.facility = null;
          this.facilityDeletedEvent.emit(this.facilityId);
        },
          (error =>
            this.toastrService.error(this.translate.instant('An error occurred:') + ` ${error?.error.message}`, this.translate.instant('Error while deleting facility'), { disableTimeOut: true})));
      }
  }

  facilityTypeChanged() {
    this.facility.type = this.facilityTypes.find(i => i.id === this.facilitytypeId);
  }

  saveFacility() {
    const updateFacilityRequest: UpdateFacilityRequest = {
        id: this.facilityId,
        name: this.facility.name,
        abbreviation: this.facility.abbreviation,
        description: this.facility.description,
        organisationUnitTypeId: this.facility.type.id,
        address: {
          	id: this.facility.address.id,
            city: this.listOfCities.find(city => city.id == this.facility.address.cityId),
            cityId: this.facility.address.cityId,
            street: this.facility.address.street,
            postalCode: this.facility.address.postalCode
        }
    }
    this.facilityService.updateFacility(updateFacilityRequest).subscribe(
      (facility) => {
        this.toastrService.success(this.translate.instant('The facility was updated'));
        this.facilityUpdatedEvent.emit(facility);
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred:') +  ` ${error?.error.message}`, this.translate.instant('Error during update'), { disableTimeOut: true})
    );
  }

  canNotSavefacility(): boolean {
    if (this.facility.type.id > 0 
      && this.facility.name?.length > 0
      && !this.facilities.filter(i => i.id !== this.facility.id).some(i => i.name === this.facility.name)
    )
      return false;
    else
      return true;
  }

  omitSpecialChar(event){   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }
}

