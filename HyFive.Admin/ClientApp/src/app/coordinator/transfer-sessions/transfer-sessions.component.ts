import { Component, OnInit, OnDestroy } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { FacilityReport } from '../../models/api/FacilityReport';
import { ObservationService } from '../../services/data/observation.service';
import { SessionType } from '../../models/api/SessionType';
import { faArrowRight } from '@fortawesome/free-solid-svg-icons';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { User } from '../../models/api/User';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { TransferStatusTypeConstants } from '../../models/api/TransferStatusTypeConstants';
import { Facility } from '../../models/api/Facility';
import { forEach } from 'lodash-es';

@Component({
  selector: 'app-transfer-sessions',
  templateUrl: './transfer-sessions.component.html'
})
export class TransferSessionsComponent implements OnInit, OnDestroy {

  faArrowRight = faArrowRight;
  faPaperPlane = faPaperPlane;

  sessionTypeOptions = [
    { name: "All", value: null },
    { name: "Protective Equipment", value: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
    { name: "Five Indications", value: SessionType.FiveIndications, type: SessionType[SessionType.FiveIndications] },
    { name: "Gloves", value: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "Hand Jewelry", value: SessionType.HandJewelry, type: SessionType[SessionType.HandJewelry] },
  ];

  selectedSessiontype: SessionType = null;
  fromDate: Date = null;
  toDate: Date = null;

  observers = [];
  selectedObserver = null;

  facility: FacilityReport;

  facilitiesOptions: FacilityReport[] = [];
  selectedFacilityOptions: number = null;

  sessions: SessionOverviewReport[] = [];
  sessionsCoordinator: SessionOverviewReport[] = [];
  sessionsFHI: SessionOverviewReport[] = [];
  loading: boolean = false;
  SearchDone: boolean = false;

  constructor(
    private facilityService: FacilityService,
    private observationService: ObservationService,
    private toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    this.facilityService.getFacility(selectedFacilityId).subscribe((result: Facility) => {
      this.facility = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        facilityType: result.facilityType,
        name: result.name,
        region: result.region
      } as FacilityReport;

      this.facilityService.getObservers(this.facility.id).subscribe((observers) => {
        let editedObservers = [];
        observers.forEach(obs => {
        let firstLast = `${obs.firstName} ${obs.lastName}`;
        if (obs.isDisabled) {
          firstLast += ' (disabled user)';
        }
        const o = {...obs, firstLast}; 
        editedObservers.push(o);
        });
        editedObservers = editedObservers.sort(this.compareFirstNameForUsers);
        editedObservers = this.showDisabledObserversBottom(editedObservers);
        editedObservers.unshift({ id: null, firstLast: "All" });
        this.observers = editedObservers;
      });
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  showDisabledObserversBottom(observers): User[] {
    var observersList = observers.filter(o => o.isDisabled === false);
    var observersWhoAreDisabled = observers.filter(o => o.isDisabled);
    observersList.push.apply(observersList, observersWhoAreDisabled);
    return observersList;
  }

  getSessions() {
    this.loading = true;
    this.observationService.getSessionsForFacility(
      this.facility.id,
      this.selectedObserver,
      this.selectedSessiontype,
      this.fromDate,
      this.toDate
    ).subscribe((results) => {
      this.loading = false;
      this.SearchDone = true;
      this.sessions = results;
      this.updateLists();
    },
    (error) => {
      this.loading = false;
    })
  }

  updateLists() {
    this.sessionsCoordinator = this.sessions.filter(x => x.transferStatus.code === TransferStatusTypeConstants.TransferredToCoordinator);
    this.sessionsFHI = this.sessions.filter(x => x.transferStatus.code === TransferStatusTypeConstants.TransferredToAdmin);
  }

  transfer(sessionId) {
    this.loading = true;
    this.observationService.transferSessionToFHI(this.facility.id, sessionId).subscribe((result) => {
      if (result) {
        this.sessions.find(x => x.id === result.id).transferStatus = result.transferStatus;
        this.toastrService.success('The session(s) was transferred');
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

