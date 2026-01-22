import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output, OnDestroy } from '@angular/core';
import { faPaperPlane } from '@fortawesome/free-solid-svg-icons';
import { TransferStatusTypeConstants } from '../../models/api/TransferStatusTypeConstants';
import { SessionOverviewReport } from '../../models/api/SessionOverviewReport';
import { SessionType } from '../../models/api/SessionType';
import { ObservationService } from '../../services/data/observation.service';
import { ToastrService } from 'ngx-toastr';
import {SessionService} from '../../services/data/session.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-overview-sessions-view',
  templateUrl: './overview-sessions-view.component.html'
})
export class OverviewSessionsViewComponent implements OnDestroy {

  faPaperPlane = faPaperPlane;

  @Input() session: SessionOverviewReport[] = [];
  @Input() isTransferOverview = false;

  @Output() transferEvent = new EventEmitter();
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  istransferToFhi = false;
  transferstatusTypeConstants = TransferStatusTypeConstants;
  SessionType = SessionType;

  constructor(
    private readonly observationService: ObservationService,
    private readonly sessionService: SessionService,
    private readonly toastrService: ToastrService,
    private readonly datePipe: DatePipe,
    private readonly translate: TranslateService) { }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  isTransferToFhi(code) {
    return code === TransferStatusTypeConstants.TransferredToAdmin;
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
    const errorMessage = this.translate.instant("An error occurred while deleting session with id") + ` ${sessionOverviewReport.id}`;
    this.sessionService.deleteSession(sessionOverviewReport.id, sessionOverviewReport.department.facilityId).subscribe(
      (isDeleted) => {
        if (isDeleted){
          this.toastrService.success(this.translate.instant("Session with id") + ` ${sessionOverviewReport.id} ` + this.translate.instant("was deleted"));
          this.session = this.session.filter((s) => s.id !== sessionOverviewReport.id);
        }
        else {
          this.toastrService.error(errorMessage, '', { disableTimeOut: true});
        }
      },
      (error) => {
        this.toastrService.error(errorMessage + ' : ' + error?.error.message ? error?.error.message : error, '',  { disableTimeOut: true});
      }
    );
  }

  getDeleteMessage(sessionOverviewReport: SessionOverviewReport) {
    return this.translate.instant("You are about to delete session registered by") +  ` ${sessionOverviewReport.observerName}, `
    + this.translate.instant("created") + ` ${this.datePipe.transform(sessionOverviewReport.createdDate, 'dd.MM.yyyy HH:mm')} ` + this.translate.instant("by") + ` ${sessionOverviewReport.observations?.length} ` + this.translate.instant("associated observation") + ` ${sessionOverviewReport.observations?.length > 1 ? this.translate.instant('is') : ''}. `
    + this.translate.instant("Are you sure you want to delete this session?");
  }
}
