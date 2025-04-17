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

  @Output() oppositeEvent = new EventEmitter();
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  isOppositeToFhi = false;
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

  isOverfortTilFhi(code) {
    return code === TransferstatusTypeConstants.OverfortTilFhi;
  }

  overfor(sesjonId, event) {
    event.stopPropagation();
    this.oppositeEvent.emit(sesjonId);
  }

  velgOverforTilFhi(sesjonId, event) {
    event.stopPropagation();
    this.session.find(s => s.id === sesjonId).erValgt = event.target.checked; 
  }

  deleteSession(sessionOverviewReport: SessionOverviewReport) {
    const feilmelding = `En feil skjedde under sletting av sesjon med id ${sessionOverviewReport.id}`;
    this.sessionService.deleteSession(sessionOverviewReport.id, sessionOverviewReport.avdeling.institutionId).subscribe(
      (erSlettet) => {
        if (erSlettet){
          this.toastrService.success(`Sesjon med id ${sessionOverviewReport.id} ble slettet`);
          this.session = this.session.filter((s) => s.id !== sessionOverviewReport.id);
        }
        else {
          this.toastrService.error(feilmelding, '', { disableTimeOut: true});
        }
      },
      (error) => {
        this.toastrService.error(feilmelding + ' : ' + error?.error ? error.error : error, '',  { disableTimeOut: true});
      }
    );
  }

  hentSlettemelding(sessionOverviewReport: SessionOverviewReport) {
    return `Du er i ferd med å slette sesjon registrert av ${sessionOverviewReport.observatorNavn},
    opprettet ${this.datePipe.transform(sessionOverviewReport.timeOfCreation, 'dd.MM.yyyy HH:mm')} med ${sessionOverviewReport.observasjoner?.length} tilhørende observasjon${sessionOverviewReport.observasjoner?.length > 1 ? 'er' : ''}.
    Er du sikker på at du vil slette denne sesjonen?`;
  }
}
