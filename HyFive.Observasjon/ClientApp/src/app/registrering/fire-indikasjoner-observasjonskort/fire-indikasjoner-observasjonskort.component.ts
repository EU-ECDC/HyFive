import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { FourIndicationsSessionView } from '../../models/registration/FourIndications-session-view.model';
import { Activity } from '../../models/api/Activity';
import { Uuid } from '../../utils/uuid';
import { ActivityUnderRegistration, ObservationEventService } from '../../services/events/observation-event.service';
import { Animations } from '../../shared/animasjoner/animasjoner';
import { Card } from '../../models/registration/card.model';
import { faSave, faTrashAlt, faTimesCircle } from '@fortawesome/free-regular-svg-icons';
import { Colors } from '../../utils/colors';
import { BaseKortSwipe } from '../../shared/kort-swipe/kort-swipe';
import { faHandHoldingWater, faDivide, faEraser, faHandsWash } from '@fortawesome/free-solid-svg-icons';
import { Role } from '../../models/api/Role';
import { IndicationType } from '../../models/api/IndicationType';
import { ActivityTypeConstants } from 'src/app/models/api/ActivityTypeConstants';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityType } from '../../models/api/ActivityType';
import { faTimes } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Activities } from '../../constants/Activities';


@Component({
  selector: 'app-fire-indikasjoner-observasjonskort',
  templateUrl: './fire-indikasjoner-observasjonskort.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class FireIndikasjonerObservasjonskortComponent extends BaseKortSwipe implements OnInit {

  ActivityTypeConstants = ActivityTypeConstants;
  comment: string;
  activity: Activity;
  activityTypes: ActivityType[];
  valgteIndikasjoner: IndicationType[] = new Array();
  aktivitetUnderRegistrering: ActivityUnderRegistration = null;
  observasjonMangelTekst: string;
  visInfoModal: boolean = false;
  dialogueTexts = DialogueTexts;
  sprit: string = Activities.Alcohol;
  vask: string = Activities.Wash;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faHandHoldingWater = faHandHoldingWater;
  faHandsWash = faHandsWash;
  faDivide = faDivide;
  faTimes = faTimes;
  faTimesCircle = faTimesCircle;
  colors = Colors;

  @Input("card") card: Card;
  @Input("roleSelected") roleSelected: Role[]
  @Input("sessionView") sessionView: FourIndicationsSessionView

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Card>();

  constructor(
    private observationEventService: ObservationEventService,
    private activityService: ActivityService,
    protected modalService: NgbModal
  ) {
    super(modalService);
  }

  ngOnInit(): void {
    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });
    this.observationEventService.registrationActivityHasBegun.subscribe(activity => {
      if (activity.parentId == this.card.id) {
        this.aktivitetUnderRegistrering = activity;
      }
    })
  }

  slettKort() {
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  velgRolle(role: Role) {
    this.card.role = role;
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card[kortIndex] = this.card;
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  nullstillKort() {
    this.comment = "";
    this.valgteIndikasjoner = [];
    this.activity = null;
    this.aktivitetUnderRegistrering = null;
    this.observationEventService.observationResetEvent.emit(this.card.id);
  }

  kanIkkeLagre(): boolean {
    if (this.valgteIndikasjoner?.length === 0) {
      this.observasjonMangelTekst = "Indikasjon(er) mangler";
      this.visInfoModal = true;
    }
    else if (!this.activity) {
      this.observasjonMangelTekst = "Activity mangler";
      this.visInfoModal = true;
    }
    return !(this.valgteIndikasjoner?.length && this.activity);
  }

  registerObservation() {
    let observation: FourIndicationsObservation = {
      id: Uuid.generateUUID(),
      sessionId: this.sessionView.sessionId,
      comment: this.comment,
      role: this.card.role,
      activity: this.activity,
      indicationTypes: this.valgteIndikasjoner,
      registrationTime: new Date(Date.now())
    }

    this.observasjonRegistrert.emit(observation);

    this.nullstillKort();

    this.card.isActive = false;
  }

  registerActivity(activity: Activity) {
    this.activity = activity;
  }

  fjernTidsregistrering(){
    this.activity.timeSpent = 0;
    this.activity.timeRecordingWasDone = false;
  }
  indikasjonsValgChanged(valgteIndikasjoner: IndicationType[]) {
    this.valgteIndikasjoner = valgteIndikasjoner;
  }

  registerComment(comment: string) {
    this.comment = comment;
  }

  skalAktivitetDeaktiveres(aktivitetTypeKode: string) {
    return this.activity != null
      || (this.aktivitetUnderRegistrering != null && this.aktivitetUnderRegistrering.activityType.code != aktivitetTypeKode)
  }

  isRegistered(aktivitetTypeKode: string) {
    return this.activity?.activityType.code == aktivitetTypeKode;
  }

  cardIsSelected() {
    this.card.isActive = true;
    this.kortErValgtEvent.emit(this.card);
  }

  getAktivitetType(code: string) {
    return this.activityTypes?.find(x => x.code === code);
  }

  closeInfoModal(erVisInfoModal): void {
    this.visInfoModal = erVisInfoModal;
  }
}
