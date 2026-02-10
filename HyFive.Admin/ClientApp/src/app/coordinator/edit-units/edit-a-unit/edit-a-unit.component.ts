import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Unit } from '../../../models/api/Unit';
import { FacilityService } from '../../../services/data/facility.service';
import { DepartmentService } from '../../../services/data/department.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { UnitService } from '../../../services/data/unit.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-edit-a-unit',
  templateUrl: './edit-a-unit.component.html'
})
export class EditAUnitComponent implements OnInit, OnDestroy {

  @Input() unit: Unit;
  @Input() units: Unit[] = [];
  unitCopy: Unit;
  departmentsSelection: DepartmentSelection[];
  UrlPaths = UrlPaths;

  unitsList: Unit[] = [];

  fawarningicon = faExclamationTriangle;

  constructor(
    private readonly facilityService: FacilityService,
    private readonly departmentService: DepartmentService,
    private readonly toastrService: ToastrService,

    private readonly unitService: UnitService,
    private readonly translate: TranslateService) { }

  ngOnInit(): void {
    if (this.unit) {
      this.unitCopy = structuredClone(this.unit);
      this.loadDepartments();
    }
    else {
      this.toastrService.error('Department not loaded', 'Technical error', { disableTimeOut: true});
    }
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadDepartments() {

    this.unitService.getUnitsForFacility(this.unitCopy.facilityId).subscribe((facility) => {
      this.unitsList = facility;

      this.facilityService.getDepartments(this.unitCopy.facilityId).subscribe(
        (departments) => {
          this.departmentsSelection = departments.map(a => (
            {
              department: a, isSelected: this.unitCopy.departments.map(k => k.id).indexOf(a.id) !== -1,
              isAlreadyAtUnit: this.unitsList.some(k => k.departments.some(av => av.id === a.id) && k.id !== this.unitCopy.id)
            }));
        },
        (err) => this.toastrService.error(`Could not load units: ${err?.eeror.message ? err.error.message : err}`, 'Technical error', { disableTimeOut: true})
      );

     });

    
  }

  canNotSaveUnit(): boolean {
    return this.canSaveUnit() === false;
  }

  canSaveUnit(): boolean {
    return this.unitCopy.facilityId > 0
      && this.unitCopy.name?.length > 0
      && !this.units
                    .filter(cl => cl.id !== this.unitCopy.id)
                    .some(cl => cl.name == this.unitCopy.name)
      && this.departmentsSelection?.filter(r => r.isSelected)?.length > 0;
  }

  omitSpecialChar(event: KeyboardEvent): boolean {
    const char = event.key;
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

    const noEmojis = value.replaceAll(
      /[\p{Extended_Pictographic}\p{Emoji_Presentation}\p{Symbol}]/gu,
      ''
    );

    return noEmojis.trim();
  }

  saveUnit() {
    this.unitCopy.name = this.sanitizeUnitName(this.unitCopy.name);

    if (!this.unitCopy.name) {
      return;
    }

    this.unitCopy.departments = this.departmentsSelection.filter(m => m.isSelected).map(r => r.department);
    this.unitService.updateUnit(this.unitCopy).subscribe(
      (k) => {
        // Must replace values ​​on the original object to support updating the list when navigating back to the unit overview
        this.unit.name = k.name;
        this.unit.facilityId = k.facilityId;
        this.unit.departments = k.departments;
        this.toastrService.success(this.translate.instant('Unit updated'));
      },
      (err) => this.toastrService.error(`Technical error while updating: ${err?.error.message ? err.error.message : err}`, '', { disableTimeOut: true})
    );
  }
}

