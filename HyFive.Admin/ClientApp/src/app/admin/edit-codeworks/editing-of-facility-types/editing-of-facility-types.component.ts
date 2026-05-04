import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FacilitiesTypesService } from '../../../services/data/FacilitiesTypes.service';
import { CreateFacilityTypeRequest } from 'src/app/models/api/CreateFacilityTypeRequest';
import { KeyEventService } from '../../../services/events/key-event.service';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';

@Component({
  selector: 'app-editing-of-facility-types',
  templateUrl: './editing-of-facility-types.component.html'
})
export class EditingFacilityTypesComponent implements OnInit, OnDestroy {

  facilityTypes: OrganisationUnitType[] = [];
  newFacilityType: CreateFacilityTypeRequest = this.emptyRequest();
  facilitytypeAsChanged: OrganisationUnitType = null;

  constructor(
    private readonly FacilitiesTypesService: FacilitiesTypesService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadtFacilityTypes();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadtFacilityTypes() {
    this.FacilitiesTypesService.getFacilityTypes().subscribe(
      (result) => this.facilityTypes = result,
      (error) => this.toastrService.error('An error occurred while loading Facility type: ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): CreateFacilityTypeRequest {
    return {
      code: null,
      name: null
    }
  }

  createFacilityType(): void {
    if (!this.newFacilityType.code.startsWith("F_")) {
      this.newFacilityType = {...this.newFacilityType, code: 'F_' + this.newFacilityType.code};
    }
    this.FacilitiesTypesService.createFacilityType(this.newFacilityType).subscribe(
      (createdFacilitytype) => this.toastrService.success(`Facility type created.`),
      error => this.toastrService.error(`An error occurred while creating the  type ${this.newFacilityType.name}. Error: "${error.error.message}"`, '', { disableTimeOut: true}),
      () => { this.newFacilityType = this.emptyRequest(); this.loadtFacilityTypes(); }
    );
  }

  selectedFacilitytype(facilitytype: OrganisationUnitType): void {
    if (this.facilitytypeAsChanged?.id == facilitytype.id) return;
    this.facilitytypeAsChanged = structuredClone(facilitytype);
  }

  updateFacilityType(facilityType: OrganisationUnitType): void {
    if (!facilityType.code.startsWith("F_")) {
      facilityType = {...facilityType, code: 'F_' + facilityType.code};
    }
    this.FacilitiesTypesService.updateFacilityType(facilityType).subscribe(
      (updatedFacilitytype) => {
        this.toastrService.success("Facility type updated");
        this.loadtFacilityTypes();
      },
      error => this.toastrService.error('An error occurred while updating Facilitytype: ' + error?.error.message, '', { disableTimeOut: true}),
      () => this.facilitytypeAsChanged = null
    );
  }

  deleteFacilityType(facilitytypeId: number) {
    this.FacilitiesTypesService.deleteFacilityType(facilitytypeId).subscribe(
      (isDeleted) => {
        this.toastrService.success("Facility type was deleted");
        this.facilitytypeAsChanged = null;
        this.loadtFacilityTypes();
      }
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.facilitytypeAsChanged = null;
  }
}
