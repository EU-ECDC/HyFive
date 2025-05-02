import { Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Role } from "src/app/models/api/Role";
import { Kort } from "src/app/models/registrering/kort.model";
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { BeskyttelsesutstyrSesjonsvisning } from "../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model";
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import { BeskyttelsesutstyrKort } from '../../models/registrering/beskyttelsesutstyr-kort.model';
import { BeskyttelsesutstyrMapper } from '../../utils/beskyttelsesutstyrmapper';
import { ProtectiveEquipmentSession } from '../../models/api/ProtectiveEquipmentSession';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from "../beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component";
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { SessionType } from '../../models/api/SessionType';
import { CdkDragDrop } from "@angular/cdk/drag-drop";
import {BeskyttelsesutstyrSesjonService} from "../../services/data/beskyttelsesutstyr-sesjon.service";

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
  dialogtekster = Dialogtekster;
  sessionsdata: ProtectiveEquipmentSession = null;
  roles: Role[];
  beskyttelsesutstyrValg: ProtectiveEquipment[] = [];
  beskyttelsesutstyrsesjontype: number = SessionType.ProtectiveEquipment;
  institusjonid: number;

  faEraser = faEraser;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;
  faTimes = faTimes;
  farger = Farger;
  ikonTypeMap: Map<string, IconProp> = BeskyttelsesutstyrMapper.getIkontypeMap();

  @Input("kort") kort: BeskyttelsesutstyrKort;
  @Input("rollevalg") rollevalg: Role[];
  @Input("sesjonsvisning") sesjonsvisning: BeskyttelsesutstyrSesjonsvisning;

  @Output() observasjonRegistrert = new EventEmitter();
  @Output() observasjonOppdatert = new EventEmitter();
  @Output() sesjonsvisningOppdatert = new EventEmitter();
  @Output() kortErValgtEvent = new EventEmitter<Kort>();

  constructor(modalService: NgbModal,
              private beskyttelsesutstyrSesjonService: BeskyttelsesutstyrSesjonService) {
    super(modalService);
  }

  ngOnInit(): void {
    this.beskyttelsesutstyrSesjonService.beskyttelsesutstyrOppdatert.subscribe((bu) => {
      this.sesjonsvisning.setting.equipmentTypes = bu;

      this.oppdaterBeskyttelsesutstyrValg();
    })
    this.beskyttelsesutstyrValg = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.sesjonsvisning.setting.equipmentTypes);
    this.institusjonid = this.sesjonsvisning.department.institutionId;
  }



  slettKort() {
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
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
    this.beskyttelsesutstyrValg = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.sesjonsvisning.setting.equipmentTypes);
  }

  nullstillUtstyr(valg: ProtectiveEquipment) {
    let valgIndex = this.beskyttelsesutstyrValg.findIndex(x => x.equipmentType.id === valg.equipmentType.id);
    this.beskyttelsesutstyrValg[valgIndex] = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.sesjonsvisning.setting.equipmentTypes).find(x => x.equipmentType.id === valg.equipmentType.id);
  }


  kanIkkeLagre(): boolean {
    let antallKvalifisertUtstyr = this.beskyttelsesutstyrSesjonService.antallKvalifisertUtstyr(this.beskyttelsesutstyrValg);

    if (antallKvalifisertUtstyr < 1) {
      this.observasjonMangelTekst = Dialogtekster.KanIkkeLagreBeskyttelsesutstyrObservasjon;
      this.visInfoModal = true;
    }
    return antallKvalifisertUtstyr < 1;
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
      valgtUtstyr.incorrectTypes = result.equipmentType.incorrectTypes.filter(x => x.erValgt);
      valgtUtstyr.wasUsed = result.wasUsedCorrectly || valgtUtstyr.incorrectTypes.length > 0 || valgtUtstyr.comment !== '';
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
    this.kort.role = role;
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort[kortIndex] = this.kort;
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  registrerObservasjon() {
    let observasjon: ProtectiveEquipmentObservation = {
      id: Uuid.generateUUID(),
      role: this.kort.role,
      registrationTime: new Date(Date.now()),
      sessionId: this.sesjonsvisning.sessionId,
      comment: this.comment,
      settingtype: this.sesjonsvisning.setting,
      protectiveEquipmentList: this.beskyttelsesutstyrValg
    };

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

  handleValgDragDrop($event: CdkDragDrop<any>, valgIndikert: boolean){
    var droppedValg = $event.item.data;
    let valg = this.beskyttelsesutstyrValg.find(b => b.equipmentType.id == droppedValg.equipmentType.id);
    valg.isRequired = valgIndikert;
    valg.equipmentType.isRequired = valgIndikert;
    this.beskyttelsesutstyrSesjonService.oppdaterSesjonUtstyrstyper(this.sesjonsvisning.sessionId, this.beskyttelsesutstyrValg.map(b => b.equipmentType))
    this.cardLockedInPlace = false;
  }

  private oppdaterBeskyttelsesutstyrValg() {
    this.sesjonsvisning.setting.equipmentTypes
    for(let i = 0; i < this.beskyttelsesutstyrValg.length; i++){
      var valg = this.beskyttelsesutstyrValg[i];
      var equipmentType = this.sesjonsvisning.setting.equipmentTypes.find(u => u.code == valg.equipmentType.code);
      valg.isRequired = equipmentType.isRequired;
      valg.equipmentType = equipmentType;
    }
  }
}

