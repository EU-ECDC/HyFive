import { Component, ElementRef, HostListener, Input, ViewChild } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { Department} from 'src/app/models/api/Department';
import { DepartmentType } from 'src/app/models/api/DepartmentType';
import { DownloadExcelModel } from 'src/app/models/api/downloadExcelModel';
import { InstitutionReport } from 'src/app/models/api/InstitutionReport';
import { InstitutionType } from 'src/app/models/api/InstitutionType';
import { Role } from 'src/app/models/api/Role';
import { SessionType } from 'src/app/models/api/SessionType';
import { DepartmentService } from 'src/app/services/data/department.service';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { ReportService } from 'src/app/services/data/report.service';
import { RoleService } from 'src/app/services/data/role.service';
import { DownloadFileHelper } from 'src/app/utils/download-file-helper';

@Component({
  selector: 'app-compliance-pdf',
  templateUrl: './compliance-pdf.component.html'
})
export class CompliancePdfComponent {

  @Input() sessionType: SessionType;

  selectedInstitutionId: number;
  // selectedInstitutionTypes: InstitutionType[] = [];
  selectedInstitutions: InstitutionReport[] = [];
  // selectedDepartmentTypes: DepartmentType[];
  selectedDepartments: Department[] = [];
  fromDate: Date = null;
  toDate: Date = null;
  institution: InstitutionReport;
  // institutionTypes: InstitutionType[];
  departments: Department[] = [];
  // departmentTypes: DepartmentType[] = [];
  allDepartments: Department[] = [];
  institutions: InstitutionReport[] = [];
  allInstitutions: InstitutionReport[] = [];
  canSelectInstitution = false;
  storedReport = false;
  createInstitutionalReport = false;

  private selectedRole: AuthorizedRole;

  dropdownSettings: IDropdownSettings;

  constructor(
    private institutionService: InstitutionService,
    private reportService: ReportService,
    private toastrService: ToastrService,
    private roleService: RoleService,
    private departmentService: DepartmentService,
    private authorizationService: AuthorizationService) { }

  @ViewChild('dropdownRef', { static: false }) dropdownRef: ElementRef;
  isDropdownFocused: boolean = false;
  
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
  const clickedInside = this.dropdownRef?.nativeElement.contains(event.target);
  
