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
import { BaseCardSwipe } from '../../shared/card-swipe/card-swipe';
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
  selector: 'app-four-indications-observation-card',
  templateUrl: './four-indications-observation-card.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class FourIndicationsObservationCardComponent extends BaseCardSwipe implements OnInit {

  ActivityTypeConstants = ActivityTypeConstants;
  comment: string;
  activity: Activity;
  activityTypes: ActivityType[];
  selectedIndications: IndicationType[] = new Array();
  activityUnderRegistration: ActivityUnderRegistration = null;
  observationMissingText: string;
  showInfoModal: boolean = false;
  dialogueTexts = DialogueTexts;
  alcohol: string = Activities.Alcohol;
  wash: string = Activities.Wash;

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

  @Output() observationRegister = new EventEmitter();
  @Output() observationUpdate = new EventEmitter();
  @Output() sessionViewUpdate = new EventEmitter();
  @Output() cardIsSelectedEvent = new EventEmitter<Card>();

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
        this.activityUnderRegistration = activity;
      }
    })
  }

  deleteCard() {
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(cardIndex, 1);
    this.sessionViewUpdate.emit(this.sessionView);
  }

  selectRole(role: Role) {
    this.card.role = role;
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card[cardIndex] = this.card;
    this.sessionViewUpdate.emit(this.sessionView);
  }

  resetCard() {
    this.comment = "";
    this.selectedIndications = [];
    this.activity = null;
    this.activityUnderRegistration = null;
    this.observationEventService.observationResetEvent.emit(this.card.id);
  }

  canNotSave(): boolean {
    if (this.selectedIndications?.length === 0) {
      this.observationMissingText = "Indication(s) missing";
      this.showInfoModal = true;
    }
    else if (!this.activity) {
      this.observationMissingText = "Activity missing";
      this.showInfoModal = true;
    }
    return !(this.selectedIndications?.length && this.activity);
  }

  registerObservation() {
    let observation: FourIndicationsObservation = {
      id: Uuid.generateUUID(),
      sessionId: this.sessionView.sessionId,
      comment: this.comment,
      role: this.card.role,
      activity: this.activity,
      indicationTypes: this.selectedIndications,
      registrationTime: new Date(Date.now())
    }

    this.observationRegister.emit(observation);

    this.resetCard();

    this.card.isActive = false;
  }

  registerActivity(activity: Activity) {
    this.activity = activity;
  }

  removeTimeRegistration(){
    this.activity.timeSpent = 0;
    this.activity.timeRecordingWasDone = false;
  }
  indicationSelectionChanged(selectedIndications: IndicationType[]) {
    this.selectedIndications = selectedIndications;
  }

  registerComment(comment: string) {
    this.comment = comment;
  }

  ShouldActivityDeactivated(activityTypeCode: string) {
    return this.activity != null
      || (this.activityUnderRegistration != null && this.activityUnderRegistration.activityType.code != activityTypeCode)
  }

  isRegistered(activityTypeCode: string) {
    return this.activity?.activityType.code == activityTypeCode;
  }

  cardIsSelected() {
    this.card.isActive = true;
    this.cardIsSelectedEvent.emit(this.card);
  }

  getAktivityType(code: string) {
    return this.activityTypes?.find(x => x.code === code);
  }

  closeInfoModal(erVisInfoModal): void {
    this.showInfoModal = erVisInfoModal;
  }
}
