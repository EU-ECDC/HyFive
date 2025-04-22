import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TransferstatusTypeConstants } from '../../../models/api/TransferstatusTypeConstants';
import { SessionType } from '../../../models/api/SessionType';
import { FourIndicationsObservation } from '../../../models/api/FourIndicationsObservation';
import { Role } from '../../../models/api/Role';
import { IndicationType } from '../../../models/api/IndicationType';
import { ActivityType } from '../../../models/api/ActivityType';
import { AktivitetTypeKonstanter } from '../../../models/api/AktivitetTypeKonstanter';
import { ObservationService } from '../../../services/data/observation.service';
import { ToastrService } from 'ngx-toastr';
import {BraceletObservation} from "../../../models/api/BraceletObservation";
import {Department} from "../../../models/api/Department";
import {KeyEventService} from "../../../services/events/key-event.service";

@Component({
  selector: 'app-edit-four-indications-observations',
  templateUrl: './edit-four-indications-observations.component.html'
})
export class EditFourIndicationsObservationsComponent implements OnInit {

  @Input() observations: FourIndicationsObservation[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;
  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  fourIndicationsObservationWhichChanged: FourIndicationsObservation = null;
  handsmykkeObservasjonSomEndres: BraceletObservation = null;

  secondsUsed: number;

  kanLagres = true;
  transferstatusTypeConstants = TransferstatusTypeConstants;
  SessionType = SessionType;

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.fourIndicationsObservationWhichChanged)
        this.fourIndicationsObservationWhichChanged= null;
    });
  }

  velgObservasjon(observasjon: FourIndicationsObservation) {
    if(!this.canEdit){
      return;
    }

    this.fourIndicationsObservationWhichChanged = JSON.parse(JSON.stringify(observasjon));
    this.fourIndicationsObservationWhichChanged.sessionId = this.sessionId;
  }

  velgRolle(rolle: Role) {
    this.fourIndicationsObservationWhichChanged.role = rolle;
  }

  indikasjonsValgChanged(valgteIndikasjoner: IndicationType[]) {
    this.fourIndicationsObservationWhichChanged.indicationTypes = valgteIndikasjoner;
  }

  velgAktivitet(ActivityType: ActivityType) {
    this.fourIndicationsObservationWhichChanged.activity.activityType = ActivityType;
  }

  endretSekunderBrukt(secondsUsed: number) {
    this.fourIndicationsObservationWhichChanged.activity.secondsUsed = secondsUsed;
  }

  endretKommentar(kommentar: string) {
    this.fourIndicationsObservationWhichChanged.comment = kommentar;
  }

  updateFourIndicationsObservation() {
    if (this.fourIndicationsObservationWhichChanged.activity.activityType.code === AktivitetTypeKonstanter.IkkeUtfort
      || this.fourIndicationsObservationWhichChanged.activity.activityType.code === AktivitetTypeKonstanter.IkkeRegistrert) {
      this.fourIndicationsObservationWhichChanged.activity.secondsUsed = 0;
      this.fourIndicationsObservationWhichChanged.activity.TimekeepingWasRemoved = false;
    }
    else {
      this.fourIndicationsObservationWhichChanged.activity.usedGlove = null;
      if (this.fourIndicationsObservationWhichChanged.activity.secondsUsed <= 0){
        this.fourIndicationsObservationWhichChanged.activity.secondsUsed = 0;
        this.fourIndicationsObservationWhichChanged.activity.TimekeepingWasRemoved = false;
      }
      else {
        this.fourIndicationsObservationWhichChanged.activity.TimekeepingWasRemoved = true;
      }
    }

    if (this.kanLagres) {
      this.observationService.updateFourIndicationsObservation(this.fourIndicationsObservationWhichChanged).subscribe(
        (erOppdatert) => {
          this.fourIndicationsObservationWhichChanged = null;
          this.toastrService.success('Observasjonen ble oppdatert');
          this.observationUpdatedEvent.emit();
        },
        (error) => {
          this.toastrService.error(error?.error ? error.error : error, 'Feil ved oppdatering av observasjon', { disableTimeOut: true});
        }
      );
    }
  }

  deleteFourIndicationsObservation() {
    this.observationService.deleteFourIndicationsObservation(this.fourIndicationsObservationWhichChanged.id, this.sessionId).subscribe(
      () => {
        this.fourIndicationsObservationWhichChanged = null;
        this.toastrService.success('Observasjonen ble slettet');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error,'Feil ved sletting av observasjon', { disableTimeOut: true});
      });
  }

  avbrytRedigeringAvObservasjon(event) {
    event.stopPropagation();
    this.fourIndicationsObservationWhichChanged = null;
  }
}