    if (clickedInside) {
      this.isDropdownFocused = true;
    } else if (this.isDropdownFocused) {
      this.isDropdownFocused = false;
      this.selectInstitution();
    }
  }

  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();
    
    if (this.selectedRole === AuthorizedRole.Coordinator) {
      // this.loadDepartmentTypes();
      this.canSelectInstitution = false;
      this.selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
      this.institutionService.getInstitution(this.selectedInstitutionId)
        .subscribe(
          (institution) => {
            this.institution = institution;
          }
        )
      this.loadCoordinatorInstitutionDepartments(this.selectedInstitutionId)
    }
    else if (this.selectedRole === AuthorizedRole.Administrator) {
      this.canSelectInstitution = true;
      // this.loadAdminInstitutionsTypes();
      this.institutionService.getInstitutions().subscribe(
        (institutions) => {
          this.institutions = institutions;
          this.allInstitutions = institutions;
        });
      }
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select all',
      unSelectAllText: 'Select all',
      itemsShowLimit: 3
    };
  }

  // filterInstitutionsByType() {
  //   this.selectedInstitutions = [];
  //   this.selectedDepartments = [];
  //   this.selectedDepartmentTypes = [];
  //   if (this.selectedInstitutionTypes?.length > 0) {
  //     this.institutions =  this.allInstitutions?.filter(item => this.selectedInstitutionTypes.some(si => si.id == item.institutionType.id));
  //   } else {
  //     this.institutions = this.allInstitutions;
  //   }
  // }

  // filterDepartmentsByType() {
  //   this.selectedDepartments = [];
  //   if (this.selectedDepartmentTypes?.length > 0) {
  //     console
  //     this.departments =  this.allDepartments.filter(item => this.selectedDepartmentTypes.some(sd => sd.id == item.departmentTypeId));
  //   } else {
  //     this.departments = this.allDepartments;
  //   }
  // }

  onChangeModelInstitution() {
    if (this.selectedInstitutions.length > 0) {
      if (
        // this.departmentTypes.length > 0 && 
        this.allDepartments.length > 0) {
        this.allDepartments = this.allDepartments.filter(dep => this.selectedInstitutions.some(inst => inst.id == dep.institutionId));
        this.departments = this.allDepartments;
        // this.departmentTypes = Array.from(
        //                         new Map(this.allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values());
      }
      this.selectedDepartments = [];
      // this.selectedDepartmentTypes = [];
    } else {
      this.resetDepartments();
    }
  }

  resetDepartments() {
    this.departments = [];
    this.allDepartments = [];
    // this.departmentTypes = [];
    this.selectedDepartments = [];
    // this.selectedDepartmentTypes = [];
  }
      
  selectInstitution(): void {
    this.departments = [];
    this.allDepartments = [];
    // this.selectedDepartmentTypes = [];
    this.selectedDepartments = [];
    if (this.selectedInstitutions != null && this.selectedInstitutions?.length > 0) {
      var institutionIds = this.selectedInstitutions?.map(inst => inst.id);
      this.loadInstitutionsDepartments(institutionIds)
    }
  };

  reset(): void {
    this.selectedDepartments = [];
    // this.selectedDepartmentTypes = [];
    this.departments = [];
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedInstitutionId = null;
    }
  }

  canCreateReport() {
    return (this.selectedDepartments.length > 0 && this.fromDate && this.toDate);
  }

  saveReport() {
    this.toastrService.clear();

    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    // const institutionTypeIds = this.selectedInstitutionTypes?.map(t => t.id) ?? [];
    const institutionIds = this.selectedInstitutionId ? [this.selectedInstitutionId] : this.selectedInstitutions?.map(t => t.id) ?? [];
    // const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];

    this.reportService.reportForSessionTypeHasData(
      {
      sessionType: this.sessionType,
      // institutionTypeIds,
      institutionIds,
      // departmentTypeIds,
      departmentIds,
      fromDate: this.fromDate, 
      toDate: this.toDate,
      roleId: this.selectedRole
      }).subscribe(
        reportHasData => {
          if (reportHasData) {
            this.storedReport = true;
            this.downloadPdf().subscribe(() => {
              this.storedReport = false
            },
              (error) => {
                this.storedReport = false
                this.toastrService.error(error?.message ? error.message : error, 'Error while downloading report', { disableTimeOut: true });
              });
          } else {
            // this.toastrService.info('There are no observations for selected values', '', { positionClass: 'toast-center-center' });
            this.toastrService.info('There are no observations for selected values', '');
          }
        })
  }

  private downloadPdf(): Observable<any> {
    let url = '/api/v1/report/';

    if (this.sessionType == SessionType.FiveIndications) {
      url += 'fiveindications';
    } else if (this.sessionType == SessionType.HandJewelry) {
      url += 'handjewelry';
    }

    url += '/department/pdf/';
    // url += `?fromDate=${this.fromDate}&toDate=${this.toDate}`;
    // url += `&role=${this.selectedRole}`;
    // this.selectedInstitutions.forEach(inst =>  {url += `&institutionId=${inst.id}`});
    // this.selectedDepartments.forEach(dep => { url += `&departmentId=${dep.id}`});
    // url += `&institutionId=${this.selectedInstitutionId}&departmentId=${this.selectedDepartmentId}`;
    const payload: DownloadExcelModel = {
      departmentIds:  this.selectedDepartments?.map(dep => dep.id) ?? [],
      institutionIds: this.selectedInstitutionId ? [this.selectedInstitutionId] : this.selectedInstitutions?.map(t => t.id) ?? [],
      fromDate: this.fromDate,
      toDate: this.toDate,
      role: this.selectedRole
    } 

    return DownloadFileHelper.downloadFile(url, 'application/pdf, */*', payload);
  }

  // loadAdminInstitutionsTypes() {
  //   this.institutionService.getInstitutionTypes().subscribe((result) => {
  //     this.institutionTypes = result,
  //     (error) => this.toastrService.error('An error occurred while loading institution types: ' + error?.message, '', { disableTimeOut: true })
  //   });
  // }

  // loadDepartmentTypes() {
  //   this.departmentService.getDepartmentTypes().subscribe(
  //     (departmentTypes) => {
  //       this.departmentTypes = departmentTypes;
  //     },
  //     (error) => {
  //       this.toastrService.error(error.error.message, 'Loading departmentType failed', {disableTimeOut: true});
  //   });
  // }

  loadCoordinatorInstitutionDepartments(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
        this.allDepartments = this.departments;
        // this.selectedInstitutionTypes.push(institution.institutionType);
      })
  };

  loadInstitutionsDepartments(institutionIds: number[]) {
  this.institutionService.getComplianceInstitutions(institutionIds).subscribe(institutions => {
    const allDepartments = institutions.reduce((all, inst) => {
      return all.concat(inst.departments);
    }, []);

    const uniqueDepartments = Array.from(
      new Map(allDepartments.map(dep => [dep.id, dep])).values()
    );

    const uniqueDepartmentTypes = Array.from(
      new Map(allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values()
    );

      // this.departmentTypes = uniqueDepartmentTypes;
      this.departments = uniqueDepartments;
      this.allDepartments = uniqueDepartments;
    });
  }
}
