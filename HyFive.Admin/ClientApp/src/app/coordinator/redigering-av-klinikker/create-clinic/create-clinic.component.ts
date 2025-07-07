import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { InstitutionService } from '../../../services/data/institution.service';
import { ToastrService } from 'ngx-toastr';
import { ClinicService } from '../../../services/data/clinic.service';
import { Clinic } from '../../../models/api/Clinic';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { DepartmentService } from '../../../services/data/department.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-create-clinic',
  templateUrl: './create-clinic.component.html'
})
export class CreateClinicComponent implements OnInit, OnDestroy {

  newClinic: Clinic;
  departmentsSelection: DepartmentSelection[] = [];

  clinicsList: Clinic[] = [];

  fawarningicon = faExclamationTriangle;

  @Input() institutionId: number;
  @Output() clinicCreatedEvent: EventEmitter<Clinic> = new EventEmitter<Clinic>();


  constructor(
    private institutionService: InstitutionService,
    private clinicService: ClinicService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.resetForm();
    this.loadDepartments();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  createClinic() {
    this.newClinic.departments = this.departmentsSelection
      .filter(r => r.isSelected)
      .map((r) => ({ id: r.department.id, departmentTypeId: 0, roles: null, institutionId: this.institutionId, name: null, departmentType: null }));

    this.clinicService.createClinic(this.newClinic).subscribe((clinic) => {
      this.toastrService.success('Clinic created', `Clinic with ID: ${clinic.id} created`);
      this.clinicCreatedEvent.emit(clinic);

      this.loadDepartments();
    },
      (error) => this.toastrService.error(`An error occurred while creating Clinic. Error message from server: ${error?.message ? error.message : error}`, 'Error while creating clinic', { disableTimeOut: true}),
      () => { this.resetForm(); }
    );
  }

  loadDepartments() {

    this.clinicService.getClinicsForInstitution(this.institutionId).subscribe((result: Clinic[]) => {
      this.clinicsList = result;

      this.institutionService.getDepartments(this.institutionId).subscribe(
        (departments) => {
          this.departmentsSelection = departments.map(a =>
          ({
            department: a, isSelected: false,
            isAlreadyAtClinic: this.clinicsList.some(k => k.departments.some(av => av.id === a.id))
          }));
        },
        (err) => this.toastrService.error(`Could not load Clinics: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );
    });
  }

  resetForm() {
    this.newClinic = {
      id: 0,
      name: null,
      institutionId: this.institutionId,
      departments: []
    };
    for (const department of this.departmentsSelection) {
      department.isSelected = false;
    }
  }

  canNotCreateClinic(): boolean {
    return this.canCreateClinic() === false;
  }

  canCreateClinic(): boolean {
    return this.newClinic.institutionId > 0
      && this.departmentsSelection?.filter(r => r.isSelected)?.length > 0
      && this.clinicsList.find(cl => cl.name == this.newClinic.name) == undefined
      && this.newClinic.name?.length > 0;
  }

  omitSpecialChar(event) {   
    var k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }
}
