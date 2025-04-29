import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Clinic } from '../../../models/api/Clinic';
import { InstitutionService } from '../../../services/data/institution.service';
import { DepartmentService } from '../../../services/data/department.service';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { DepartmentSelection } from '../../../models/code-work/departmentSelection.model';
import { ClinicService } from '../../../services/data/clinic.service';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-rediger-en-klinikk',
  templateUrl: './rediger-en-klinikk.component.html'
})
export class RedigerEnKlinikkComponent implements OnInit, OnDestroy {

  @Input() klinikk: Clinic;
  klinikkKopi: Clinic;
  departmentsSelection: DepartmentSelection[];
  UrlPaths = UrlPaths;

  clinicsList: Clinic[] = [];

  fawarningicon = faExclamationTriangle;

  constructor(
    private institutionService: InstitutionService,
    private departmentService: DepartmentService,
    private toastrService: ToastrService,
    private clinicService: ClinicService) { }

  ngOnInit(): void {
    if (this.klinikk) {
      this.klinikkKopi = JSON.parse(JSON.stringify(this.klinikk));
      this.loadDepartments();
    }
    else {
      this.toastrService.error('Departmentikke lastet', 'Technical error', { disableTimeOut: true});
    }
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadDepartments() {

    this.clinicService.getClinicsForInstitution(this.klinikkKopi.institutionId).subscribe((institution) => {
      this.clinicsList = institution;

      this.institutionService.getDepartments(this.klinikkKopi.institutionId).subscribe(
        (departments) => {
          this.departmentsSelection = departments.map(a => (
            {
              department: a, isSelected: this.klinikkKopi.departments.map(k => k.id).indexOf(a.id) !== -1,
              isAlreadyAtClinic: this.clinicsList.some(k => k.departments.some(av => av.id === a.id) && k.id !== this.klinikkKopi.id)
            }));
        },
        (err) => this.toastrService.error(`Could not load klinikker: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
      );

     });

    
  }

  kanIkkeLagreKlinikk(): boolean {
    return this.kanLagreKlinikk() === false;
  }

  kanLagreKlinikk(): boolean {
    return this.klinikkKopi.institutionId > 0
      && this.klinikkKopi.name?.length > 0
      && this.departmentsSelection?.filter(r => r.isSelected)?.length > 0;
  }

  lagreKlinikk() {
    this.klinikkKopi.departments = this.departmentsSelection.filter(m => m.isSelected).map(r => r.department);
    this.clinicService.updateClinic(this.klinikkKopi).subscribe(
      (k) => {
        // Må replace verdier på original-objektet for å støtte oppdatering av liste når en navigerer tilbake til klinikk-oversikt
        this.klinikk.name = k.name;
        this.klinikk.institutionId = k.institutionId;
        this.klinikk.departments = k.departments;
        this.toastrService.success('Clinic oppdatert');
      },
      (err) => this.toastrService.error(`Technical error ved oppdatering: ${err?.message ? err.message : err}`, '', { disableTimeOut: true})
    );
  }
}
