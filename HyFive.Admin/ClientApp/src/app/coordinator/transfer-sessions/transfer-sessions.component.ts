import { Component, OnInit, OnDestroy } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { FacilityReport } from '../../models/api/FacilityReport';
import { ObservationService } from '../../services/data/observation.service';
import { SessionType } from '../../models/api/SessionType';
import { faArrowRight, faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { User } from '../../models/api/User';
import { ToastrService } from 'ngx-toastr';
import { TransferStatusTypeConstants } from '../../models/api/TransferStatusTypeConstants';
import { Facility } from '../../models/api/Facility';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';

@Component({
  selector: 'app-transfer-sessions',
  templateUrl: './transfer-sessions.component.html'
})
export class TransferSessionsComponent implements OnInit, OnDestroy {

  faArrowRight = faArrowRight;
  faPaperPlane = faPaperPlane;

  sessionTypeOptions = [
    { name: "All", value: null },
    //{ name: "Protective Equipment", value: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
    { name: "Five Indications", value: SessionType.FiveIndications, type: SessionType[SessionType.FiveIndications] },
    { name: "Gloves", value: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "Hand Jewelry", value: SessionType.HandJewelry, type: SessionType[SessionType.HandJewelry] },
  ];

  transferStatusOptions = [
    { name: 'All', value: null },
    { name: 'Transferred to Admin', value: TransferStatusTypeConstants.TransferredToAdmin },
    { name: 'Transferred to Coordinator', value:  TransferStatusTypeConstants.TransferredToCoordinator },
  ];


  selectedSessiontype: SessionType = null;
  selectedTransferStatus: string | null = null;

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
    private readonly facilityService: FacilityService,
    private readonly observationService: ObservationService,
    private readonly toastrService: ToastrService,
    private readonly translate: TranslateService
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
      } as FacilityReport;

      this.translate.get(this.sessionTypeOptions.map(it => it.name)).pipe(take(1)).subscribe(() => {
        this.sessionTypeOptions = this.sessionTypeOptions.map(opt => {
          return {
            ...opt,
            name: this.translate.instant(opt.name)
          }
        })
      });

      this.facilityService.getObservers(this.facility.id).subscribe((observers) => {
        let editedObservers = [];
        for (const obs of observers) {
        let firstLast = `${obs.firstName} ${obs.lastName}`;
        if (obs.isDisabled) {
          firstLast += this.translate.instant(' (disabled user)');
        }
        const o = {...obs, firstLast}; 
        editedObservers.push(o);
        };
        editedObservers = editedObservers.toSorted(this.compareFirstNameForUsers);
        editedObservers = this.showDisabledObserversBottom(editedObservers);
        editedObservers.unshift({ id: null, firstLast: this.translate.instant("All") });
        this.observers = editedObservers;
      });
    });
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  showDisabledObserversBottom(observers): User[] {
    let observersList = observers.filter(o => o.isDisabled === false);
    let observersWhoAreDisabled = observers.filter(o => o.isDisabled);
    observersList.push.apply(observersList, observersWhoAreDisabled);
    return observersList;
  }

  getSessions() {
    this.loading = true;
    this.observationService.getSessionsForFacility(
      this.facility.id,
      this.selectedObserver,
      this.selectedSessiontype,
      this.selectedTransferStatus,
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
        this.toastrService.success(this.translate.instant('The session(s) was transferred'));
        this.updateLists();
        this.loading = false;
      }
      else this.toastrService.error(this.translate.instant('An error occurred during the transfer. Please try again.'), '', { disableTimeOut: true});
    });
  }

  get allSessionsSelected(): boolean {
    return (
      this.sessionsCoordinator.length > 0 &&
      this.sessionsCoordinator.every(s => s.isSelected === true)
    );
  }

  markAllSessions(): void {
    const shouldSelectAll = !this.allSessionsSelected;

    for (const s of this.sessionsCoordinator) {
      s.isSelected = shouldSelectAll;
    }
  }

  transferSessions() {
    let selectedSessions = this.sessionsCoordinator.filter(s => s.isSelected);
  
    for (const session of selectedSessions) {
      this.transfer(session.id);
    };
  }

  reset(): void {
    this.selectedObserver = null;
    this.selectedSessiontype = null;
    this.selectedTransferStatus = null; 
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

