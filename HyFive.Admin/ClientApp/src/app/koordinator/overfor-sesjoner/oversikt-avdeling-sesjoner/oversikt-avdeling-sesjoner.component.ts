import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QueryParameters } from '../../../_felles/konstanter/queryparameters';
import { SesjonType } from '../../../models/api/SesjonType';
import { ObservasjonService } from '../../../services/data/observasjon.service';
import { SesjonOversiktRapport } from '../../../models/api/SesjonOversiktRapport';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { DatePipe } from '@angular/common';
import { AvdelingService } from '../../../services/data/avdeling.service';
import { Avdeling } from '../../../models/api/Avdeling';
import { AuthorizedRole } from '../../../_felles/authorization/authorized-role';
import { AuthorizationService } from '../../../_felles/services/authorization.service';

@Component({
  selector: 'app-oversikt-avdeling-sesjoner',
  templateUrl: './oversikt-avdeling-sesjoner.component.html'
})
export class OversiktAvdelingSesjonerComponent implements OnInit {

  avdelingsid: number;
  valgtSesjontype: SesjonType = null;
  fraDato: Date;
  tilDato: Date;
  valgteInstitusjonAlternativer: number = null;

  sesjontypeAlternativer = [
    { navn: "FireIndikasjoner", verdi: SesjonType.FireIndikasjoner },
    { navn: "Håndsmykker", verdi: SesjonType.Handsmykker },
    { navn: "Beskyttelsesutstyr", verdi: SesjonType.Beskyttelsesutstyr },
    { navn: "Hansker", verdi: SesjonType.Hansker }
  ];

  avdeling: Avdeling;
  sesjoner: SesjonOversiktRapport[] = [];
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
        if (!params[QueryParameters.Avdelingsid]) this.router.navigate([`/${UrlPaths.observasjoner}`]);

        this.valgtSesjontype = parseInt(params[QueryParameters.Sesjontype]) || null;
        this.fraDato = params[QueryParameters.FraDato] || null;
        this.tilDato = params[QueryParameters.TilDato] || null;
        this.avdelingsid = parseInt(params[QueryParameters.Avdelingsid]) || null;
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
      this.sesjoner = resultater;

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

