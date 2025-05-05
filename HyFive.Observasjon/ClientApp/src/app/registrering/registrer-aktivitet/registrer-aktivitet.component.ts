import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Activity } from '../../models/api/Activity';
import { ObservationEventService } from '../../services/events/observation-event.service';
import { faClock } from '@fortawesome/free-solid-svg-icons';
import { ActivityType } from '../../models/api/ActivityType';
import { ActivityTypeConstants } from '../../models/api/ActivityTypeConstants';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-registrer-aktivitet',
  templateUrl: './registrer-aktivitet.component.html'
})

export class RegistrerAktivitetComponent implements OnInit {

  timerErStartet: boolean = false;
  interval;
  tidtakingUtfores: boolean = false;
  faClock = faClock;
  visTekst: boolean = true;

  benyttetHansker?: boolean = null;

  @Input("parentId") parentId: string;
  @Input("tidtaking") tidtaking: boolean;
  @Input("deaktivert") deaktivert: boolean;
  @Input("activityType") activityType: ActivityType;
  @Input("seconds") seconds: number;
  @Input("erRegistrert") erRegistrert: boolean;
  @Input("bekreftelseModalSkalVises") bekreftelseModalSkalVises: boolean;
  @Input("icon") icon: string;

  @Output() aktivitetRegistertEvent = new EventEmitter<Activity>();

  constructor(private observationEventService: ObservationEventService, private modalService: NgbModal) {
  }

  ngOnInit(): void {
    if (this.seconds && this.seconds > 0) {
      this.visTekst = false;
    }
    this.observationEventService.observationResetEvent.subscribe((parentId) => {
      if (parentId === this.parentId) {
        this.nullstillKomponent();
      }
    })
  }

  getAktivitetTekst() {
    if (this.activityType?.code === ActivityTypeConstants.Desinfeksjon)
      return 'Alcohol';
    if (this.activityType?.code === ActivityTypeConstants.Handvask)
      return 'Wash';
    if(this.activityType?.code === ActivityTypeConstants.NotExecuted)
      return 'Ikke utført';
  }

  registrerAktivitet(modalName) {

    if (this.bekreftelseModalSkalVises && !this.tidtakingUtfores) {
      this.modalService.open(modalName, { windowClass: 'hh-modal' });
    }
    else {
      if (this.tidtaking) {
        this.startEllerStoppTimer();
        this.visTekst = false;
      }
      else {
        this.aktivitetRegistertEvent.emit({ activityType: this.activityType, timeRecordingWasDone: false });
      }
    }
  }

  bekrefteRegistrering(bleBekreftet: boolean) {

    this.benyttetHansker = bleBekreftet;

    if (this.tidtaking) {
      this.startEllerStoppTimer();
      this.visTekst = false;
    }
    else {
      this.aktivitetRegistertEvent.emit({ activityType: this.activityType, timeRecordingWasDone: false, gloveUsed: bleBekreftet});
    }
    
  }

  startEllerStoppTimer() {
    this.observationEventService.registrationActivityHasBegun.emit({ parentId: this.parentId, activityType: this.activityType })
    if (this.timerErStartet === false) {
      this.startTimer();
      this.tidtakingUtfores = true;
    }
    else if (this.seconds > 0) {
      this.stoppTimer();
      this.tidtakingUtfores = false;
      this.aktivitetRegistertEvent.emit({ activityType: this.activityType, timeSpent: this.seconds, timeRecordingWasDone: true, gloveUsed: this.benyttetHansker})
    }
  }

  private nullstillKomponent() {
    this.stoppTimer();
    this.seconds = 0;
    this.visTekst = true;
    this.tidtakingUtfores = false;
  }

  private stoppTimer() {
    this.timerErStartet = false;
    clearInterval(this.interval);
    this.interval = 0;
  }

  private startTimer() {
    this.timerErStartet = true;
    this.seconds = 0;
    this.interval = setInterval(() => {
      this.seconds++;
    }, 1000);
  }
}
