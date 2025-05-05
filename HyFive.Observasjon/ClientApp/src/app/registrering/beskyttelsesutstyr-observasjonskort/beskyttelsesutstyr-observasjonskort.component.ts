import { Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Role } from "src/app/models/api/Role";
import { Card } from "src/app/models/registration/card.model";
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { ProtectiveEquipmentSessionView } from "../../models/registration/protectiveEquipment-sessionView.model";
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import { ProtectiveEquipmentCard } from '../../models/registration/protectiveEquipment-card.model';
import { ProtectiveEquipmentMapper } from '../../utils/protectiveEquipment-mapper';
import { ProtectiveEquipmentSession } from '../../models/api/ProtectiveEquipmentSession';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from "../beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component";
import { DialogueTexts } from '../../constants/dialogueTexts';
import { SessionType } from '../../models/api/SessionType';
import { CdkDragDrop } from "@angular/cdk/drag-drop";
import {ProtectiveEquipmentSessionService} from "../../services/data/protectiveEquipment-session.service";

@Component({
  selector: 'app-beskyttelsesutstyr-observasjonskort',
  templateUrl: './beskyttelsesutstyr-observasjonskort.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class BeskyttelsesutstyrObservasjonskortComponent extends BaseKortSwipe implements OnInit {

  comment: string;
  observasjonMangelTekst: string;
  visInfoModal: boolean = false;
  dialogueTexts = DialogueTexts;
  sessionsdata: ProtectiveEquipmentSession = null;
  roles: Role[];
  beskyttelsesutstyrValg: ProtectiveEquipment[] = [];
  beskyttelsesutstyrsesjontype: number = SessionType.ProtectiveEquipment;
  institutionid: number;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;
  faTimes = faTimes;
  farger = Farger;
  ikonTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();

  @Input("card") card: ProtectiveEquipmentCard;
  @Input("roleSelected") roleSelected: Role[];
  @Input("sessionView") sessionView: ProtectiveEquipmentSessionView;

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Card>();

  constructor(modalService: NgbModal,
              private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService) {
    super(modalService);
  }

  ngOnInit(): void {
    this.protectiveEquipmentSessionService.protectiveEquipmentUpdated.subscribe((bu) => {
      this.sessionView.setting.equipmentTypes = bu;

      this.oppdaterBeskyttelsesutstyrValg();
    })
    this.beskyttelsesutstyrValg = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.sessionView.setting.equipmentTypes);
    this.institutionid = this.sessionView.department.institutionId;
  }



  slettKort() {
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  beskyttelsesutstyrIndikert(): ProtectiveEquipment[] {
    return this.beskyttelsesutstyrValg.filter(b => b.isRequired);
  }

  beskyttelsesutstyrIkkeIndikert(): ProtectiveEquipment[] {
    return this.beskyttelsesutstyrValg.filter(b => b.isRequired === false);
  }

  registrerKommentar(comment: string) {
    this.comment = comment;
  }

  nullstillKort() {
    this.comment = "";
    this.beskyttelsesutstyrValg = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.sessionView.setting.equipmentTypes);
  }

  nullstillUtstyr(valg: ProtectiveEquipment) {
    let valgIndex = this.beskyttelsesutstyrValg.findIndex(x => x.equipmentType.id === valg.equipmentType.id);
    this.beskyttelsesutstyrValg[valgIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.sessionView.setting.equipmentTypes).find(x => x.equipmentType.id === valg.equipmentType.id);
  }


  kanIkkeLagre(): boolean {
    let numberOfQualifiedEquipment = this.protectiveEquipmentSessionService.numberOfQualifiedEquipment(this.beskyttelsesutstyrValg);

    if (numberOfQualifiedEquipment < 1) {
      this.observasjonMangelTekst = DialogueTexts.CanNotSaveProtectiveEquipmentObservation;
      this.visInfoModal = true;
    }
    return numberOfQualifiedEquipment < 1;
  }

  changed(event, valg: ProtectiveEquipment) {
    this.cardLockedInPlace = false;
    event.srcElement.blur();
    event.preventDefault();

    if (valg.wasUsed)
      this.visModal(valg);
    else
      this.nullstillUtstyr(valg);
  }

  visModal(valgtUtstyr: ProtectiveEquipment) {
    this.cardLockedInPlace = false;
    const modalRef = this.modalService.open(BeskyttelsesutstyrModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: BeskyttelsesutstyrModalComponentConfig.windowClass
    });

    modalRef.componentInstance.valgtUtstyr = JSON.parse(JSON.stringify(valgtUtstyr)) as ProtectiveEquipment;
    modalRef.componentInstance.valgtUtstyr.wasUsedCorrectly = null;

    modalRef.result.then((result: ProtectiveEquipment) => {
      valgtUtstyr.wasUsedCorrectly = result.wasUsedCorrectly;
      valgtUtstyr.comment = result.comment;
      valgtUtstyr.isRequired = result.isRequired;
      valgtUtstyr.equipmentType.isRequired = result.isRequired;
      valgtUtstyr.misuseTypes = result.equipmentType.misuseTypes.filter(x => x.isSelected);
      valgtUtstyr.wasUsed = result.wasUsedCorrectly || valgtUtstyr.misuseTypes.length > 0 || valgtUtstyr.comment !== '';
    }, (reason) => {
      valgtUtstyr.wasUsed = false;
    });
  }

  setAlleUtstyrTilRiktigBrukt(event) {
    this.beskyttelsesutstyrIndikert().forEach(x => {
      x.wasUsed = true;
      x.wasUsedCorrectly = true;
    });
  }

  velgRolle(role: Role) {
    this.card.role = role;
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card[kortIndex] = this.card;
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  registrerObservasjon() {
    let observasjon: ProtectiveEquipmentObservation = {
      id: Uuid.generateUUID(),
      role: this.card.role,
      registrationTime: new Date(Date.now()),
      sessionId: this.sessionView.sessionId,
      comment: this.comment,
      settingtype: this.sessionView.setting,
      protectiveEquipmentList: this.beskyttelsesutstyrValg
    };

    this.observasjonRegistrert.emit(observasjon);

    this.nullstillKort();

    this.card.isActive = false;
  }

  kortErValgt() {
    this.card.isActive = true;
    this.kortErValgtEvent.emit(this.card);
  }

  lukkInfoModal(erVisInfoModal): void {
    this.visInfoModal = erVisInfoModal;
  }

  handleValgDragDrop($event: CdkDragDrop<any>, valgIndikert: boolean){
    var droppedValg = $event.item.data;
    let valg = this.beskyttelsesutstyrValg.find(b => b.equipmentType.id == droppedValg.equipmentType.id);
    valg.isRequired = valgIndikert;
    valg.equipmentType.isRequired = valgIndikert;
    this.protectiveEquipmentSessionService.updateSessionEquipmentTypes(this.sessionView.sessionId, this.beskyttelsesutstyrValg.map(b => b.equipmentType))
    this.cardLockedInPlace = false;
  }

  private oppdaterBeskyttelsesutstyrValg() {
    this.sessionView.setting.equipmentTypes
    for(let i = 0; i < this.beskyttelsesutstyrValg.length; i++){
      var valg = this.beskyttelsesutstyrValg[i];
      var equipmentType = this.sessionView.setting.equipmentTypes.find(u => u.code == valg.equipmentType.code);
      valg.isRequired = equipmentType.isRequired;
      valg.equipmentType = equipmentType;
    }
  }
}

