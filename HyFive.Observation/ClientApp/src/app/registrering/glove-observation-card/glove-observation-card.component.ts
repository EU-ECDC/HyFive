import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faCheck, faCircle, faEraser  } from '@fortawesome/free-solid-svg-icons';
import { faHandPaper } from "@fortawesome/free-regular-svg-icons";
import { Role } from "src/app/models/api/Role";
import { Card } from "src/app/models/registration/card.model";
import { Animations } from "../../shared/animations/animations";
import { BaseCardSwipe } from "../../shared/card-swipe/card-swipe";
import { Colors } from "../../utils/colors";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GloveSession } from "../../models/api/GloveSession";
import { GloveSessionView } from "../../models/registration/glove-session-view.model";
import { GloveObservation } from "../../models/api/GloveObservation";
import { GloveWithIndicationTypeService } from "../../services/data/glove-with-indication-type.service";
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';
import { GloveWithoutIndicationTypeService } from "../../services/data/glove-without-indication-type.service";
import { GloveWithoutIndicationType } from "../../models/api/GloveWithoutIndicationType";
import { PostGloveHandHygieneTypeService } from "../../services/data/post-glove-hand-hygiene-type-service";
import { PostGloveHandHygieneType } from "../../models/api/PostGloveHandHygieneType";
import { DialogueTexts } from '../../constants/dialogueTexts';
import { SessionType } from "src/app/models/api/SessionType";
import { TranslateService } from "@ngx-translate/core";

@Component({
  selector: 'app-glove-observation-card',
  templateUrl: './glove-observation-card.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class GloveObservationCardComponent extends BaseCardSwipe implements OnInit {

  comment: string;
  observationMissingText: string;
  showInfoModal: boolean = false;
  dialogueTexts = DialogueTexts;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;
  faTimes = faTimes;
  faHandPaper = faHandPaper;
  colors = Colors;

  sessionsdata: GloveSession = null;
  roles: Role[];
  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[] = [];
  postGloveHandHygieneTypes: PostGloveHandHygieneType[] = [];
  activeTab = "with";
  glovesUsed = null;
  selectedHandHygieneAfterGlovesUsed = null;
  glovesSessionType: number = SessionType.Gloves;
  facilityid: number;
  
  uuid: string;

  @Input() card: Card;
  @Input() roleSelected: Role[];
  @Input() sessionView: GloveSessionView;

  @Output() observationRegister = new EventEmitter();
  @Output() observationUpdate = new EventEmitter();
  @Output() sessionViewUpdate = new EventEmitter();
  @Output() cardIsSelectedEvent = new EventEmitter<Card>();

  constructor(
    private readonly gloveWithIndicationTypeService: GloveWithIndicationTypeService,
    private readonly gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,
    private readonly postGloveHandHygieneTypeService: PostGloveHandHygieneTypeService,
    protected modalService: NgbModal,
    protected translate: TranslateService
  ) {
    super(modalService, translate);
  }

  ngOnInit(): void {
    this.gloveWithIndicationTypeService.getGloveWithIndicationTypes().subscribe((gloveWithIndicationTypes) => {
      this.gloveWithIndicationTypes = gloveWithIndicationTypes.toSorted((a, b) => b.id - a.id);
    });
    this.gloveWithoutIndicationTypeService.getGloveWithoutIndicationTypes().subscribe((gloveWithoutIndicationTypes) => {
      this.gloveWithoutIndicationTypes = gloveWithoutIndicationTypes;
    });
    this.postGloveHandHygieneTypeService.getPostGloveHandHygieneTypes().subscribe((postGloveHandHygieneTypes) => {
      this.postGloveHandHygieneTypes = postGloveHandHygieneTypes.toSorted((a, b) => a.id - b.id);
    });
    this.uuid = Uuid.generateUUID();
    this.facilityid = this.sessionView.department.facilityId;
  }

  deleteCard() {
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(cardIndex, 1);
    this.sessionViewUpdate.emit(this.sessionView);
  }

  resetTab() {
    this.gloveWithIndicationTypes.forEach(x => x.isSelected = false);
    this.gloveWithoutIndicationTypes.forEach(x => x.isSelected = false);
    this.glovesUsed = this.activeTab === "with" ? null : true;
    this.selectedHandHygieneAfterGlovesUsed = null;
  }

  gloveWithIndicationsChanged(code, event) {
    this.gloveWithIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  gloveWithoutIndicationsChanged(code, event) {
    this.gloveWithoutIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  registerComment(comment: string) {
    this.comment = comment;
  }

  resetCard() {
    this.resetTab();
    this.comment = "";
  }

  canNotSave(): boolean {

    if (this.activeTab === "with") {
      if (this.gloveWithIndicationTypes.filter(h => h.isSelected).length === 0) {
        this.observationMissingText = this.translate.instant("Indication(s) missing");
        this.showInfoModal = true;
        return true;
      }
      else if (this.glovesUsed === null) {
        this.observationMissingText = this.translate.instant("'Gloves used?' must be answered");
        this.showInfoModal = true;
        return true;
      }
    }
    else {
      if (this.gloveWithoutIndicationTypes.filter(h => h.isSelected).length === 0) {
        this.observationMissingText = this.translate.instant("Indication(s) missing");
        this.showInfoModal = true;
        return true;
      }
    }
  }

  selectRole(role: Role) {
    this.card.role = role;
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card[cardIndex] = this.card;
    this.sessionViewUpdate.emit(this.sessionView);
  }

  registerObservation() {
    let observation = {
      id: Uuid.generateUUID(),
      sessionId: this.sessionView.sessionId,
      registeredTime: new Date(Date.now()),
      role: this.card.role,
      comment: this.comment,
      gloveWithIndicationTypes: this.gloveWithIndicationTypes.filter(x => x.isSelected),
      gloveWithoutIndicationTypes: this.gloveWithoutIndicationTypes.filter(x => x.isSelected),
      glovesUsed: this.glovesUsed,
      postGloveHandHygieneType: this.glovesUsed ? this.postGloveHandHygieneTypes.find(x => x.code === this.selectedHandHygieneAfterGlovesUsed) : null,
    } as GloveObservation;

    this.observationRegister.emit(observation);

    this.resetCard();

    this.card.isActive = false;
  }

  cardIsSelected() {
    this.card.isActive = true;
    this.cardIsSelectedEvent.emit(this.card);
  }

  closeInfoModal(erVisInfoModal): void {
    this.showInfoModal = erVisInfoModal;
  }
}

