import { Component, OnInit, OnDestroy } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';
import { ObservasjonService } from '../../services/data/observasjon.service';
import { SesjonType } from '../../models/api/SesjonType';
import { faArrowRight } from '@fortawesome/free-solid-svg-icons';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { Bruker } from '../../models/api/Bruker';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { TransferstatusTypeConstants } from '../../models/api/TransferstatusTypeConstants';
import { Institusjon } from '../../models/api/Institusjon';
import { forEach } from 'lodash-es';

@Component({
  selector: 'app-overfor-sesjoner',
  templateUrl: './overfor-sesjoner.component.html'
})
export class OverforSesjonerComponent implements OnInit, OnDestroy {

  faArrowRight = faArrowRight;
  faPaperPlane = faPaperPlane;

  sesjontypeAlternativer = [
    { navn: "Beskyttelsesutstyr", verdi: SesjonType.Beskyttelsesutstyr, type: SesjonType[SesjonType.Beskyttelsesutstyr] },
    { navn: "FireIndikasjoner", verdi: SesjonType.FireIndikasjoner, type: SesjonType[SesjonType.FireIndikasjoner] },
    { navn: "Hansker", verdi: SesjonType.Hansker, type: SesjonType[SesjonType.Hansker] },
    { navn: "Håndsmykker", verdi: SesjonType.Handsmykker, type: SesjonType[SesjonType.Handsmykker] },
  ];

  valgtSesjontype: SesjonType = null;
  fraDato: Date = null;
  tilDato: Date = null;

  observatorer: Bruker[] = [];
  valgtObservator: Bruker = null;

  institusjon: InstitusjonRapport;

  institusjonerAlternativer: InstitusjonRapport[] = [];
  valgteInstitusjonAlternativer: number = null;

  sessions: SessionOverviewReport[] = [];
  sessionsCoordinator: SessionOverviewReport[] = [];
  sessionsFHI: SessionOverviewReport[] = [];
  laster: boolean = false;
  sokGjort: boolean = false;

  constructor(
    private institusjonService: InstitusjonService,
    private observasjonService: ObservasjonService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    let valgtInstitusjonsId = this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentInstitusjon(valgtInstitusjonsId).subscribe((result: Institusjon) => {
      this.institusjon = {
        id: result.id,
        herId: result.herId,
        forkortelse: result.forkortelse,
        institusjontype: result.institusjontype,
        navn: result.navn,
        region: result.region
      } as InstitusjonRapport;

      this.institusjonService.hentObservatorer(this.institusjon.id).subscribe((observatorer) => {
        this.observatorer = observatorer.sort(this.compareFornavnForBrukere);
        this.observatorer = this.visDeaktivertObservatorerNedest(observatorer);
      });
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  visDeaktivertObservatorerNedest(observatorer: Bruker[]): Bruker[] {
    var observatorerListe = observatorer.filter(o => o.erDeaktivert === false);
    var observatorerSomErDeaktivert = observatorer.filter(o => o.erDeaktivert);
    observatorerListe.push.apply(observatorerListe, observatorerSomErDeaktivert);
    return observatorerListe;
  }

  hentSesjoner() {
    this.laster = true;
    this.observasjonService.hentSesjonerForInstitusjon(
      this.institusjon.id,
      this.valgtObservator,
      this.valgtSesjontype,
      this.fraDato,
      this.tilDato
    ).subscribe((resultater) => {
      this.laster = false;
      this.sokGjort = true;
      this.sessions = resultater;
      this.oppdaterLister();
    })
  }

  oppdaterLister() {
    this.sessionsCoordinator = this.sessions.filter(x => x.overforingstatus.kode === TransferstatusTypeConstants.OverfortTilKoordinator);
    this.sessionsFHI = this.sessions.filter(x => x.overforingstatus.kode === TransferstatusTypeConstants.OverfortTilFhi);
  }

  overfor(sesjonId) {
    this.laster = true;
    this.observasjonService.overforSesjonTilFHI(this.institusjon.id, sesjonId).subscribe((result) => {
      if (result) {
        this.sessions.find(x => x.id === result.id).overforingstatus = result.overforingstatus;
        this.toastrService.success('Sesjonen(e) ble overført til FHI');
        this.oppdaterLister();
        this.laster = false;
      }
      else this.toastrService.error('Det oppstod en feil under overføringen. Vennligst prøv på nytt.', '', { disableTimeOut: true});
    });
  }

  merkAlleSesjoner() {
    this.sessionsCoordinator.forEach(s => s.erValgt = true);
  }

  overforSesjoner() {
    let valgteSesjoner = this.sessionsCoordinator.filter(s => s.erValgt);
  
    valgteSesjoner.forEach(sesjon => {
      this.overfor(sesjon.id);
    });
  }

  nullstill(): void {
    this.valgtObservator = null;
    this.valgtSesjontype = null;
    this.fraDato = null;
    this.tilDato = null;
    this.nullstillSokeresultat();
  }

  nullstillSokeresultat() {
    this.sokGjort = false;
    this.sessionsCoordinator = [];
    this.sessionsFHI = [];
    this.sessions = [];
  }

  observasjonSlettet(): void {
    this.hentSesjoner();
  }

  harVelgtMinstEnSesjon(): boolean {
    return this.sessionsCoordinator.some(s => s.erValgt);
  }

  compareFornavnForBrukere(a: Bruker, b: Bruker): number {
    if (a.fornavn.toLowerCase() < b.fornavn.toLowerCase()) return -1;
    if (a.fornavn.toLowerCase() > b.fornavn.toLowerCase()) return 1;
    return 0;
  }
}

