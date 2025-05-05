import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { HandJewelrySessionView } from "src/app/models/registration/handJewelry-session-view.model";
import { HandJewelrySession } from "src/app/models/api/HandJewelrySession";
import { Role } from "src/app/models/api/Role";
import { HandJewelryObservation } from 'src/app/models/api/HandJewelryObservation';
import { Card } from "src/app/models/registration/card.model";
import { HandJewelryType } from '../../models/api/HandJewelryType';
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { Handsmykkevalg } from "../../models/registration/handJewelry-selection.model";
import { HandsmykkeMapper } from "../../utils/handsmykke-mapper";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { HandJewelryTypeConstants } from "../../models/api/HandJewelryTypeConstants";
import { HandJewelryTypeService } from "../../services/data/hand-jewelry-type.service";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DialogueTexts } from '../../constants/dialogueTexts';

@Component({
  selector: 'app-handsmykker-observasjonskort',
  templateUrl: './handsmykker-observasjonskort.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class HandsmykkerObservasjonskortComponent extends BaseKortSwipe implements OnInit {

  comment: string;
  observasjonMangelTekst: string;
  visInfoModal: boolean = false;
  dialogueTexts = DialogueTexts;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;
  faTimes = faTimes;
  farger = Farger;
  ikonTypeMap: Map<HandJewelryTypeConstants, IconProp> = HandsmykkeMapper.getIkontypeMap();

  sessionsdata: HandJewelrySession = null;
  roles: Role[];
  handJewelryTypes: HandJewelryType[] = [];

  handsmykkevalg = [] as Handsmykkevalg[];

  @Input("card") card: Card;
  @Input("roleSelected") roleSelected: Role[];
  @Input("sessionView") sessionView: HandJewelrySessionView;

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Card>();

  constructor(
    private handJewelryTypeService: HandJewelryTypeService,
    protected modalService: NgbModal) {
    super(modalService);
  }

  ngOnInit(): void {
    this.handJewelryTypeService.getHandJewelryTypes().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
      this.handsmykkevalg = HandsmykkeMapper.getHandsmykkevalg(this.handJewelryTypes, []);
    });
  }

  slettKort() {
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  registrerKommentar(comment: string) {
    this.comment = comment;
  }

  nullstillKort() {
    this.comment = "";
    this.handsmykkevalg = HandsmykkeMapper.getHandsmykkevalg(this.handJewelryTypes, []);
  }

  antallValgteHandsmykker() {
    return this.handsmykkevalg.reduce((acc, curr) => { if (curr.isSelected) return acc + 1; return acc; }, 0);
  }

  kanIkkeLagre(): boolean {
    let antallValgteHandsmykker = this.antallValgteHandsmykker();

    if (antallValgteHandsmykker < 1) {
      this.observasjonMangelTekst = "HandJewelry mangler";
      this.visInfoModal = true;
    }
    return antallValgteHandsmykker < 1;
  }

  changed(valg: Handsmykkevalg) {
    if (valg.isSelected && valg.type == HandJewelryTypeConstants.AllClear)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { if (x.type !== HandJewelryTypeConstants.AllClear) x.disabled = true; return x; }) // disable all
    else if (valg.isSelected && valg.type != HandJewelryTypeConstants.AllClear)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { if (x.type === HandJewelryTypeConstants.AllClear) x.disabled = true; return x; }) // disable altok
    else if (this.antallValgteHandsmykker() < 1)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { x.disabled = false; return x; }) // enable all
  }

  velgRolle(role: Role) {
    this.card.role = role;
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card[kortIndex] = this.card;
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  registrerObservasjon() {
    let observasjon = {
      id: Uuid.generateUUID(),
      sessionId: this.sessionView.sessionId,
      registrationTime: new Date(Date.now()),
      role: this.card.role,
      handJewelry: this.handsmykkevalg.reduce((acc, item) => {
        if (item.isSelected) acc.push(this.handJewelryTypes.find(x => x.code === item.type));
        return acc;
      }, [] as HandJewelryType[]) as HandJewelryType[],
      comment: this.comment
    } as HandJewelryObservation;

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
}

