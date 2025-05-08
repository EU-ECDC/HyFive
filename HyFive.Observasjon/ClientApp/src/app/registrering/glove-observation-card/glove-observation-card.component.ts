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
import { HandHygieneAfterGloveUseTypeService } from "../../services/data/hand-hygiene-after-glove-useType-service";
import { HandHygieneAfterGloveUseType } from "../../models/api/HandHygieneAfterGloveUseType";
import { DialogueTexts } from '../../constants/dialogueTexts';

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
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];
  activeTab = "with";
  gloveUsed = null;
  selectedHandHygieneAfterGloveUsed = null;
  
  uuid: string;

  @Input("card") card: Card;
  @Input("roleSelected") roleSelected: Role[];
  @Input("sessionView") sessionView: GloveSessionView;

  @Output() observationRegister = new EventEmitter();
  @Output() observationUpdate = new EventEmitter();
  @Output() sessionViewUpdate = new EventEmitter();
  @Output() cardIsSelectedEvent = new EventEmitter<Card>();

  constructor(
    private gloveWithIndicationTypeService: GloveWithIndicationTypeService,
    private gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,
    private handHygieneAfterGloveUseTypeService: HandHygieneAfterGloveUseTypeService,
    protected modalService: NgbModal
  ) {
    super(modalService);
  }

  ngOnInit(): void {
    this.gloveWithIndicationTypeService.getGloveWithIndicationTypes().subscribe((gloveWithIndicationTypes) => {
      this.gloveWithIndicationTypes = gloveWithIndicationTypes;
    });
    this.gloveWithoutIndicationTypeService.getGloveWithoutIndicationTypes().subscribe((gloveWithoutIndicationTypes) => {
      this.gloveWithoutIndicationTypes = gloveWithoutIndicationTypes;
    });
    this.handHygieneAfterGloveUseTypeService.getHandhygieneAfterGloveUseTypes().subscribe((handHygieneAfterGloveUseTypes) => {
      this.handHygieneAfterGloveUseTypes = handHygieneAfterGloveUseTypes;
    });
    this.uuid = Uuid.generateUUID();
  }

  deleteCard() {
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(cardIndex, 1);
    this.sessionViewUpdate.emit(this.sessionView);
  }

  resetTab() {
    this.gloveWithIndicationTypes.forEach(x => x.isSelected = false);
    this.gloveWithoutIndicationTypes.forEach(x => x.isSelected = false);
    this.gloveUsed = this.activeTab === "with" ? null : true;
    this.selectedHandHygieneAfterGloveUsed = null;
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
        this.observationMissingText = "Indication(s) missing";
        this.showInfoModal = true;
        return true;
      }
      else if (this.gloveUsed === null) {
        this.observationMissingText = "\"Glove used?\" must be answered";
        this.showInfoModal = true;
        return true;
      }
    }
    else {
      if (this.gloveWithoutIndicationTypes.filter(h => h.isSelected).length === 0) {
        this.observationMissingText = "Indication(s) missing";
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
      registrationTime: new Date(Date.now()),
      role: this.card.role,
      comment: this.comment,
      gloveWithIndicationTypes: this.gloveWithIndicationTypes.filter(x => x.isSelected),
      gloveWithoutIndicationTypes: this.gloveWithoutIndicationTypes.filter(x => x.isSelected),
      gloveUsed: this.gloveUsed,
      handHygieneAfterGloveUseType: this.gloveUsed ? this.handHygieneAfterGloveUseTypes.find(x => x.code === this.selectedHandHygieneAfterGloveUsed) : null,
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

