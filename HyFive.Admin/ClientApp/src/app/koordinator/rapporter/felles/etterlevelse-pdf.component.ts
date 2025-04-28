import { Component, Input } from '@angular/core';
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

@Component({
  selector: 'app-etterlevelse-pdf',
  templateUrl: './etterlevelse-pdf.component.html'
})
export class EtterlevelsePdfComponent {
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
      
  @Input() sesjonType: SessionType;
  
  selectedInstitutionId: number;
  valgtAvdelingId: number;
  fromDate: Date = null;
  toDate: Date = null;

  departments: Department[];
  institutions: InstitutionReport[] = [];
  canSelectInstitution = false;
  lagerRapport = false;
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
    this.departments = null;
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedInstitutionId = null;
    }
  }

  kanLageRapport() {
    return (this.valgtAvdelingId && this.fromDate && this.toDate);
  }

  lagRapport() {
    this.toastrService.clear();

    this.rapportService.rapportForSessionTypeHarData(this.sesjonType, this.selectedInstitutionId, this.valgtAvdelingId,
      this.fromDate, this.toDate, this.selectedRole).subscribe(
        rapportHarData => {
          if (rapportHarData) {
            this.lagerRapport = true;
            this.lastNedPdf().subscribe(() => {
              this.lagerRapport = false
            },
              (error) => {
                this.lagerRapport = false
                this.toastrService.error(error?.message ? error.message : error, 'Feil under nedlasting av rapport', { disableTimeOut: true });
              });
          } else {
            this.toastrService.info('Det finnes ikkke observasjoner for valgte verdier', '', { positionClass: 'toast-center-center' });
          }
        })
  }

  private lastNedPdf(): Observable<any> {
    let url = '/api/v1/rapport/';

    if (this.sesjonType == SessionType.FourIndications) {
      url += 'fireindikasjoner';
    } else if (this.sesjonType == SessionType.Handjewelry) {
      url += 'handsmykke';
    }

    url += '/avdeling/pdf/';
    url += `?fraTid=${this.fromDate}&tilTid=${this.toDate}`;
    url += `&rolle=${this.selectedRole}`;
    url += `&institutionId=${this.selectedInstitutionId}&avdelingId=${this.valgtAvdelingId}`;
''
    return LastNedFilHjelper.lastNedFil(url, 'application/pdf, */*')
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
      })
  };
}
