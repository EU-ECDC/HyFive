import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { ObservationService } from '../../services/data/observation.service';
import { SessionType } from '../../models/api/SessionType';
import { QueryParameters } from '../../_felles/konstanter/queryparameters';
import { UrlPaths } from '../../_felles/konstanter/url-paths';
import { faArrowRight, faFileDownload, faFileExcel, faFilePdf } from '@fortawesome/free-solid-svg-icons';
import { InstitutionOverviewReport } from "../../models/api/InstitutionOverviewReport";
import { ToastrService } from "ngx-toastr";
import { AuthorizedRole } from 'src/app/_felles/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_felles/services/authorization.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { AvdelingOversiktRapport } from 'src/app/models/api/AvdelingOversiktRapport';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-oversikt-observasjoner',
  templateUrl: './oversikt-observasjoner.component.html'
})
export class OversiktObservasjonerComponent implements OnInit, OnDestroy {

  SessionType = SessionType;
  faArrowRight = faArrowRight;
  faFileDownload = faFileDownload;
  faFileExcel = faFileExcel;
  faFilePdf = faFilePdf;

  sesjontyper = [
    { name: 'Beskyttelsesutstyr', verdi: SessionType.Beskyttelsesutstyr },
    { name: 'Fire indikasjoner', verdi: SessionType.FireIndikasjoner },
    { name: 'Hansker', verdi: SessionType.Hansker },
    { name: 'Håndsmykker', verdi: SessionType.Handsmykker }
  ];

  valgtSesjontype: SessionType = null;
  institusjonIdForRapportSomLastesNed = 0;

  fraDato: Date = null;
  tilDato: Date = null;
  institusjoner: InstitutionReport[] = [];
  valgtInstitusjonId: string = null;
  avdelingIdForRapportSomLastesNed = 0;

  valgtInstitusjonFraListeId: number = 0;

  institusjonOversiktRapportListe: InstitutionOverviewReport[] = [];
  kanVelgeInstitusjon = false;
  soker = false;
  private valgtRolle: AuthorizedRole;

  constructor(
    private institusjonService: InstitutionService,
    private observationService: ObservationService,
    private route: ActivatedRoute,
    private router: Router,
    private toastrService: ToastrService,
    private authorizationService: AuthorizationService,
    private httpClient: HttpClient) { }

  ngOnInit(): void {
    this.valgtRolle = this.authorizationService.hentValgtRolle();
    this.valgtInstitusjonId = this.hentInstitusjonId();

    this.route
      .queryParams
      .subscribe(params => {
        if (Object.keys(params).length === 0) {
          return;
        }

        this.valgtSesjontype = parseInt(params[QueryParameters.SessionType], 10) || null;
        this.fraDato = params[QueryParameters.FromDate] || null;
        this.tilDato = params[QueryParameters.ToDate] || null;
        if (this.valgtRolle === AuthorizedRole.Administrator)
          this.valgtInstitusjonId = params[QueryParameters.InstitutionIdIsOk] || null;
        this.getInstitutionsWithSessions();
      });

    if (this.valgtRolle === AuthorizedRole.Administrator) {
      this.kanVelgeInstitusjon = true;
      this.hentInstitusjon();
    } else if (this.valgtRolle === AuthorizedRole.Coordinator) {
      this.kanVelgeInstitusjon = false;
    }
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  hentInstitusjonId(): string {
    if(this.valgtRolle === AuthorizedRole.Coordinator)
      return this.institusjonService.hentValgtInstitusjonId().toString();
    return "null";
  }

  hentInstitusjon() {
    this.institusjonService.getInstitutions().subscribe(
      (institusjoner) => {
        this.institusjoner = institusjoner;
      },
      error => {
        this.toastrService.error(error.error.message, 'Lasting av institusjoner feilet', {disableTimeOut: true});
      });
  }

  getInstitutionsWithSessions() {
    let valgtInstitusjonId = this.valgtInstitusjonId;
    if (this.valgtInstitusjonId === null || this.valgtInstitusjonId === 'null') {
      valgtInstitusjonId = null;
    }
    this.soker = true;
    this.institusjonOversiktRapportListe = new Array<InstitutionOverviewReport>();

    this.observationService.getInstitutionsWithSessions(
      valgtInstitusjonId,
      this.valgtSesjontype ? this.valgtSesjontype : null,
      this.fraDato,
      this.tilDato,
      this.valgtRolle
    ).subscribe((resultater) => {
      if(this.valgtSesjontype !== SessionType.FireIndikasjoner && this.valgtSesjontype !== SessionType.Handsmykker &&
        this.valgtSesjontype !== SessionType.Hansker && this.valgtSesjontype !== SessionType.Beskyttelsesutstyr)
        this.valgtSesjontype = null;
      this.institusjonOversiktRapportListe = resultater;
      this.soker = false;
    },
      error => {
        this.toastrService.error(error.error.message, 'Lasting av listen feilet', {disableTimeOut: true});
        this.institusjonOversiktRapportListe = new Array<InstitutionOverviewReport>();
        this.soker = false;
      });

  }

  navigerTilObservasjonerForAvdelingen(avdeling) {
    this.router.navigate([`/${UrlPaths.observationsDepartment}`], {
      queryParams: this.trim({
        avdelingsid: avdeling.id,
        sesjontype: this.valgtSesjontype,
        fra: this.fraDato,
        til: this.tilDato,
        institusjonsidISok: this.valgtInstitusjonId
      })
    });
  }

  nullstill(): void {
    this.valgtSesjontype = null;
    this.fraDato = null;
    this.tilDato = null;

    if (this.valgtRolle === AuthorizedRole.Administrator) {
      this.valgtInstitusjonId = null;
    }

    this.institusjonOversiktRapportListe = new Array<InstitutionOverviewReport>();
  }

  nullstillSokeresultat() {
    this.institusjonOversiktRapportListe = new Array<InstitutionOverviewReport>();
  }

  lagValgtSesjonstypeTekst(): string {
    if (this.valgtSesjontype == SessionType.Beskyttelsesutstyr)
      return "Beskyttelsesutstyr";
    if (this.valgtSesjontype == SessionType.FireIndikasjoner)
      return "FireIndikasjoner";
    if (this.valgtSesjontype == SessionType.Handsmykker)
      return "Handsmykker";
    if (this.valgtSesjontype == SessionType.Hansker)
      return "Hansker";

    return "Alle";
  }

  private trim(params: Params): Params {
    Object.keys(params).forEach((k) => (params[k] == null || params[k] === 'null') && delete params[k]);
    return params;
  }

  findId(item: any) {
    this.valgtInstitusjonFraListeId = item.id;
  }

  sorter($event: IColumnSortedEvent) {

    event.stopPropagation();
    event.preventDefault();

    let propertyOf: (x: AvdelingOversiktRapport) => any;
    switch ($event.columnName) {
      case "Navn":
        propertyOf = (x: AvdelingOversiktRapport) => x.name;
        break;
      default:
        throw new Error("Ugyldig sorteringskolonne");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: AvdelingOversiktRapport, b: AvdelingOversiktRapport) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    let index = this.institusjonOversiktRapportListe.findIndex(x => x.id == this.valgtInstitusjonFraListeId);

    if (index > -1) {
      this.institusjonOversiktRapportListe[index].departments = this.institusjonOversiktRapportListe[index].departments.sort(sortFunc);
    }
  }
}
