import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QueryParameters } from '../../../_felles/konstanter/queryparameters';
import { SessionType } from '../../../models/api/SessionType';
import { ObservasjonService } from '../../../services/data/observasjon.service';
import { SesjonOversiktRapport } from '../../../models/api/SesjonOversiktRapport';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { DatePipe } from '@angular/common';
import { AvdelingService } from '../../../services/data/avdeling.service';
import { Department } from '../../../models/api/Department';
import { AuthorizedRole } from '../../../_felles/authorization/authorized-role';
import { AuthorizationService } from '../../../_felles/services/authorization.service';
import { InstitusjonService } from 'src/app/services/data/institusjon.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-oversikt-avdeling-sessions',
  templateUrl: './oversikt-avdeling-sessions.component.html'
})
export class OversiktAvdelingSesjonerComponent implements OnInit {

  avdelingsid: number;
  valgtSesjontype: SessionType = null;
  fraDato: Date;
  tilDato: Date;
  valgtInstitusjonId: number = null;
  institusjonsidISok: number;

  sesjontypeAlternativer = [
    { navn: "FireIndikasjoner", verdi: SessionType.FireIndikasjoner, type: SessionType[SessionType.FireIndikasjoner] },
    { navn: "Håndsmykker", verdi: SessionType.Handsmykker, type: SessionType[SessionType.Handsmykker] },
    { navn: "Hansker", verdi: SessionType.Hansker, type: SessionType[SessionType.Hansker] },
    { navn: "Beskyttelsesutstyr", verdi: SessionType.Beskyttelsesutstyr, type: SessionType[SessionType.Beskyttelsesutstyr] },
  ];

  avdeling: Department;
  sessions: SesjonOversiktRapport[] = [];
  laster: boolean;
  valgtRolle: AuthorizedRole;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private avdelingService: AvdelingService,
    private observasjonService: ObservasjonService,
    private datepipe: DatePipe,
    private authorizationService: AuthorizationService,
    private institusjonService: InstitusjonService
  ) { }


  ngOnInit(): void {
    this.laster = true;
    this.valgtRolle = this.authorizationService.hentValgtRolle();
    this.valgtInstitusjonId = this.hentInstitusjonId();

    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.DepartmentId]) this.router.navigate([`/${UrlPaths.observasjoner}`]);

        this.valgtSesjontype = parseInt(params[QueryParameters.Sesjontype]) || null;
        this.fraDato = params[QueryParameters.FromDate] || null;
        this.tilDato = params[QueryParameters.ToDate] || null;
        this.institusjonsidISok = params[QueryParameters.InstitusjonsidISok] || null;
        this.avdelingsid = parseInt(params[QueryParameters.DepartmentId]) || null;
      });

      const avdelingSesjonerRequest = [
        this.avdelingService.hentAvdeling(this.avdelingsid),
        this.observasjonService.hentSesjonerForAvdeling(
          this.avdelingsid,
          this.valgtSesjontype ? this.valgtSesjontype : null,
          this.fraDato,
          this.tilDato,
          this.valgtRolle
        )
      ]

      forkJoin(avdelingSesjonerRequest).subscribe((result) => {
        let i = 0;
        this.avdeling = result[i++] as Department;
        this.sessions = result[i++] as SesjonOversiktRapport[];

        this.laster = false;
        
        if(this.erKoordinatorByttetInstitusjon())
        {
          this.router.navigate([`/${UrlPaths.observasjoner}`], {
            queryParams: {
              sesjontype: null,
              fra: null,
              til: null,
            }
            });
        }
      });
  }

  hentInstitusjonId(): number {
    if(this.valgtRolle === AuthorizedRole.Coordinator) 
      return this.institusjonService.hentValgtInstitusjonId()
    return null;
  }

  erKoordinatorByttetInstitusjon() {
    return this.avdeling.institusjonId !== this.valgtInstitusjonId && this.valgtRolle == AuthorizedRole.Coordinator;
  }

  hentSesjonerForAvdeling() {
    
    this.observasjonService.hentSesjonerForAvdeling(
      this.avdelingsid,
      this.valgtSesjontype ? this.valgtSesjontype : null,
      this.fraDato,
      this.tilDato,
      this.valgtRolle
    ).subscribe((resultater) => {
      this.sessions = resultater; 
    });
  }

  visFormatedDatoMedTidspunkt(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy, HH:mm:ss');
  }

  visFormatedDato(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy');
  }

  navigerTilObservasjonerForInstitusjoner() {
    this.router.navigate([`/${UrlPaths.observasjoner}`], {
      queryParams: {
        sesjontype: this.valgtSesjontype,
        fra: this.fraDato,
        til: this.tilDato,
        institusjonsidISok: this.institusjonsidISok
      }
    });
  }
}
