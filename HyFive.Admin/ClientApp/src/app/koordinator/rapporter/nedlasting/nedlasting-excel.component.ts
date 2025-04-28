import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_felles/services/authorization.service';
import { Department} from 'src/app/models/api/Department';
import { InstitutionReport } from 'src/app/models/api/InstitutionReport';
import { SessionType } from 'src/app/models/api/SessionType';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { RapportService } from 'src/app/services/data/rapport.service';
import { LastNedFilHjelper } from 'src/app/utils/last-ned-fil-hjelper';
import { SesjonstypeRapportUrlMapper } from 'src/app/utils/sesjonstype-rapport-url-mapper';
import { SessionTypes } from 'src/app/utils/sessionTypes';

@Component({
  selector: 'app-avdelingsrapport',
  templateUrl: './nedlasting-excel.component.html'
})

export class NedlastingExcelComponent {
  constructor(
    private institutionService: InstitutionService,
    private rapportService: RapportService,
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
  valgtAvdelingId: number;
  fromDate: Date = null;
  toDate: Date = null;
  
  departments: Department[];
  institutions: InstitutionReport[] = [];
  canSelectInstitution = false;
  lagerRapport = false;
  selectedInstitutionId: number;
  lagInstitusjonsrapport = false;

  private selectedRole: AuthorizedRole;

  velgInstitusjon(): void {
    this.departments = null;
    this.valgtAvdelingId = null;
    if (this.selectedInstitutionId != null) {
      this.getInstitution(this.selectedInstitutionId)
    }
  };

  reset(): void {
    this.valgtAvdelingId = null;
    this.selectedSessiontype = null;
    this.lagInstitusjonsrapport = false;
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedInstitutionId = null;
    }
  }

  velgLagInstitusjonsrapport() {
    this.valgtAvdelingId = null;
  }

  kanLageRapport() {
    return (((
      this.selectedInstitutionId && this.valgtAvdelingId) ||
      (this.selectedInstitutionId && this.lagInstitusjonsrapport)) &&
      this.selectedSessiontype && this.fromDate && this.toDate);
  }

  lagRapport() {
    this.toastrService.clear();
    
    this.rapportService.rapportForSessionTypeHarData(this.selectedSessiontype, this.selectedInstitutionId, this.valgtAvdelingId,
      this.fromDate, this.toDate, this.selectedRole).subscribe(
        rapportHarData => {
          if (rapportHarData) {
            this.lagerRapport = true;

            let baseUrl = SesjonstypeRapportUrlMapper.getRapportUrlMap().get(this.selectedSessiontype);
            let url = `${baseUrl}?fraTid=${this.fromDate}&tilTid=${this.toDate}&avdelingId=${this.valgtAvdelingId}&institutionId=${this.selectedInstitutionId}&rolle=${this.selectedRole}`;
        
            this.lastNedExcel(url).subscribe(() => {
              this.lagerRapport = false;
            },
              (error) => {
                this.lagerRapport = false;
                this.toastrService.error(error?.message ? error.message : error, 'Det oppstod en feil under nedlasting', { disableTimeOut: true });
              });
          } else {
            this.toastrService.info('Det finnes ikkke observasjoner for valgte verdier', '', { positionClass: 'toast-center-center' });
          }
        })
  }

  private lastNedExcel(url: string): Observable<any> {
    return LastNedFilHjelper.lastNedFil(url, 'application/xlsx, */*')
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
      })
  };
}
