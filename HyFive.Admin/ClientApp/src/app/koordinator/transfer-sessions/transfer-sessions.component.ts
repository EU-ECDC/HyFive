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
  selector: 'app-transfer-sessions',
  templateUrl: './transfer-sessions.component.html'
})
export class TransferSessionsComponent implements OnInit, OnDestroy {

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
  selectedObserver: User = null;

  institution: InstitutionReport;

  institutionsOptions: InstitutionReport[] = [];
  selectedInstitutionOptions: number = null;

  sessions: SessionOverviewReport[] = [];
  sessionsCoordinator: SessionOverviewReport[] = [];
  sessionsFHI: SessionOverviewReport[] = [];
  loading: boolean = false;
  SearchDone: boolean = false;

  constructor(
    private institutionService: InstitutionService,
    private observationService: ObservationService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(selectedInstitutionId).subscribe((result: Institution) => {
      this.institution = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        institutionType: result.institutionType,
        name: result.name,
        region: result.region
      } as InstitutionReport;

      this.institutionService.getObservers(this.institution.id).subscribe((observers) => {
        this.observers = observers.sort(this.compareFirstNameForUsers);
        this.observers = this.showDisabledObserversBottom(observers);
      });
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  showDisabledObserversBottom(observers: User[]): User[] {
    var observersList = observers.filter(o => o.isDisabled === false);
    var observersWhoAreDisabled = observers.filter(o => o.isDisabled);
    observersList.push.apply(observersList, observersWhoAreDisabled);
    return observersList;
  }

  getSessions() {
    this.loading = true;
    this.observationService.getSessionsForInstitution(
      this.institution.id,
      this.selectedObserver,
      this.selectedSessiontype,
      this.fromDate,
      this.toDate
    ).subscribe((results) => {
      this.loading = false;
      this.SearchDone = true;
      this.sessions = results;
      this.updateLists();
    })
  }

  updateLists() {
    this.sessionsCoordinator = this.sessions.filter(x => x.transferStatus.code === TransferstatusTypeConstants.TransferToCoordinator);
    this.sessionsFHI = this.sessions.filter(x => x.transferStatus.code === TransferstatusTypeConstants.TransferToFhi);
  }

  transfer(sessionId) {
    this.loading = true;
    this.observationService.transferSessionToFHI(this.institution.id, sessionId).subscribe((result) => {
      if (result) {
        this.sessions.find(x => x.id === result.id).transferStatus = result.transferStatus;
        this.toastrService.success('The session(s) was transferred to FHI');
        this.updateLists();
        this.loading = false;
      }
      else this.toastrService.error('An error occurred during the transfer. Please try again.', '', { disableTimeOut: true});
    });
  }

  markAllSessions() {
    this.sessionsCoordinator.forEach(s => s.isSelected = true);
  }

  transferSessions() {
    let selectedSessions = this.sessionsCoordinator.filter(s => s.isSelected);
  
    selectedSessions.forEach(session => {
      this.transfer(session.id);
    });
  }

  reset(): void {
    this.selectedObserver = null;
    this.selectedSessiontype = null;
    this.fromDate = null;
    this.toDate = null;
    this.resetSearchresults();
  }

  resetSearchresults() {
    this.SearchDone = false;
    this.sessionsCoordinator = [];
    this.sessionsFHI = [];
    this.sessions = [];
  }

  observationDeleted(): void {
    this.getSessions();
  }

  hasSelectedAtLeastOneSession(): boolean {
    return this.sessionsCoordinator.some(s => s.isSelected);
  }

  compareFirstNameForUsers(a: User, b: User): number {
    if (a.firstName.toLowerCase() < b.firstName.toLowerCase()) return -1;
    if (a.firstName.toLowerCase() > b.firstName.toLowerCase()) return 1;
    return 0;
  }
}

