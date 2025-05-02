import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { HandsmykkeSesjonsvisning } from "src/app/models/registrering/handsmykke-sesjonsvisning.model";
import { HandsmykkeSesjon } from "src/app/models/api/HandsmykkeSesjon";
import { Role } from "src/app/models/api/Role";
import { HandJewelryObservation } from 'src/app/models/api/HandJewelryObservation';
import { Kort } from "src/app/models/registrering/kort.model";
import { HandJewelryType } from '../../models/api/HandJewelryType';
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { Handsmykkevalg } from "../../models/registrering/handsmykkevalg.model";
import { HandsmykkeMapper } from "../../utils/handsmykke-mapper";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { HandsmykkeTypeKonstanter } from "../../models/api/HandsmykkeTypeKonstanter";
import { HandsmykkeTypeService } from "../../services/data/handsmykketype.service";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Dialogtekster } from '../../konstanter/dialogtekster';

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
  dialogtekster = Dialogtekster;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;
  faTimes = faTimes;
  farger = Farger;
  ikonTypeMap: Map<HandsmykkeTypeKonstanter, IconProp> = HandsmykkeMapper.getIkontypeMap();

  sessionsdata: HandsmykkeSesjon = null;
  roles: Role[];
  handJewelryTypes: HandJewelryType[] = [];

  handsmykkevalg = [] as Handsmykkevalg[];

  @Input("kort") kort: Kort;
  @Input("rollevalg") rollevalg: Role[];
  @Input("sesjonsvisning") sesjonsvisning: HandsmykkeSesjonsvisning;

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Kort>();

  constructor(
    private handsmykkeTypeService: HandsmykkeTypeService,
    protected modalService: NgbModal) {
    super(modalService);
  }

  ngOnInit(): void {
    this.handsmykkeTypeService.getHandsmykkeTyper().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
      this.handsmykkevalg = HandsmykkeMapper.getHandsmykkevalg(this.handJewelryTypes, []);
    });
  }

  slettKort() {
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  registrerKommentar(comment: string) {
    this.comment = comment;
  }

  nullstillKort() {
    this.comment = "";
    this.handsmykkevalg = HandsmykkeMapper.getHandsmykkevalg(this.handJewelryTypes, []);
  }

  antallValgteHandsmykker() {
    return this.handsmykkevalg.reduce((acc, curr) => { if (curr.erValgt) return acc + 1; return acc; }, 0);
  }

  kanIkkeLagre(): boolean {
    let antallValgteHandsmykker = this.antallValgteHandsmykker();

    if (antallValgteHandsmykker < 1) {
      this.observasjonMangelTekst = "Handsmykker mangler";
      this.visInfoModal = true;
    }
    return antallValgteHandsmykker < 1;
  }

  changed(valg: Handsmykkevalg) {
    if (valg.erValgt && valg.type == HandsmykkeTypeKonstanter.AltOk)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { if (x.type !== HandsmykkeTypeKonstanter.AltOk) x.disabled = true; return x; }) // disable all
    else if (valg.erValgt && valg.type != HandsmykkeTypeKonstanter.AltOk)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { if (x.type === HandsmykkeTypeKonstanter.AltOk) x.disabled = true; return x; }) // disable altok
    else if (this.antallValgteHandsmykker() < 1)
      this.handsmykkevalg = this.handsmykkevalg.map(x => { x.disabled = false; return x; }) // enable all
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
      handJewelry: this.handsmykkevalg.reduce((acc, item) => {
        if (item.erValgt) acc.push(this.handJewelryTypes.find(x => x.code === item.type));
        return acc;
      }, [] as HandJewelryType[]) as HandJewelryType[],
      comment: this.comment
    } as HandJewelryObservation;

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

