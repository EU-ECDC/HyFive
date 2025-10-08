import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Unit } from '../../../models/api/Unit';
import { FacilityService } from '../../../services/data/facility.service';
import { DepartmentService } from '../../../services/data/department.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { UnitService } from '../../../services/data/unit.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

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
    private facilityService: FacilityService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService,
    private unitService: UnitService) { }

  ngOnInit(): void {
    if (this.unit) {
      this.unitCopy = JSON.parse(JSON.stringify(this.unit));
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
        (err) => this.toastrService.error(`Could not load units: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );

     });

    
  }

  canNotSaveUnit(): boolean {
    return this.canSaveUnit() === false;
  }

  canSaveUnit(): boolean {
    return this.unitCopy.facilityId > 0
      && this.unitCopy.name?.length > 0
      && this.units
                    .filter(cl => cl.id !== this.unitCopy.id)
                    .find(cl => cl.name == this.unitCopy.name) == undefined
      && this.departmentsSelection?.filter(r => r.isSelected)?.length > 0;
  }

  omitSpecialChar(event) {   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  saveUnit() {
    this.unitCopy.departments = this.departmentsSelection.filter(m => m.isSelected).map(r => r.department);
    this.unitService.updateUnit(this.unitCopy).subscribe(
      (k) => {
        // Must replace values ​​on the original object to support updating the list when navigating back to the unit overview
        this.unit.name = k.name;
        this.unit.facilityId = k.facilityId;
        this.unit.departments = k.departments;
        this.toastrService.success('Unit updated');
      },
      (err) => this.toastrService.error(`Technical error while updating: ${err?.message ? err.message : err}`, '', { disableTimeOut: true})
    );
  }
}
