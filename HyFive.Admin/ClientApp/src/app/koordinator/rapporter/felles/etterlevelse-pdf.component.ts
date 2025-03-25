import { Component, Input } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_felles/services/authorization.service';
import { Avdeling } from 'src/app/models/api/Avdeling';
import { InstitusjonRapport } from 'src/app/models/api/InstitusjonRapport';
import { SesjonType } from 'src/app/models/api/SesjonType';
import { InstitusjonService } from 'src/app/services/data/institusjon.service';
import { RapportService } from 'src/app/services/data/rapport.service';
import { LastNedFilHjelper } from 'src/app/utils/last-ned-fil-hjelper';

@Component({
  selector: 'app-etterlevelse-pdf',
  templateUrl: './etterlevelse-pdf.component.html'
})
export class EtterlevelsePdfComponent {
  constructor(
    private institusjonService: InstitusjonService,
    private rapportService: RapportService,
    private toastrService: ToastrService,
    private authorizationService: AuthorizationService) { }
    
    ngOnInit(): void {
      this.valgtRolle = this.authorizationService.hentValgtRolle();
      
      if (this.valgtRolle === AuthorizedRole.Koordinator) {
        this.valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId();
        this.hentInstitusjon(this.valgtInstitusjonId)
      }
      else if (this.valgtRolle === AuthorizedRole.Administrator) {
        this.kanVelgeInstitusjon = true;
        
        this.institusjonService.hentInstitusjoner().subscribe(
          (institusjoner) => {
            this.institusjoner = institusjoner;
          });
        }
      }
      
  @Input() sesjonType: SesjonType;
  
  valgtInstitusjonId: number;
  valgtAvdelingId: number;
  fraDato: Date = null;
  tilDato: Date = null;

  avdelinger: Avdeling[];
  institusjoner: InstitusjonRapport[] = [];
  kanVelgeInstitusjon = false;
  lagerRapport = false;
  lagInstitusjonsrapport = false;

  private valgtRolle: AuthorizedRole;

  velgInstitusjon(): void {
    this.avdelinger = null;
    this.valgtAvdelingId = null;
    if (this.valgtInstitusjonId != null) {
      this.hentInstitusjon(this.valgtInstitusjonId)
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

    this.rapportService.rapportForSesjonTypeHarData(this.sesjonType, this.valgtInstitusjonId, this.valgtAvdelingId,
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

    if (this.sesjonType == SesjonType.FireIndikasjoner) {
      url += 'fireindikasjoner';
    } else if (this.sesjonType == SesjonType.Handsmykker) {
      url += 'handsmykke';
    }

    url += '/avdeling/pdf/';
    url += `?fraTid=${this.fraDato}&tilTid=${this.tilDato}`;
    url += `&rolle=${this.valgtRolle}`;
    url += `&institusjonId=${this.valgtInstitusjonId}&avdelingId=${this.valgtAvdelingId}`;
''
    return LastNedFilHjelper.lastNedFil(url, 'application/pdf, */*')
  }

  private hentInstitusjon(institusjonId: number) {
    this.institusjonService.hentInstitusjon(institusjonId).subscribe(
      institusjon => {
        this.avdelinger = institusjon.avdelinger;
      })
  };
}
