import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faCheck, faCircle, faEraser  } from '@fortawesome/free-solid-svg-icons';
import { faHandPaper } from "@fortawesome/free-regular-svg-icons";
import { Role } from "src/app/models/api/Role";
import { Card } from "src/app/models/registration/card.model";
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GloveSession } from "../../models/api/GloveSession";
import { GloveSessionView } from "../../models/registration/glove-session-view.model";
import { GloveObservation } from "../../models/api/GloveObservation";
import { HanskeMedIndikasjonTypeService } from "../../services/data/hanske-med-indikasjon-type.service";
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';
import { HanskeUtenIndikasjonTypeService } from "../../services/data/hanske-uten-indikasjon-type.service";
import { GloveWithoutIndicationType } from "../../models/api/GloveWithoutIndicationType";
import { HandhygieneEtterHanskebrukTypeService } from "../../services/data/handhygiene-etter-hanskebruk-type.service";
import { HandHygieneAfterGloveUseType } from "../../models/api/HandHygieneAfterGloveUseType";
import { DialogueTexts } from '../../constants/dialogueTexts';

@Component({
  selector: 'app-hanske-observasjonskort',
  templateUrl: './hanske-observasjonskort.component.html',
  animations: [
    Animations.swipeLeftRight
  ]
})
export class HanskeObservasjonskortComponent extends BaseKortSwipe implements OnInit {

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
  faHandPaper = faHandPaper;
  farger = Farger;

  sessionsdata: GloveSession = null;
  roles: Role[];
  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[] = [];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];
  activeTab = "med";
  hanskeBenyttet = null;
  valgtHandhygieneEtterHanskebruk = null;
  
  uuid: string;

  @Input("card") card: Card;
  @Input("roleSelected") roleSelected: Role[];
  @Input("sessionView") sessionView: GloveSessionView;

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Card>();

  constructor(
    private hanskeMedIndikasjonTypeService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonTypeService: HanskeUtenIndikasjonTypeService,
    private handhygieneEtterHanskebrukTypeService: HandhygieneEtterHanskebrukTypeService,
    protected modalService: NgbModal
  ) {
    super(modalService);
  }

  ngOnInit(): void {
    this.hanskeMedIndikasjonTypeService.getHanskeMedIndikasjonTyper().subscribe((gloveWithIndicationTypes) => {
      this.gloveWithIndicationTypes = gloveWithIndicationTypes;
    });
    this.hanskeUtenIndikasjonTypeService.getHanskeUtenIndikasjonTyper().subscribe((gloveWithoutIndicationTypes) => {
      this.gloveWithoutIndicationTypes = gloveWithoutIndicationTypes;
    });
    this.handhygieneEtterHanskebrukTypeService.getHandhygieneEtterHanskebrukTyper().subscribe((handHygieneAfterGloveUseTypes) => {
      this.handHygieneAfterGloveUseTypes = handHygieneAfterGloveUseTypes;
    });
    this.uuid = Uuid.generateUUID();
  }

  slettKort() {
    let kortIndex = this.sessionView.card.findIndex(x => x.id === this.card.id);
    this.sessionView.card.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sessionView);
  }

  nullstillFane() {
    this.gloveWithIndicationTypes.forEach(x => x.isSelected = false);
    this.gloveWithoutIndicationTypes.forEach(x => x.isSelected = false);
    this.hanskeBenyttet = this.activeTab === "med" ? null : true;
    this.valgtHandhygieneEtterHanskebruk = null;
  }

  hanskeMedIndikasjonerChanged(code, event) {
    this.gloveWithIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  hanskeUtenIndikasjonerChanged(code, event) {
    this.gloveWithoutIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  registrerKommentar(comment: string) {
    this.comment = comment;
  }

  nullstillKort() {
    this.nullstillFane();
    this.comment = "";
  }

  kanIkkeLagre(): boolean {

    if (this.activeTab === "med") {
      if (this.gloveWithIndicationTypes.filter(h => h.isSelected).length === 0) {
        this.observasjonMangelTekst = "Indikasjon(er) mangler";
        this.visInfoModal = true;
        return true;
      }
      else if (this.hanskeBenyttet === null) {
        this.observasjonMangelTekst = "\"Hanske brukt?\" må besvares";
        this.visInfoModal = true;
        return true;
      }
    }
    else {
      if (this.gloveWithoutIndicationTypes.filter(h => h.isSelected).length === 0) {
        this.observasjonMangelTekst = "Indikasjon(er) mangler";
        this.visInfoModal = true;
        return true;
      }
    }
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
      comment: this.comment,
      gloveWithIndicationTypes: this.gloveWithIndicationTypes.filter(x => x.isSelected),
      gloveWithoutIndicationTypes: this.gloveWithoutIndicationTypes.filter(x => x.isSelected),
      gloveUsed: this.hanskeBenyttet,
      handHygieneAfterGloveUseType: this.hanskeBenyttet ? this.handHygieneAfterGloveUseTypes.find(x => x.code === this.valgtHandhygieneEtterHanskebruk) : null,
    } as GloveObservation;

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

