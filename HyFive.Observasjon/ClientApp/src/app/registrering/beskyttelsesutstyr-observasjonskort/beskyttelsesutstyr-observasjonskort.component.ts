import { Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Uuid } from "src/app/utils/uuid";
import { faSave, faTrashAlt, faTimes, faEraser, faCheck, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Rolle } from "src/app/models/api/Rolle";
import { Kort } from "src/app/models/registrering/kort.model";
import { Animations } from "../../shared/animasjoner/animasjoner";
import { BaseKortSwipe } from "../../shared/kort-swipe/kort-swipe";
import { Farger } from "../../utils/farger";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { BeskyttelsesutstyrSesjonsvisning } from "../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model";
import { BeskyttelsesutstyrObservasjon } from '../../models/api/BeskyttelsesutstyrObservasjon';
import { BeskyttelsesutstyrKort } from '../../models/registrering/beskyttelsesutstyr-kort.model';
import { BeskyttelsesutstyrMapper } from '../../utils/beskyttelsesutstyrmapper';
import { BeskyttelsesutstyrSesjon } from '../../models/api/BeskyttelsesutstyrSesjon';
import { Beskyttelsesutstyr } from '../../models/api/Beskyttelsesutstyr';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from "../beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component";
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { SesjonType } from '../../models/api/SesjonType';
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

  kommentar: string;
  observasjonMangelTekst: string;
  visInfoModal: boolean = false;
  dialogtekster = Dialogtekster;
  sesjonsdata: BeskyttelsesutstyrSesjon = null;
  roller: Rolle[];
  beskyttelsesutstyrValg: Beskyttelsesutstyr[] = [];
  beskyttelsesutstyrsesjontype: number = SesjonType.Beskyttelsesutstyr;
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
  @Input("rollevalg") rollevalg: Rolle[];
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
      this.sesjonsvisning.setting.utstyrstyper = bu;

      this.oppdaterBeskyttelsesutstyrValg();
    })
    this.beskyttelsesutstyrValg = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.sesjonsvisning.setting.utstyrstyper);
    this.institusjonid = this.sesjonsvisning.avdeling.institusjonId;
  }



  slettKort() {
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort.splice(kortIndex, 1);
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  beskyttelsesutstyrIndikert(): Beskyttelsesutstyr[] {
    return this.beskyttelsesutstyrValg.filter(b => b.erIndikert);
  }

  beskyttelsesutstyrIkkeIndikert(): Beskyttelsesutstyr[] {
    return this.beskyttelsesutstyrValg.filter(b => b.erIndikert === false);
  }

  registrerKommentar(kommentar: string) {
    this.kommentar = kommentar;
  }

  nullstillKort() {
    this.kommentar = "";
    this.beskyttelsesutstyrValg = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.sesjonsvisning.setting.utstyrstyper);
  }

  nullstillUtstyr(valg: Beskyttelsesutstyr) {
    let valgIndex = this.beskyttelsesutstyrValg.findIndex(x => x.utstyrstype.id === valg.utstyrstype.id);
    this.beskyttelsesutstyrValg[valgIndex] = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.sesjonsvisning.setting.utstyrstyper).find(x => x.utstyrstype.id === valg.utstyrstype.id);
  }


  kanIkkeLagre(): boolean {
    let antallKvalifisertUtstyr = this.beskyttelsesutstyrSesjonService.antallKvalifisertUtstyr(this.beskyttelsesutstyrValg);

    if (antallKvalifisertUtstyr < 1) {
      this.observasjonMangelTekst = Dialogtekster.KanIkkeLagreBeskyttelsesutstyrObservasjon;
      this.visInfoModal = true;
    }
    return antallKvalifisertUtstyr < 1;
  }

  changed(event, valg: Beskyttelsesutstyr) {
    this.cardLockedInPlace = false;
    event.srcElement.blur();
    event.preventDefault();

    if (valg.bleBenyttet)
      this.visModal(valg);
    else
      this.nullstillUtstyr(valg);
  }

  visModal(valgtUtstyr: Beskyttelsesutstyr) {
    this.cardLockedInPlace = false;
    const modalRef = this.modalService.open(BeskyttelsesutstyrModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: BeskyttelsesutstyrModalComponentConfig.windowClass
    });

    modalRef.componentInstance.valgtUtstyr = JSON.parse(JSON.stringify(valgtUtstyr)) as Beskyttelsesutstyr;
    modalRef.componentInstance.valgtUtstyr.bleBenyttetRiktig = null;

    modalRef.result.then((result: Beskyttelsesutstyr) => {
      valgtUtstyr.bleBenyttetRiktig = result.bleBenyttetRiktig;
      valgtUtstyr.kommentar = result.kommentar;
      valgtUtstyr.erIndikert = result.erIndikert;
      valgtUtstyr.utstyrstype.erIndikert = result.erIndikert;
      valgtUtstyr.feilbruktyper = result.utstyrstype.feilbruktyper.filter(x => x.erValgt);
      valgtUtstyr.bleBenyttet = result.bleBenyttetRiktig || valgtUtstyr.feilbruktyper.length > 0 || valgtUtstyr.kommentar !== '';
    }, (reason) => {
      valgtUtstyr.bleBenyttet = false;
    });
  }

  setAlleUtstyrTilRiktigBrukt(event) {
    this.beskyttelsesutstyrIndikert().forEach(x => {
      x.bleBenyttet = true;
      x.bleBenyttetRiktig = true;
    });
  }

  velgRolle(rolle: Rolle) {
    this.kort.rolle = rolle;
    let kortIndex = this.sesjonsvisning.kort.findIndex(x => x.id === this.kort.id);
    this.sesjonsvisning.kort[kortIndex] = this.kort;
    this.sesjonsvisningOppdatert.emit(this.sesjonsvisning);
  }

  registrerObservasjon() {
    let observasjon: BeskyttelsesutstyrObservasjon = {
      id: Uuid.generateUUID(),
      rolle: this.kort.rolle,
      registrerttidspunkt: new Date(Date.now()),
      sesjonId: this.sesjonsvisning.sesjonId,
      kommentar: this.kommentar,
      settingtype: this.sesjonsvisning.setting,
      beskyttelsesutstyrliste: this.beskyttelsesutstyrValg
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
    let valg = this.beskyttelsesutstyrValg.find(b => b.utstyrstype.id == droppedValg.utstyrstype.id);
    valg.erIndikert = valgIndikert;
    valg.utstyrstype.erIndikert = valgIndikert;
    this.beskyttelsesutstyrSesjonService.oppdaterSesjonUtstyrstyper(this.sesjonsvisning.sesjonId, this.beskyttelsesutstyrValg.map(b => b.utstyrstype))
    this.cardLockedInPlace = false;
  }

  private oppdaterBeskyttelsesutstyrValg() {
    this.sesjonsvisning.setting.utstyrstyper
    for(let i = 0; i < this.beskyttelsesutstyrValg.length; i++){
      var valg = this.beskyttelsesutstyrValg[i];
      var utstyrstype = this.sesjonsvisning.setting.utstyrstyper.find(u => u.kode == valg.utstyrstype.kode);
      valg.erIndikert = utstyrstype.erIndikert;
      valg.utstyrstype = utstyrstype;
    }
  }
}

