import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FacilityService } from '../../../services/data/facility.service';
import { Facility } from '../../../models/api/Facility';
import { FacilityType } from '../../../models/api/FacilityType';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { City } from 'src/app/models/api/City';
import { CityService } from 'src/app/services/data/City.service';

@Component({
  selector: 'app-edit-a-facility',
  templateUrl: './edit-a-facility.component.html'
})
export class EditFacilityComponent implements OnInit {

  constructor(private facilityService: FacilityService,
              private toastrService: ToastrService,
              private cityService: CityService) { }

  facility: Facility = null;
  facilityTypes: FacilityType[] = [];
  facilitytypeId = 0;
  listOfCities: City[] = [];

  UrlPaths = UrlPaths;
  cityId = 0;


  @Input() facilityId: number;
  @Input() facilities: Facility[] = [];
  @Output() facilityDeletedEvent: EventEmitter<number> = new EventEmitter<number>();
  @Output() facilityUpdatedEvent: EventEmitter<Facility> = new EventEmitter<Facility>();

  ngOnInit(): void {
    if (this.facilityId === 0) {
      this.facility = null;
      return;
    }
    this.facilityService.getFacility(this.facilityId).subscribe((facility) => {
      this.facility = facility;
      this.facilitytypeId = facility.facilityType.id;
      this.cityId = facility.city?.id;
      
      this.facilityService.getFacilityTypes().subscribe((types) => {
        this.facilityTypes = types;
      });
    });

    this.cityService.getAllCities().subscribe(
      (allCities) => {
        this.listOfCities = [ { id: 0, name: null }, ...allCities];
    });
  }

  deleteFacility() {
      if (this.facilityId > 0) {
        this.facilityService.deleteFacility(this.facilityId).subscribe(() => {
          this.facility = null;
          this.facilityDeletedEvent.emit(this.facilityId);
        },
          (error =>
            this.toastrService.error(`An error occurred: ${error?.error} / ${error?.message}`, 'Error while deleting facility', { disableTimeOut: true})));
      }
  }

  facilityTypeChanged() {
    this.facility.city = null;
    this.cityId = 0;
    this.facility.facilityType = this.facilityTypes.find(i => i.id === this.facilitytypeId);
  }

  saveFacility() {
    this.facilityService.updateFacility(this.facility).subscribe(
      (facility) => {
        this.toastrService.success('The facility was updated');
        this.facilityUpdatedEvent.emit(facility);
      },
      (error) => this.toastrService.error(`An error occurred: ${error?.error} / ${error?.message}`, 'Error during update', { disableTimeOut: true}));
  }

  cityChanged() {
      if (this.cityId == 0) {
        this.facility.city = null;
      } else {
        this.facility.city = this.listOfCities.find(r => r.id === this.cityId);
      }
  }

  canNotSavefacility(): boolean {
    if (this.facility.facilityType.id > 0 
      && this.facility.name?.length > 0
      && this.facility.city?.id > 0
      && this.facilities.filter(i => i.id !== this.facility.id).find(i => i.name === this.facility.name) == undefined
    )
      return false;
    else
      return true;
  }

  omitSpecialChar(event){   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }
}

