import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { FacilityService } from '../../../services/data/facility.service';
import { ToastrService } from 'ngx-toastr';
import { UnitService } from '../../../services/data/unit.service';
import { Unit } from '../../../models/api/Unit';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { DepartmentService } from '../../../services/data/department.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-create-unit',
  templateUrl: './create-unit.component.html'
})
export class CreateUnitComponent implements OnInit, OnDestroy {

  newUnit: Unit;
  departmentsSelection: DepartmentSelection[] = [];

  unitsList: Unit[] = [];

  fawarningicon = faExclamationTriangle;

  @Input() facilityId: number;
  @Output() unitCreatedEvent: EventEmitter<Unit> = new EventEmitter<Unit>();


  constructor(
    private facilityService: FacilityService,
    private unitService: UnitService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.resetForm();
    this.loadDepartments();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  createUnit() {
    this.newUnit.departments = this.departmentsSelection
      .filter(r => r.isSelected)
      .map((r) => ({ id: r.department.id, departmentTypeId: 0, roles: null, facilityId: this.facilityId, name: null, departmentType: null }));

    this.unitService.createUnit(this.newUnit).subscribe((unit) => {
      this.toastrService.success('Unit created', `Unit with ID: ${unit.id} created`);
      this.unitCreatedEvent.emit(unit);

      this.loadDepartments();
    },
      (error) => this.toastrService.error(`An error occurred while creating Unit. Error message from server: ${error?.message ? error.message : error}`, 'Error while creating unit', { disableTimeOut: true}),
      () => { this.resetForm(); }
    );
  }

  loadDepartments() {

    this.unitService.getUnitsForFacility(this.facilityId).subscribe((result: Unit[]) => {
      this.unitsList = result;

      this.facilityService.getDepartments(this.facilityId).subscribe(
        (departments) => {
          this.departmentsSelection = departments.map(a =>
          ({
            department: a, isSelected: false,
            isAlreadyAtUnit: this.unitsList.some(k => k.departments.some(av => av.id === a.id))
          }));
        },
        (err) => this.toastrService.error(`Could not load Units: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );
    });
  }

  resetForm() {
    this.newUnit = {
      id: 0,
      name: null,
      facilityId: this.facilityId,
      departments: []
    };
    for (const department of this.departmentsSelection) {
      department.isSelected = false;
    }
  }

  canNotCreateUnit(): boolean {
    return this.canCreateUnit() === false;
  }

  canCreateUnit(): boolean {
    return this.newUnit.facilityId > 0
      && this.departmentsSelection?.filter(r => r.isSelected)?.length > 0
      && this.unitsList.find(cl => cl.name == this.newUnit.name) == undefined
      && this.newUnit.name?.length > 0;
  }

  omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }
}
