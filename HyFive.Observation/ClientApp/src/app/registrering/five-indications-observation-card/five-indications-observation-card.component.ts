import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FiveIndicationsObservation } from '../../models/api/FiveIndicationsObservation';
import { FiveIndicationsSessionView } from '../../models/registration/FiveIndications-session-view.model';
import { Activity } from '../../models/api/Activity';
import { Uuid } from '../../utils/uuid';
import { ActivityUnderRegistration, ObservationEventService } from '../../services/events/observation-event.service';
import { Animations } from '../../shared/animations/animations';
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
import { SessionType } from 'src/app/models/api/SessionType';


@Component({
  selector: 'app-five-indications-observation-card',
  templateUrl: './five-indications-observation-card.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class FiveIndicationsObservationCardComponent extends BaseCardSwipe implements OnInit {

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
  fiveIndicationsSessionType: number = SessionType.FiveIndications;
  institutionid: number;

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
  @Input("sessionView") sessionView: FiveIndicationsSessionView

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
    this.institutionid = this.sessionView.department.institutionId;
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
    let observation: FiveIndicationsObservation = {
      id: Uuid.generateUUID(),
      sessionId: this.sessionView.sessionId,
      comment: this.comment,
      role: this.card.role,
      activity: this.activity,
      indicationTypes: this.selectedIndications,
      registeredTime: new Date(Date.now())
    }

    this.observationRegister.emit(observation);

    this.resetCard();

    this.card.isActive = false;
  }

  registerActivity(activity: Activity) {
    this.activity = activity;
  }

  removeTimeRegistration(){
    this.activity.secondsUsed = 0;
    this.activity.TimingWasPerformed = false;
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

  getActivityType(code: string) {
    return this.activityTypes?.find(x => x.code === code);
  }

  closeInfoModal(erVisInfoModal): void {
    this.showInfoModal = erVisInfoModal;
  }
}
