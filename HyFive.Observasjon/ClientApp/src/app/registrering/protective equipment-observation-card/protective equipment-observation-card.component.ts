import { Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Role } from "src/app/models/api/Role";
import { Card } from "src/app/models/registration/card.model";
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Colors } from "../../utils/colors";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { ProtectiveEquipmentSessionView } from "../../models/registration/protectiveEquipment-sessionView.model";
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import { ProtectiveEquipmentCard } from '../../models/registration/protectiveEquipment-card.model';
import { ProtectiveEquipmentMapper } from '../../utils/protectiveEquipment-mapper';
import { ProtectiveEquipmentSession } from '../../models/api/ProtectiveEquipmentSession';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ProtectiveEquipmentModalComponent, ProtectiveEquipmentModalComponentConfig } from "../protective-equipment-modal/protective-equipment-modal.component";
import { DialogueTexts } from '../../constants/dialogueTexts';
import { SessionType } from '../../models/api/SessionType';
import { CdkDragDrop } from "@angular/cdk/drag-drop";
import {ProtectiveEquipmentSessionService} from "../../services/data/protectiveEquipment-session.service";

@Component({
  selector: 'app-protective equipment-observation-card',
  templateUrl: './protective equipment-observation-card.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class ProtectiveEquipmentObservationCardComponent extends BaseKortSwipe implements OnInit {

  comment: string;
  observationDeficiencyText: string;
  showInfoModal: boolean = false;
  dialogueTexts = DialogueTexts;
  sessionsdata: ProtectiveEquipmentSession = null;
  roles: Role[];
  ProtectiveEquipmentSelection: ProtectiveEquipment[] = [];
  protectiveEquipmentSessionType: number = SessionType.ProtectiveEquipment;
  institutionid: number;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;
  faTimes = faTimes;
  colors = Colors;
  iconTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();

  @Input("card") card: ProtectiveEquipmentCard;
  @Input("roleSelected") roleSelected: Role[];
  @Input("sessionView") sessionView: ProtectiveEquipmentSessionView;

  @Output() observationRegister = new EventEmitter();
  @Output() observationUpdate = new EventEmitter();
  @Output() sessionViewUpdate = new EventEmitter();
  @Output() cardIsSelectedEvent = new EventEmitter<Card>();

  constructor(modalService: NgbModal,
              private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService) {
    super(modalService);
  }

  ngOnInit(): void {
    this.protectiveEquipmentSessionService.protectiveEquipmentUpdated.subscribe((bu) => {
      this.sessionView.setting.equipmentTypes = bu;

      this.updateProtectiveEquipmentSelection();
    })
    this.ProtectiveEquipmentSelection = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.sessionView.setting.equipmentTypes);
    this.institutionid = this.sessionView.department.institutionId;
  }



  deleteCard() {
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(cardIndex, 1);
    this.sessionViewUpdate.emit(this.sessionView);
  }

  ProtectiveEquipmentRequired(): ProtectiveEquipment[] {
    return this.ProtectiveEquipmentSelection.filter(b => b.isRequired);
  }

  ProtectiveEquipmentNotRequired(): ProtectiveEquipment[] {
    return this.ProtectiveEquipmentSelection.filter(b => b.isRequired === false);
  }

  registerComment(comment: string) {
    this.comment = comment;
  }

  resetCard() {
    this.comment = "";
    this.ProtectiveEquipmentSelection = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.sessionView.setting.equipmentTypes);
  }

  resetEquipment(select: ProtectiveEquipment) {
    let valgIndex = this.ProtectiveEquipmentSelection.findIndex(x => x.equipmentType.id === select.equipmentType.id);
    this.ProtectiveEquipmentSelection[valgIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.sessionView.setting.equipmentTypes).find(x => x.equipmentType.id === select.equipmentType.id);
  }


  canNotSave(): boolean {
    let numberOfQualifiedEquipment = this.protectiveEquipmentSessionService.numberOfQualifiedEquipment(this.ProtectiveEquipmentSelection);

    if (numberOfQualifiedEquipment < 1) {
      this.observationDeficiencyText = DialogueTexts.CanNotSaveProtectiveEquipmentObservation;
      this.showInfoModal = true;
    }
    return numberOfQualifiedEquipment < 1;
  }

  changed(event, select: ProtectiveEquipment) {
    this.cardLockedInPlace = false;
    event.srcElement.blur();
    event.preventDefault();

    if (select.wasUsed)
      this.showModal(select);
    else
      this.resetEquipment(select);
  }

  showModal(selectedEquipment: ProtectiveEquipment) {
    this.cardLockedInPlace = false;
    const modalRef = this.modalService.open(ProtectiveEquipmentModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: ProtectiveEquipmentModalComponentConfig.windowClass
    });

    modalRef.componentInstance.selectedEquipment = JSON.parse(JSON.stringify(selectedEquipment)) as ProtectiveEquipment;
    modalRef.componentInstance.selectedEquipment.wasUsedCorrectly = null;

    modalRef.result.then((result: ProtectiveEquipment) => {
      selectedEquipment.wasUsedCorrectly = result.wasUsedCorrectly;
      selectedEquipment.comment = result.comment;
      selectedEquipment.isRequired = result.isRequired;
      selectedEquipment.equipmentType.isRequired = result.isRequired;
      selectedEquipment.misuseTypes = result.equipmentType.misuseTypes.filter(x => x.isSelected);
      selectedEquipment.wasUsed = result.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '';
    }, (reason) => {
      selectedEquipment.wasUsed = false;
    });
  }

  setAllEquipmentToProperUsed(event) {
    this.ProtectiveEquipmentRequired().forEach(x => {
      x.wasUsed = true;
      x.wasUsedCorrectly = true;
    });
  }

  selectRole(role: Role) {
    this.card.role = role;
    let cardIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card[cardIndex] = this.card;
    this.sessionViewUpdate.emit(this.sessionView);
  }

  registerObservation() {
    let observation: ProtectiveEquipmentObservation = {
      id: Uuid.generateUUID(),
      role: this.card.role,
      registrationTime: new Date(Date.now()),
      sessionId: this.sessionView.sessionId,
      comment: this.comment,
      settingtype: this.sessionView.setting,
      protectiveEquipmentList: this.ProtectiveEquipmentSelection
    };

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

  handleSelectDragDrop($event: CdkDragDrop<any>, selectionRequired: boolean){
    var droppedSelection = $event.item.data;
    let select = this.ProtectiveEquipmentSelection.find(b => b.equipmentType.id == droppedSelection.equipmentType.id);
    select.isRequired = selectionRequired;
    select.equipmentType.isRequired = selectionRequired;
    this.protectiveEquipmentSessionService.updateSessionEquipmentTypes(this.sessionView.sessionId, this.ProtectiveEquipmentSelection.map(b => b.equipmentType))
    this.cardLockedInPlace = false;
  }

  private updateProtectiveEquipmentSelection() {
    this.sessionView.setting.equipmentTypes
    for(let i = 0; i < this.ProtectiveEquipmentSelection.length; i++){
      var select = this.ProtectiveEquipmentSelection[i];
      var equipmentType = this.sessionView.setting.equipmentTypes.find(u => u.code == select.equipmentType.code);
      select.isRequired = equipmentType.isRequired;
      select.equipmentType = equipmentType;
    }
  }
}

