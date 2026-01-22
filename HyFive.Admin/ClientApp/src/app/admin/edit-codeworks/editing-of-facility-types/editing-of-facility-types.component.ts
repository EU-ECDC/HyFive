import { Component, OnInit, OnDestroy } from '@angular/core';
import { FacilityType } from '../../../models/api/FacilityType';
import { ToastrService } from 'ngx-toastr';
import { FacilitiesTypesService } from '../../../services/data/FacilitiesTypes.service';
import { CreateFacilityTypeRequest } from 'src/app/models/api/CreateFacilityTypeRequest';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-facility-types',
  templateUrl: './editing-of-facility-types.component.html'
})
export class EditingFacilityTypesComponent implements OnInit, OnDestroy {

  facilityTypes: FacilityType[] = [];
  newFacilityType: CreateFacilityTypeRequest = this.emptyRequest();
  facilitytypeAsChanged: FacilityType = null;

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
    this.FacilitiesTypesService.createFacilityType(this.newFacilityType).subscribe(
      (createdFacilitytype) => this.toastrService.success(`Facility type created.`),
      error => this.toastrService.error(`An error occurred while creating the  type ${this.newFacilityType.name}. Error: "${error.error.message}"`, '', { disableTimeOut: true}),
      () => { this.newFacilityType = this.emptyRequest(); this.loadtFacilityTypes(); }
    );
  }

  selectedFacilitytype(facilitytype: FacilityType): void {
    if (this.facilitytypeAsChanged?.id == facilitytype.id) return;
    this.facilitytypeAsChanged = structuredClone(facilitytype);
  }

  updateFacilityType(facilitytype: FacilityType): void {
    this.FacilitiesTypesService.updateFacilityType(facilitytype).subscribe(
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
