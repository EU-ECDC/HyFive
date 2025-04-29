import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_felles/services/authorization.service';
import { Department} from 'src/app/models/api/Department';
import { InstitutionReport } from 'src/app/models/api/InstitutionReport';
import { SessionType } from 'src/app/models/api/SessionType';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { ReportService } from 'src/app/services/data/report.service';
import { DownloadFileHelper } from 'src/app/utils/download-file-helper';
import { SesjonstypeRapportUrlMapper } from 'src/app/utils/sesjonstype-rapport-url-mapper';
import { SessionTypes } from 'src/app/utils/sessionTypes';

@Component({
  selector: 'app-avdelingsrapport',
  templateUrl: './nedlasting-excel.component.html'
})

export class NedlastingExcelComponent {
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

  velgLagInstitusjonsrapport() {
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

            let baseUrl = SesjonstypeRapportUrlMapper.getRapportUrlMap().get(this.selectedSessiontype);
            let url = `${baseUrl}?fromTime=${this.fromDate}&toTime=${this.toDate}&departmentId=${this.selectedDepartmentId}&institutionId=${this.selectedInstitutionId}&rolle=${this.selectedRole}`;
        
            this.lastNedExcel(url).subscribe(() => {
              this.storedReport = false;
            },
              (error) => {
                this.storedReport = false;
                this.toastrService.error(error?.message ? error.message : error, 'Det oppstod en feil under nedlasting', { disableTimeOut: true });
              });
          } else {
            this.toastrService.info('There are no observations for selected values', '', { positionClass: 'toast-center-center' });
          }
        })
  }

  private lastNedExcel(url: string): Observable<any> {
    return DownloadFileHelper.downloadFile(url, 'application/xlsx, */*')
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
      })
  };
}
