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
    private institusjonService: InstitutionService,
    private rapportService: RapportService,
    private toastrService: ToastrService,
    private authorizationService: AuthorizationService) { }

  ngOnInit(): void {
    this.valgtRolle = this.authorizationService.hentValgtRolle();

    if (this.valgtRolle === AuthorizedRole.Coordinator) {
      this.valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId();
      this.hentInstitusjon(this.valgtInstitusjonId)
    }
    else if (this.valgtRolle === AuthorizedRole.Administrator) {
      this.kanVelgeInstitusjon = true;

      this.institusjonService.getInstitutions().subscribe(
        (institusjoner) => {
          this.institusjoner = institusjoner;
        });
    }
  }

  sessionTypes = SessionTypes.GetSessionTypes();

  valgtSesjontype: SessionType = null;
  valgtAvdelingId: number;
  fraDato: Date = null;
  tilDato: Date = null;
  
  avdelinger: Department[];
  institusjoner: InstitutionReport[] = [];
  kanVelgeInstitusjon = false;
  lagerRapport = false;
  valgtInstitusjonId: number;
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
    this.valgtSesjontype = null;
    this.lagInstitusjonsrapport = false;
    this.fraDato = null;
    this.tilDato = null;
    this.toastrService.clear();

    if (this.valgtRolle === AuthorizedRole.Administrator) {
      this.valgtInstitusjonId = null;
    }
  }

  velgLagInstitusjonsrapport() {
    this.valgtAvdelingId = null;
  }

  kanLageRapport() {
    return (((
      this.valgtInstitusjonId && this.valgtAvdelingId) ||
      (this.valgtInstitusjonId && this.lagInstitusjonsrapport)) &&
      this.valgtSesjontype && this.fraDato && this.tilDato);
  }

  lagRapport() {
    this.toastrService.clear();
    
    this.rapportService.rapportForSessionTypeHarData(this.valgtSesjontype, this.valgtInstitusjonId, this.valgtAvdelingId,
      this.fraDato, this.tilDato, this.valgtRolle).subscribe(
        rapportHarData => {
          if (rapportHarData) {
            this.lagerRapport = true;

            let baseUrl = SesjonstypeRapportUrlMapper.getRapportUrlMap().get(this.valgtSesjontype);
            let url = `${baseUrl}?fraTid=${this.fraDato}&tilTid=${this.tilDato}&avdelingId=${this.valgtAvdelingId}&institutionId=${this.valgtInstitusjonId}&rolle=${this.valgtRolle}`;
        
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

  private hentInstitusjon(institutionId: number) {
    this.institusjonService.hentInstitusjon(institutionId).subscribe(
      institution => {
        this.avdelinger = institution.departments;
      })
  };
}
