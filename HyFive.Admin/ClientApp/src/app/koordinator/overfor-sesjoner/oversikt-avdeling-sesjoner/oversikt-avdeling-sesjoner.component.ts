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

@Component({
  selector: 'app-oversikt-avdeling-sessions',
  templateUrl: './oversikt-avdeling-sessions.component.html'
})
export class OversiktAvdelingSesjonerComponent implements OnInit {

  avdelingsid: number;
  valgtSesjontype: SessionType = null;
  fraDato: Date;
  tilDato: Date;
  valgteInstitusjonAlternativer: number = null;

  sesjontypeAlternativer = [
    { navn: "FireIndikasjoner", verdi: SessionType.FireIndikasjoner },
    { navn: "Håndsmykker", verdi: SessionType.Handsmykker },
    { navn: "Beskyttelsesutstyr", verdi: SessionType.Beskyttelsesutstyr },
    { navn: "Hansker", verdi: SessionType.Hansker }
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
    private authorizationService: AuthorizationService  ) { }


  ngOnInit(): void {
    this.valgtRolle = this.authorizationService.hentValgtRolle();
    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.DepartmentId]) this.router.navigate([`/${UrlPaths.observasjoner}`]);

        this.valgtSesjontype = parseInt(params[QueryParameters.Sesjontype]) || null;
        this.fraDato = params[QueryParameters.FromDate] || null;
        this.tilDato = params[QueryParameters.ToDate] || null;
        this.avdelingsid = parseInt(params[QueryParameters.DepartmentId]) || null;
        this.valgteInstitusjonAlternativer = parseInt(params[QueryParameters.Institusjonider]) || null;

        this.hentAvdeling();
        this.hentSesjonerForAvdeling();
      });
  }

  hentAvdeling() {
    this.avdelingService.hentAvdeling(
      this.avdelingsid
    ).subscribe((resultat) => {
      this.avdeling = resultat;
    });
  }

  hentSesjonerForAvdeling() {
    this.laster = true;
    this.observasjonService.hentSesjonerForAvdeling(
      this.avdelingsid,
      this.valgtSesjontype ? this.valgtSesjontype : null,
      this.fraDato,
      this.tilDato,
      this.valgtRolle
    ).subscribe((resultater) => {
      this.sessions = resultater;

      this.laster = false;
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
        institusjonider: this.valgteInstitusjonAlternativer
      }
    });
  }
}

