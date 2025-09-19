import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FacilityService } from '../../../services/data/facility.service';
import { Facility } from '../../../models/api/Facility';
import { FacilityType } from '../../../models/api/FacilityType';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/konstanter/url-paths';
import { HealthcareOrganization } from 'src/app/models/api/HealthcareOrganization';
import { HealthcareOrganizationService } from 'src/app/services/data/healthcareOrganization.service';
import { MunicipalityService } from 'src/app/services/data/municipality.service';
import { Municipality } from 'src/app/models/api/Municipality';

@Component({
  selector: 'app-edit-a-facility',
  templateUrl: './edit-a-facility.component.html'
})
export class EditFacilityComponent implements OnInit {

  constructor(private facilityService: FacilityService,
              private toastrService: ToastrService,
              private municipalityService: MunicipalityService,
              private healthcareOrganizationService: HealthcareOrganizationService) { }

  facility: Facility = null;
  facilityTypes: FacilityType[] = [];
  facilitytypeId = 0;
  listOfHealthcareOrganizations: HealthcareOrganization[] = [];

  municipality: Municipality = null;
  municipalities: Municipality[];
  municipalityId = 0;
  UrlPaths = UrlPaths;
  healthcareOrganizationId = 0;


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
      this.municipalityId = facility.municipality?.id;
      this.healthcareOrganizationId = facility.healthcareOrganization?.id;
      
      this.facilityService.getFacilityTypes().subscribe((types) => {
        this.facilityTypes = types;
      });
    });

    this.municipalityService.getMunicipalities().subscribe(
      (municipalities) => {
        this.municipalities = [ {id: 0, number: null, name: null}, ...municipalities];
    });

    this.healthcareOrganizationService.getAllHealthcareOrganizations().subscribe(
      (allHealthcareorganization) => {
        this.listOfHealthcareOrganizations = [ { id: 0, name: null, regionalHealthcareOrganization: null, regionalHealthcareOrganizationId: null }, ...allHealthcareorganization];
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
    this.facility.healthcareOrganization = null;
    this.facility.municipality = null;
    this.healthcareOrganizationId = 0;
    this.municipalityId = 0;
    this.facility.facilityType = this.facilityTypes.find(i => i.id === this.facilitytypeId);
  }

  municipalityChanged() {
      this.healthcareOrganizationId = 0;
      this.facility.healthcareOrganization = null;
      if (this.municipalityId == 0) {
        this.facility.municipality = null;
      } else {
        this.facility.municipality = this.municipalities.find(r => r.id === this.municipalityId);
      }
  }

  saveFacility() {
    this.facilityService.updateFacility(this.facility).subscribe(
      (facility) => {
        this.toastrService.success('The facility was updated');
        this.facilityUpdatedEvent.emit(facility);
      },
      (error) => this.toastrService.error(`An error occurred: ${error?.error} / ${error?.message}`, 'Error during update', { disableTimeOut: true}));
  }

  healthEnterpriseChanged() {
      this.municipalityId = 0;
      this.facility.municipality = null;
      if (this.healthcareOrganizationId == 0) {
        this.facility.healthcareOrganization = null;
      } else {
        this.facility.healthcareOrganization = this.listOfHealthcareOrganizations.find(r => r.id === this.healthcareOrganizationId);
      }
  }

  canNotSavefacility(): boolean {
    if (this.facility.facilityType.id > 0 
      && this.facility.name?.length > 0
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

