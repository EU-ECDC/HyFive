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

  sessionTypeOptions = [
    { name: "ProtectiveEquipment", value: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
    { name: "FourIndications", value: SessionType.FourIndications, type: SessionType[SessionType.FourIndications] },
    { name: "Gloves", value: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "Handjewelry", value: SessionType.Handjewelry, type: SessionType[SessionType.Handjewelry] },
  ];

  selectedSessiontype: SessionType = null;
  fromDate: Date = null;
  toDate: Date = null;

  observers: User[] = [];
  valgtObservator: User = null;

  institusjon: InstitutionReport;

  institusjonerAlternativer: InstitutionReport[] = [];
  selectedInstitutionOptions: number = null;

  sessions: SessionOverviewReport[] = [];
  sessionsCoordinator: SessionOverviewReport[] = [];
  sessionsFHI: SessionOverviewReport[] = [];
  loading: boolean = false;
  sokGjort: boolean = false;

  constructor(
    private institutionService: InstitutionService,
    private observationService: ObservationService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    let valgtInstitusjonsId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(valgtInstitusjonsId).subscribe((result: Institution) => {
      this.institusjon = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        institutionType: result.institutionType,
        name: result.name,
        region: result.region
      } as InstitutionReport;

      this.institutionService.getObservers(this.institusjon.id).subscribe((observers) => {
        this.observers = observers.sort(this.compareFornavnForBrukere);
        this.observers = this.visDeaktivertObservatorerNedest(observers);
      });
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  visDeaktivertObservatorerNedest(observers: User[]): User[] {
    var observatorerListe = observers.filter(o => o.isDisabled === false);
    var observatorerSomErDeaktivert = observers.filter(o => o.isDisabled);
    observatorerListe.push.apply(observatorerListe, observatorerSomErDeaktivert);
    return observatorerListe;
  }

  hentSesjoner() {
    this.loading = true;
    this.observationService.getSessionsForInstitution(
      this.institusjon.id,
      this.valgtObservator,
      this.selectedSessiontype,
      this.fromDate,
      this.toDate
    ).subscribe((results) => {
      this.loading = false;
      this.sokGjort = true;
      this.sessions = results;
      this.oppdaterLister();
    })
  }

  oppdaterLister() {
    this.sessionsCoordinator = this.sessions.filter(x => x.transferStatus.code === TransferstatusTypeConstants.TransferToCoordinator);
    this.sessionsFHI = this.sessions.filter(x => x.transferStatus.code === TransferstatusTypeConstants.TransferToFhi);
  }

  overfor(sesjonId) {
    this.loading = true;
    this.observationService.oppositeSessionToFHI(this.institusjon.id, sesjonId).subscribe((result) => {
      if (result) {
        this.sessions.find(x => x.id === result.id).transferStatus = result.transferStatus;
        this.toastrService.success('Sesjonen(e) ble overført til FHI');
        this.oppdaterLister();
        this.loading = false;
      }
      else this.toastrService.error('Det oppstod en feil under overføringen. Vennligst prøv på nytt.', '', { disableTimeOut: true});
    });
  }

  merkAlleSesjoner() {
    this.sessionsCoordinator.forEach(s => s.isSelected = true);
  }

  overforSesjoner() {
    let valgteSesjoner = this.sessionsCoordinator.filter(s => s.isSelected);
  
    valgteSesjoner.forEach(sesjon => {
      this.overfor(sesjon.id);
    });
  }

  reset(): void {
    this.valgtObservator = null;
    this.selectedSessiontype = null;
    this.fromDate = null;
    this.toDate = null;
    this.resetSearchresults();
  }

  resetSearchresults() {
    this.sokGjort = false;
    this.sessionsCoordinator = [];
    this.sessionsFHI = [];
    this.sessions = [];
  }

  observasjonSlettet(): void {
    this.hentSesjoner();
  }

  harVelgtMinstEnSesjon(): boolean {
    return this.sessionsCoordinator.some(s => s.isSelected);
  }

  compareFornavnForBrukere(a: User, b: User): number {
    if (a.firstName.toLowerCase() < b.firstName.toLowerCase()) return -1;
    if (a.firstName.toLowerCase() > b.firstName.toLowerCase()) return 1;
    return 0;
  }
}

