import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Clinic } from '../../../models/api/Clinic';
import { FacilityService } from '../../../services/data/facility.service';
import { DepartmentService } from '../../../services/data/department.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { ClinicService } from '../../../services/data/clinic.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-edit-a-clinic',
  templateUrl: './edit-a-clinic.component.html'
})
export class EditAClinicComponent implements OnInit, OnDestroy {

  @Input() clinic: Clinic;
  @Input() clinics: Clinic[] = [];
  clinicCopy: Clinic;
  departmentsSelection: DepartmentSelection[];
  UrlPaths = UrlPaths;

  clinicsList: Clinic[] = [];

  fawarningicon = faExclamationTriangle;

  constructor(
    private facilityService: FacilityService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService,
    private clinicService: ClinicService) { }

  ngOnInit(): void {
    if (this.clinic) {
      this.clinicCopy = JSON.parse(JSON.stringify(this.clinic));
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

    this.clinicService.getClinicsForFacility(this.clinicCopy.facilityId).subscribe((facility) => {
      this.clinicsList = facility;

      this.facilityService.getDepartments(this.clinicCopy.facilityId).subscribe(
        (departments) => {
          this.departmentsSelection = departments.map(a => (
            {
              department: a, isSelected: this.clinicCopy.departments.map(k => k.id).indexOf(a.id) !== -1,
              isAlreadyAtClinic: this.clinicsList.some(k => k.departments.some(av => av.id === a.id) && k.id !== this.clinicCopy.id)
            }));
        },
        (err) => this.toastrService.error(`Could not load clinics: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );

     });

    
  }

  canNotSaveClinic(): boolean {
    return this.canSaveClinic() === false;
  }

  canSaveClinic(): boolean {
    return this.clinicCopy.facilityId > 0
      && this.clinicCopy.name?.length > 0
      && this.clinics
                    .filter(cl => cl.id !== this.clinicCopy.id)
                    .find(cl => cl.name == this.clinicCopy.name) == undefined
      && this.departmentsSelection?.filter(r => r.isSelected)?.length > 0;
  }

  omitSpecialChar(event) {   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  saveClinic() {
    this.clinicCopy.departments = this.departmentsSelection.filter(m => m.isSelected).map(r => r.department);
    this.clinicService.updateClinic(this.clinicCopy).subscribe(
      (k) => {
        // Must replace values ​​on the original object to support updating the list when navigating back to the clinic overview
        this.clinic.name = k.name;
        this.clinic.facilityId = k.facilityId;
        this.clinic.departments = k.departments;
        this.toastrService.success('Clinic updated');
      },
      (err) => this.toastrService.error(`Technical error while updating: ${err?.message ? err.message : err}`, '', { disableTimeOut: true})
    );
  }
}
