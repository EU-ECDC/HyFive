import { Component, OnInit, OnDestroy } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { ObservationService } from '../../services/data/observation.service';
import { SessionType } from '../../models/api/SessionType';
import { faArrowRight } from '@fortawesome/free-solid-svg-icons';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { User } from '../../models/api/User';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { TransferstatusTypeConstants } from '../../models/api/TransferstatusTypeConstants';
import { Institution } from '../../models/api/Institution';
import { forEach } from 'lodash-es';

@Component({
  selector: 'app-overfor-sesjoner',
  templateUrl: './overfor-sesjoner.component.html'
})
export class OverforSesjonerComponent implements OnInit, OnDestroy {

  faArrowRight = faArrowRight;
  faPaperPlane = faPaperPlane;

  sesjontypeAlternativer = [
    { name: "ProtectiveEquipment", verdi: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
    { name: "FourIndications", verdi: SessionType.FourIndications, type: SessionType[SessionType.FourIndications] },
    { name: "Gloves", verdi: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "Håndsmykker", verdi: SessionType.Handjewelry, type: SessionType[SessionType.Handjewelry] },
  ];

  valgtSesjontype: SessionType = null;
  fraDato: Date = null;
  tilDato: Date = null;

  observatorer: User[] = [];
  valgtObservator: User = null;

  institusjon: InstitutionReport;

  institusjonerAlternativer: InstitutionReport[] = [];
  valgteInstitusjonAlternativer: number = null;

  sessions: SessionOverviewReport[] = [];
  sessionsCoordinator: SessionOverviewReport[] = [];
  sessionsFHI: SessionOverviewReport[] = [];
  laster: boolean = false;
  sokGjort: boolean = false;

  constructor(
    private institusjonService: InstitutionService,
    private observationService: ObservationService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    let valgtInstitusjonsId = this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentInstitusjon(valgtInstitusjonsId).subscribe((result: Institution) => {
      this.institusjon = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        institutionType: result.institutionType,
        name: result.name,
        region: result.region
      } as InstitutionReport;

      this.institusjonService.hentObservatorer(this.institusjon.id).subscribe((observatorer) => {
        this.observatorer = observatorer.sort(this.compareFornavnForBrukere);
        this.observatorer = this.visDeaktivertObservatorerNedest(observatorer);
      });
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  visDeaktivertObservatorerNedest(observatorer: User[]): User[] {
    var observatorerListe = observatorer.filter(o => o.isDisabled === false);
    var observatorerSomErDeaktivert = observatorer.filter(o => o.isDisabled);
    observatorerListe.push.apply(observatorerListe, observatorerSomErDeaktivert);
    return observatorerListe;
  }

  hentSesjoner() {
    this.laster = true;
    this.observationService.getSessionsForInstitution(
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
    this.sessionsCoordinator = this.sessions.filter(x => x.overforingstatus.code === TransferstatusTypeConstants.OverfortTilKoordinator);
    this.sessionsFHI = this.sessions.filter(x => x.overforingstatus.code === TransferstatusTypeConstants.OverfortTilFhi);
  }

  overfor(sesjonId) {
    this.laster = true;
    this.observationService.oppositeSessionToFHI(this.institusjon.id, sesjonId).subscribe((result) => {
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

  compareFornavnForBrukere(a: User, b: User): number {
    if (a.firstName.toLowerCase() < b.firstName.toLowerCase()) return -1;
    if (a.firstName.toLowerCase() > b.firstName.toLowerCase()) return 1;
    return 0;
  }
}

