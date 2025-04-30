import { Component, Input } from '@angular/core';
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

@Component({
  selector: 'app-compliance-pdf',
  templateUrl: './compliance-pdf.component.html'
})
export class CompliancePdfComponent {
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
      
  @Input() sessionType: SessionType;
  
  selectedInstitutionId: number;
  selectedDepartmentId: number;
  fromDate: Date = null;
  toDate: Date = null;

  departments: Department[];
  institutions: InstitutionReport[] = [];
  canSelectInstitution = false;
  storedReport = false;
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
    this.departments = null;
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedInstitutionId = null;
    }
  }

  canCreateReport() {
    return (this.selectedDepartmentId && this.fromDate && this.toDate);
  }

  saveReport() {
    this.toastrService.clear();

    this.reportService.reportForSessionTypeHasData(this.sessionType, this.selectedInstitutionId, this.selectedDepartmentId,
      this.fromDate, this.toDate, this.selectedRole).subscribe(
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
            this.toastrService.info('There are no observations for selected values', '', { positionClass: 'toast-center-center' });
          }
        })
  }

  private downloadPdf(): Observable<any> {
    let url = '/api/v1/report/';

    if (this.sessionType == SessionType.FourIndications) {
      url += 'fourindications';
    } else if (this.sessionType == SessionType.HandJewelry) {
      url += 'handjewelry';
    }

    url += '/department/pdf/';
    url += `?fromTime=${this.fromDate}&toTime=${this.toDate}`;
    url += `&role=${this.selectedRole}`;
    url += `&institutionId=${this.selectedInstitutionId}&departmentId=${this.selectedDepartmentId}`;
''
    return DownloadFileHelper.downloadFile(url, 'application/pdf, */*')
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
      })
  };
}
