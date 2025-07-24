import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { Department} from 'src/app/models/api/Department';
import { DownloadExcelModel } from 'src/app/models/api/downloadExcelModel';
import { InstitutionReport } from 'src/app/models/api/InstitutionReport';
import { SessionType } from 'src/app/models/api/SessionType';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { ReportService } from 'src/app/services/data/report.service';
import { DownloadFileHelper } from 'src/app/utils/download-file-helper';
import { SessionTypeReportUrlMapper } from 'src/app/utils/sessionstype-report-url-mapper';
import { SessionTypes } from 'src/app/utils/sessionTypes';

@Component({
  selector: 'app-download-excel',
  templateUrl: './download-excel.component.html'
})

export class DownloadExcelComponent {
  constructor(
    private institutionService: InstitutionService,
    private reportService: ReportService,
    private toastrService: ToastrService,
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
      this.selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
      this.getInstitution(this.selectedInstitutionId);
      this.loadCoordinatorInstitutionDepartments(this.selectedInstitutionId)
    }
    else if (this.selectedRole === AuthorizedRole.Administrator) {
      this.canSelectInstitution = true;

      this.institutionService.getInstitutions().subscribe(
        (institutions) => {
          this.institutions = institutions;
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

  sessionTypes = SessionTypes.GetSessionTypes();

  selectedSessiontype: SessionType = null;
  fromDate: Date = null;
  toDate: Date = null;
  
  departments: Department[];
  institutions: InstitutionReport[] = [];
  canSelectInstitution = false;
  storedReport = false;
  selectedInstitutionId: number;
  selectedInstitutions: InstitutionReport[] = [];
  // selectedInstitutionTypes: InstitutionType[] = [];
  // selectedDepartmentTypes: DepartmentType[];
  selectedDepartments: Department[] = [];
  // institutionTypes: InstitutionType[];
  // departmentTypes: DepartmentType[] = [];
  allDepartments: Department[] = [];
  allInstitutions: InstitutionReport[] = [];
  createInstitutionalReport = false;

  private selectedRole: AuthorizedRole;
  dropdownSettings: IDropdownSettings;

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

  reset(): void {
    this.selectedDepartments = [];
    // this.selectedDepartmentTypes = [];
    this.selectedSessiontype = null;
    this.createInstitutionalReport = false;
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedInstitutionId = null;
    }
  }

  selectCreateInstitutionalReport() {
    this.selectedDepartments = [];
  }

  canCreateReport() {
    // return (((
    //   this.selectedInstitutionId && this.selectedDepartmentId) ||
    //   (this.selectedInstitutionId && this.createInstitutionalReport)) &&
    //   this.selectedSessiontype && this.fromDate && this.toDate);

    return (
      (
        ((this.selectedInstitutionId || this.selectedInstitutions.length > 0) && this.selectedDepartments.length > 0) ||
        ((this.selectedInstitutionId || this.selectedInstitutions.length > 0) && this.createInstitutionalReport)
      ) && this.selectedSessiontype && this.fromDate && this.toDate);
  }

  saveReport() {
    this.toastrService.clear();
    
    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    // const institutionTypeIds = this.selectedInstitutionTypes?.map(t => t.id) ?? [];
    const institutionIds = this.selectedInstitutionId ? [this.selectedInstitutionId] : this.selectedInstitutions?.map(t => t.id) ?? [];
    // const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];

    this.reportService.reportForSessionTypeHasData(
      {
        sessionType: this.selectedSessiontype,
        // institutionTypeIds: [], 
        institutionIds: institutionIds,
        // departmentTypeIds: [],
        departmentIds: departmentIds,
        fromDate: this.fromDate,
        toDate: this.toDate,
        roleId: this.selectedRole
      }).subscribe(
        reportHasData => {
          if (reportHasData) {
            this.storedReport = true;

            let baseUrl = SessionTypeReportUrlMapper.getReportUrlMap().get(this.selectedSessiontype);
            let url = `${baseUrl}`;
                const payload: DownloadExcelModel = {
                  departmentIds: departmentIds,
                  institutionIds: institutionIds,
                  fromDate: this.fromDate,
                  toDate: this.toDate,
                  role: this.selectedRole
                } 
        
            this.downloadExcel(url, payload).subscribe(() => {
              this.storedReport = false;
            },
              (error) => {
                this.storedReport = false;
                this.toastrService.error(error?.message ? error.message : error, 'An error occurred during download', { disableTimeOut: true });
              });
          } else {
            // this.toastrService.info('There are no observations for selected values', '', { positionClass: 'toast-center-center' });
            this.toastrService.info('There are no observations for selected values', '');

          }
        })
  }

  loadCoordinatorInstitutionDepartments(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
        this.allDepartments = this.departments;
        // this.selectedInstitutionTypes.push(institution.institutionType);
      })
  };

  private downloadExcel(url: string, payload: DownloadExcelModel): Observable<any> {
    return DownloadFileHelper.downloadFile(url, 'application/xlsx, */*', payload)
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
      })
  };
}
