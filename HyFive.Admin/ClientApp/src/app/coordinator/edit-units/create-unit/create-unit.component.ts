import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { FacilityService } from '../../../services/data/facility.service';
import { ToastrService } from 'ngx-toastr';
import { UnitService } from '../../../services/data/unit.service';
import { Unit } from '../../../models/api/Unit';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { DepartmentService } from '../../../services/data/department.service';
import { faInfoCircle } from '@fortawesome/free-solid-svg-icons';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-create-unit',
  templateUrl: './create-unit.component.html'
})
export class CreateUnitComponent implements OnInit, OnDestroy {

  newUnit: Unit;
  departmentsSelection: DepartmentSelection[] = [];

  unitsList: Unit[] = [];

  faInfoIcon = faInfoCircle;

  @Input() facilityId: number;
  @Output() unitCreatedEvent: EventEmitter<Unit> = new EventEmitter<Unit>();
  @Output() resetFormEvent: EventEmitter<boolean> = new EventEmitter<boolean>();



  constructor(
    private readonly facilityService: FacilityService,
    private readonly unitService: UnitService,
    private readonly departmentService: DepartmentService,
    private readonly toastrService: ToastrService,
    private readonly translate: TranslateService) { }

  ngOnInit(): void {
    this.resetForm();
    this.loadDepartments();
  }

  ngOnDestroy(): void {
    this.resetForm();
    this.toastrService.clear();
  }
  
  createUnit() {
    this.newUnit.name = this.sanitizeUnitName(this.newUnit.name);
    if (!this.newUnit.name) {
      return;
    }

    this.newUnit.departments = this.departmentsSelection
      .filter(r => r.isSelected)
      .map((r) => ({ id: r.department.id, departmentTypeId: 0, roles: null, facilityId: this.facilityId, name: null, departmentType: null }));

    this.unitService.createUnit(this.newUnit).subscribe((unit) => {
      this.toastrService.success(this.translate.instant('Unit created'), this.translate.instant('Unit with ID:') + ` ${unit.id} ` + this.translate.instant('created'));
      this.unitCreatedEvent.emit(unit);

      this.loadDepartments();
    },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while creating Unit. Error message from server:') + ` ${error?.message ? error.message : error}`, this.translate.instant('Error while creating unit'), { disableTimeOut: true}),
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
        (err) => this.toastrService.error(this.translate.instant('Could not load Units:') + ` ${err?.message ? err.message : err} `, this.translate.instant('Technical error'), { disableTimeOut: true})
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
      && !this.unitsList.some(cl => cl.name == this.newUnit.name)
      && this.newUnit.name?.length > 0;
  }

  omitSpecialChar(event) {   
    const char = event.key;

    // Allow letters, numbers, spaces, basic punctuation
    const allowed = /^[a-zA-Z0-9\s.,'-]$/;

    if (!allowed.test(char)) {
      event.preventDefault();
      return false;
    }

    return true;
  }

  sanitizeUnitName(value: string): string {
    if (!value) {
      return value;
    }

    // Remove emojis / pictographs / symbols (covers paste too)
    const noEmojis = value.replaceAll(
      /[\p{Extended_Pictographic}\p{Emoji_Presentation}\p{Symbol}]/gu,
      ''
    );

    return noEmojis.trim();
  }

  cancelForm() {
    this.resetFormEvent.emit(true);
  }
}
