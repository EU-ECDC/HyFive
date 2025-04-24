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
      this.valgtRolle = this.authorizationService.getSelectedRole();
      
      if (this.valgtRolle === AuthorizedRole.Coordinator) {
        this.valgtInstitusjonId = this.institutionService.getSelectedInstitutionId();
        this.getInstitution(this.valgtInstitusjonId)
      }
      else if (this.valgtRolle === AuthorizedRole.Administrator) {
        this.kanVelgeInstitusjon = true;
        
        this.institutionService.getInstitutions().subscribe(
          (institusjoner) => {
            this.institusjoner = institusjoner;
          });
        }
      }
      
  @Input() sesjonType: SessionType;
  
  valgtInstitusjonId: number;
  valgtAvdelingId: number;
  fraDato: Date = null;
  tilDato: Date = null;

  avdelinger: Department[];
  institusjoner: InstitutionReport[] = [];
  kanVelgeInstitusjon = false;
  lagerRapport = false;
  lagInstitusjonsrapport = false;

  private valgtRolle: AuthorizedRole;

  velgInstitusjon(): void {
    this.avdelinger = null;
    this.valgtAvdelingId = null;
    if (this.valgtInstitusjonId != null) {
      this.getInstitution(this.valgtInstitusjonId)
    }
  };

  nullstill(): void {
    this.valgtAvdelingId = null;
    this.avdelinger = null;
    this.fraDato = null;
    this.tilDato = null;
    this.toastrService.clear();

    if (this.valgtRolle === AuthorizedRole.Administrator) {
      this.valgtInstitusjonId = null;
    }
  }

  kanLageRapport() {
    return (this.valgtAvdelingId && this.fraDato && this.tilDato);
  }

  lagRapport() {
    this.toastrService.clear();

    this.rapportService.rapportForSessionTypeHarData(this.sesjonType, this.valgtInstitusjonId, this.valgtAvdelingId,
      this.fraDato, this.tilDato, this.valgtRolle).subscribe(
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
    url += `?fraTid=${this.fraDato}&tilTid=${this.tilDato}`;
    url += `&rolle=${this.valgtRolle}`;
    url += `&institutionId=${this.valgtInstitusjonId}&avdelingId=${this.valgtAvdelingId}`;
''
    return LastNedFilHjelper.lastNedFil(url, 'application/pdf, */*')
  }

  private getInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.avdelinger = institution.departments;
      })
  };
}
