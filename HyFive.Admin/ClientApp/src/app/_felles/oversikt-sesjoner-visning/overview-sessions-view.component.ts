import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output, OnDestroy } from '@angular/core';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { TransferstatusTypeConstants } from '../../models/api/TransferstatusTypeConstants';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { SessionType } from '../../models/api/SessionType';
import { ObservationService } from '../../services/data/observation.service';
import { ToastrService } from 'ngx-toastr';
import {SessionService} from '../../services/data/session.service';

@Component({
  selector: 'app-overview-sessions-view',
  templateUrl: './overview-sessions-view.component.html'
})
export class OverviewSessionsViewComponent implements OnInit, OnDestroy {

  faPaperPlane = faPaperPlane;

  @Input() session: SessionOverviewReport[] = [];
  @Input() isTransferOverview = false;

  @Output() transferEvent = new EventEmitter();
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  istransferToFhi = false;
  transferstatusTypeConstants = TransferstatusTypeConstants;
  SessionType = SessionType;

  constructor(
    private observationService: ObservationService,
    private sessionService: SessionService,
    private toastrService: ToastrService,
    private datePipe: DatePipe) { }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  isTransferToFhi(code) {
    return code === TransferstatusTypeConstants.TransferToFhi;
  }

  transfer(sessionId, event) {
    event.stopPropagation();
    this.transferEvent.emit(sessionId);
  }

  selectOverToFhi(sessionId, event) {
    event.stopPropagation();
    this.session.find(s => s.id === sessionId).isSelected = event.target.checked; 
  }

  deleteSession(sessionOverviewReport: SessionOverviewReport) {
    const errorMessage = `An error occurred while deleting session with id ${sessionOverviewReport.id}`;
    this.sessionService.deleteSession(sessionOverviewReport.id, sessionOverviewReport.department.institutionId).subscribe(
      (isDeleted) => {
        if (isDeleted){
          this.toastrService.success(`Session with id ${sessionOverviewReport.id} was deleted`);
          this.session = this.session.filter((s) => s.id !== sessionOverviewReport.id);
        }
        else {
          this.toastrService.error(errorMessage, '', { disableTimeOut: true});
        }
      },
      (error) => {
        this.toastrService.error(errorMessage + ' : ' + error?.error ? error.error : error, '',  { disableTimeOut: true});
      }
    );
  }

  getDeleteMessage(sessionOverviewReport: SessionOverviewReport) {
    return `You are about to delete session registered by ${sessionOverviewReport.observerName},
    created ${this.datePipe.transform(sessionOverviewReport.timeOfCreation, 'dd.MM.yyyy HH:mm')} by ${sessionOverviewReport.observations?.length} associated observation${sessionOverviewReport.observations?.length > 1 ? 'is' : ''}.
    Are you sure you want to delete this session?`;
  }
}
