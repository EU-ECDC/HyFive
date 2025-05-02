import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faCheck, faCircle, faEraser  } from '@fortawesome/free-solid-svg-icons';
import { faHandPaper } from "@fortawesome/free-regular-svg-icons";
import { Role } from "src/app/models/api/Role";
import { Kort } from "src/app/models/registrering/kort.model";
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GloveSession } from "../../models/api/GloveSession";
import { HanskeSesjonsvisning } from "../../models/registrering/hansker-sesjonsvisning.model";
import { HanskeObservasjon } from "../../models/api/HanskeObservasjon";
import { HanskeMedIndikasjonTypeService } from "../../services/data/hanske-med-indikasjon-type.service";
import { HanskeMedIndikasjonType } from '../../models/api/HanskeMedIndikasjonType';
import { HanskeUtenIndikasjonTypeService } from "../../services/data/hanske-uten-indikasjon-type.service";
import { HanskeUtenIndikasjonType } from "../../models/api/HanskeUtenIndikasjonType";
import { HandhygieneEtterHanskebrukTypeService } from "../../services/data/handhygiene-etter-hanskebruk-type.service";
import { HandHygieneAfterGloveUseType } from "../../models/api/HandHygieneAfterGloveUseType";
import { Dialogtekster } from '../../konstanter/dialogtekster';

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
  dialogtekster = Dialogtekster;

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
  hanskeMedIndikasjonTyper: HanskeMedIndikasjonType[] = [];
  hanskeUtenIndikasjonTyper: HanskeUtenIndikasjonType[] = [];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];
  activeTab = "med";
  hanskeBenyttet = null;
  valgtHandhygieneEtterHanskebruk = null;
  
  uuid: string;

  @Input("kort") kort: Kort;
  @Input("rollevalg") rollevalg: Role[];
  @Input("sesjonsvisning") sesjonsvisning: HanskeSesjonsvisning;

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Kort>();

  constructor(
    private hanskeMedIndikasjonTypeService: HanskeMedIndikasjonTypeService,
    private hanskeUtenIndikasjonTypeService: HanskeUtenIndikasjonTypeService,
    private handhygieneEtterHanskebrukTypeService: HandhygieneEtterHanskebrukTypeService,
    protected modalService: NgbModal
  ) {
    super(modalService);
  }

  ngOnInit(): void {
    this.hanskeMedIndikasjonTypeService.getHanskeMedIndikasjonTyper().subscribe((hanskeMedIndikasjonTyper) => {
      this.hanskeMedIndikasjonTyper = hanskeMedIndikasjonTyper;
    });
    this.hanskeUtenIndikasjonTypeService.getHanskeUtenIndikasjonTyper().subscribe((hanskeUtenIndikasjonTyper) => {
      this.hanskeUtenIndikasjonTyper = hanskeUtenIndikasjonTyper;
    });
    this.handhygieneEtterHanskebrukTypeService.getHandhygieneEtterHanskebrukTyper().subscribe((handHygieneAfterGloveUseTypes) => {
      this.handHygieneAfterGloveUseTypes = handHygieneAfterGloveUseTypes;
    });
    this.uuid = Uuid.generateUUID();
  }

  slettKort() {
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  nullstillFane() {
    this.hanskeMedIndikasjonTyper.forEach(x => x.erValgt = false);
    this.hanskeUtenIndikasjonTyper.forEach(x => x.erValgt = false);
    this.hanskeBenyttet = this.activeTab === "med" ? null : true;
    this.valgtHandhygieneEtterHanskebruk = null;
  }

  hanskeMedIndikasjonerChanged(code, event) {
    this.hanskeMedIndikasjonTyper.forEach(x => {
      if (x.code === code) x.erValgt = event.target.checked;
    });
  }

  hanskeUtenIndikasjonerChanged(code, event) {
    this.hanskeUtenIndikasjonTyper.forEach(x => {
      if (x.code === code) x.erValgt = event.target.checked;
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
      if (this.hanskeMedIndikasjonTyper.filter(h => h.erValgt).length === 0) {
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
      if (this.hanskeUtenIndikasjonTyper.filter(h => h.erValgt).length === 0) {
        this.observasjonMangelTekst = "Indikasjon(er) mangler";
        this.visInfoModal = true;
        return true;
      }
    }
  }

  velgRolle(role: Role) {
    this.kort.role = role;
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort[kortIndex] = this.kort;
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  registrerObservasjon() {
    let observasjon = {
      id: Uuid.generateUUID(),
      sessionId: this.sesjonsvisning.sessionId,
      registrationTime: new Date(Date.now()),
      role: this.kort.role,
      comment: this.comment,
      hanskeMedIndikasjonTyper: this.hanskeMedIndikasjonTyper.filter(x => x.erValgt),
      hanskeUtenIndikasjonTyper: this.hanskeUtenIndikasjonTyper.filter(x => x.erValgt),
      gloveUsed: this.hanskeBenyttet,
      handHygieneAfterGloveUseType: this.hanskeBenyttet ? this.handHygieneAfterGloveUseTypes.find(x => x.code === this.valgtHandhygieneEtterHanskebruk) : null,
    } as HanskeObservasjon;

    this.observasjonRegistrert.emit(observasjon);

    this.nullstillKort();

    this.kort.erAktivt = false;
  }

  kortErValgt() {
    this.kort.erAktivt = true;
    this.kortErValgtEvent.emit(this.kort);
  }

  lukkInfoModal(erVisInfoModal): void {
    this.visInfoModal = erVisInfoModal;
  }
}

