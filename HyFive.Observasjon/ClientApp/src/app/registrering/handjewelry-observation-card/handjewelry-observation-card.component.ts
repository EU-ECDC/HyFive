import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { HandJewelrySessionView } from "src/app/models/registration/handJewelry-session-view.model";
import { HandJewelrySession } from "src/app/models/api/HandJewelrySession";
import { Role } from "src/app/models/api/Role";
import { HandJewelryObservation } from 'src/app/models/api/HandJewelryObservation';
import { Card } from "src/app/models/registration/card.model";
import { HandJewelryType } from '../../models/api/HandJewelryType';
import { Animations } from "../../shared/animations/animations";
import { BaseCardSwipe } from "../../shared/card-swipe/card-swipe";
import { Colors } from "../../utils/colors";
import { HandJewelrySelection } from "../../models/registration/handJewelry-selection.model";
import { HandJewelryMapper } from "../../utils/handJewelry-mapper";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { HandJewelryTypeConstants } from "../../models/api/HandJewelryTypeConstants";
import { HandJewelryTypeService } from "../../services/data/hand-jewelry-type.service";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DialogueTexts } from '../../constants/dialogueTexts';

@Component({
  selector: 'app-handjewelry-observation-card',
  templateUrl: './handjewelry-observation-card.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class HandJewelryObservationCardComponent extends BaseCardSwipe implements OnInit {

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
  colors = Colors;
  iconTypeMap: Map<HandJewelryTypeConstants, IconProp> = HandJewelryMapper.getIconTypeMap();

  sessionsdata: HandJewelrySession = null;
  roles: Role[];
  handJewelryTypes: HandJewelryType[] = [];

  handJewelrySelection = [] as HandJewelrySelection[];

  @Input("card") card: Card;
  @Input("roleSelected") roleSelected: Role[];
  @Input("sessionView") sessionView: HandJewelrySessionView;

  @Output() observationRegister = new EventEmitter();
  @Output() observationUpdate = new EventEmitter();
  @Output() sessionViewUpdate = new EventEmitter();
  @Output() cardIsSelectedEvent = new EventEmitter<Card>();

  constructor(
    private handJewelryTypeService: HandJewelryTypeService,
    protected modalService: NgbModal) {
    super(modalService);
  }

  ngOnInit(): void {
    this.handJewelryTypeService.getHandJewelryTypes().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
      this.handJewelrySelection = HandJewelryMapper.getHandjewelrySelection(this.handJewelryTypes, []);
    });
  }

  deleteCard() {
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(cardIndex, 1);
    this.sessionViewUpdate.emit(this.sessionView);
  }

  registerComment(comment: string) {
    this.comment = comment;
  }

  resetCard() {
    this.comment = "";
    this.handJewelrySelection = HandJewelryMapper.getHandjewelrySelection(this.handJewelryTypes, []);
  }

  numberOfHandJewelrySelected () {
    return this.handJewelrySelection.reduce((acc, curr) => { if (curr.isSelected) return acc + 1; return acc; }, 0);
  }

  canNotSave(): boolean {
    let numberOfHandJewelrySelected  = this.numberOfHandJewelrySelected ();

    if (numberOfHandJewelrySelected  < 1) {
      this.observationMissingText = "HandJewelry missing";
      this.showInfoModal = true;
    }
    return numberOfHandJewelrySelected  < 1;
  }

  changed(select: HandJewelrySelection) {
    if (select.isSelected && select.type == HandJewelryTypeConstants.AllClear)
      this.handJewelrySelection = this.handJewelrySelection.map(x => { if (x.type !== HandJewelryTypeConstants.AllClear) x.disabled = true; return x; }) // disable all
    else if (select.isSelected && select.type != HandJewelryTypeConstants.AllClear)
      this.handJewelrySelection = this.handJewelrySelection.map(x => { if (x.type === HandJewelryTypeConstants.AllClear) x.disabled = true; return x; }) // disable anyway
    else if (this.numberOfHandJewelrySelected () < 1)
      this.handJewelrySelection = this.handJewelrySelection.map(x => { x.disabled = false; return x; }) // enable all
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
      handJewelry: this.handJewelrySelection.reduce((acc, item) => {
        if (item.isSelected) acc.push(this.handJewelryTypes.find(x => x.code === item.type));
        return acc;
      }, [] as HandJewelryType[]) as HandJewelryType[],
      comment: this.comment
    } as HandJewelryObservation;

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

