import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { Department} from 'src/app/models/api/Department';
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

  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();

    if (this.selectedRole === AuthorizedRole.Coordinator) {
      this.selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
      this.getInstitution(this.selectedInstitutionId)
    }
    else if (this.selectedRole === AuthorizedRole.Administrator) {
      this.canSelectInstitution = true;

      this.institutionService.getInstitutions().subscribe(
        (institutions) => {
          this.institutions = institutions;
        });
    }
  }

  sessionTypes = SessionTypes.GetSessionTypes();

  selectedSessiontype: SessionType = null;
  selectedDepartmentId: number;
  fromDate: Date = null;
  toDate: Date = null;
  
  departments: Department[];
  institutions: InstitutionReport[] = [];
  canSelectInstitution = false;
  storedReport = false;
  selectedInstitutionId: number;
  createInstitutionalReport = false;

  private selectedRole: AuthorizedRole;

  selectInstitution(): void {
    this.departments = null;
    this.selectedDepartmentId = null;
    if (this.selectedInstitutionId != null) {
      this.getInstitution(this.selectedInstitutionId)
    }
  };

  reset(): void {
    this.selectedDepartmentId = null;
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
    this.selectedDepartmentId = null;
  }

  canCreateReport() {
    return (((
      this.selectedInstitutionId && this.selectedDepartmentId) ||
      (this.selectedInstitutionId && this.createInstitutionalReport)) &&
      this.selectedSessiontype && this.fromDate && this.toDate);
  }

  saveReport() {
    this.toastrService.clear();
    
    this.reportService.reportForSessionTypeHasData(this.selectedSessiontype, this.selectedInstitutionId, this.selectedDepartmentId,
      this.fromDate, this.toDate, this.selectedRole).subscribe(
        reportHasData => {
          if (reportHasData) {
            this.storedReport = true;

            let baseUrl = SessionTypeReportUrlMapper.getReportUrlMap().get(this.selectedSessiontype);
            let url = `${baseUrl}?fromTime=${this.fromDate}&toTime=${this.toDate}&departmentId=${this.selectedDepartmentId}&institutionId=${this.selectedInstitutionId}&role=${this.selectedRole}`;
        
            this.downloadExcel(url).subscribe(() => {
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

  private downloadExcel(url: string): Observable<any> {
    return DownloadFileHelper.downloadFile(url, 'application/xlsx, */*')
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
      })
  };
}
