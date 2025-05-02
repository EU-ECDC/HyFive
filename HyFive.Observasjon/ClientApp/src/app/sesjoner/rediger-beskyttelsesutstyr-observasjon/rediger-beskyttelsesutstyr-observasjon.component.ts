import { Component, Input, OnInit, EventEmitter, Output, TemplateRef, ViewChild, OnDestroy } from '@angular/core';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { Dialogtekster } from 'src/app/konstanter/dialogtekster';
import { BeskyttelsesutstyrMapper } from 'src/app/utils/beskyttelsesutstyrmapper';
import { Farger } from '../../utils/farger';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import { Department } from '../../models/api/Department';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { BeskyttelsesutstyrSesjonService } from '../../services/data/beskyttelsesutstyr-sesjon.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BeskyttelsesutstyrModalComponent, BeskyttelsesutstyrModalComponentConfig } from '../../registrering/beskyttelsesutstyr-modal/beskyttelsesutstyr-modal.component';
import { SessionType } from 'src/app/models/api/SessionType';
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-rediger-beskyttelsesutstyr-observasjon',
  templateUrl: './rediger-beskyttelsesutstyr-observasjon.component.html'
})
export class RedigerBeskyttelsesutstyrObservasjonComponent implements OnInit, OnDestroy {

  erRedigeringsmodus: boolean = false;
  dialogtekster = Dialogtekster;
  Farger = Farger;
  ikonTypeMap: Map<string, IconProp> = BeskyttelsesutstyrMapper.getIkontypeMap();
  beskyttelsesutstyr: ProtectiveEquipment[] = [];
  comment: string;
  valgtUtstyr = null;
  beskyttelsesutstyrsesjontype: SessionType = SessionType.ProtectiveEquipment;

  kanIkkeLagreMelding = Dialogtekster.KanIkkeLagreBeskyttelsesutstyrObservasjon;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private sesjonService: BeskyttelsesutstyrSesjonService,
    private modalService: NgbModal,
    private toastrService: ToastrService) { }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observasjon") observasjon: ProtectiveEquipmentObservation;
  @Input("department") department: Department;
  @Input("institusjonid") institusjonid: number;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent = new EventEmitter();


  ngOnInit(): void {
    this.beskyttelsesutstyr = this.observasjon.protectiveEquipmentList;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  registrerKommentar(comment: string) {
    this.observasjon.comment = comment;
  }

  lagreObservasjon() {
    if(!this.kanLagre()){
      this.toastrService.error(this.kanIkkeLagreMelding, '', {disableTimeOut: true})
      return;
    }
    if (this.erRedigeringsmodus) {
      this.sesjonService.endreObservasjon(this.observasjon);
    }
    this.erRedigeringsmodus = false;
  }

  slettObservasjon() {
    this.sesjonService.slettObservasjon(this.observasjon);
    this.observasjonSlettetEvent.emit();
  }

  beskyttelsesutstyrIndikert(): ProtectiveEquipment[] {
    return this.beskyttelsesutstyr.filter(b => b.isRequired);
  }

  beskyttelsesutstyrIkkeIndikert(): ProtectiveEquipment[] {
    return this.beskyttelsesutstyr.filter(b => b.isRequired === false);
  }

  changed(event, valg: ProtectiveEquipment) {
    event.srcElement.blur();
    event.preventDefault();

    valg.wasUsed = true;
    valg.equipmentType.incorrectTypes.filter(fb => fb.erValgt == true).map(fb => fb.erValgt = false);

    valg.incorrectTypes.forEach(f => {
      const index = valg.equipmentType.incorrectTypes.findIndex(fb => fb.id == f.id);
      valg.equipmentType.incorrectTypes[index].erValgt = true;
    });

    if (valg.wasUsed) {
      this.visModal(valg);
    }
  }

  visModal(valgtUtstyr: ProtectiveEquipment) {

    const modalRef = this.modalService.open(BeskyttelsesutstyrModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: BeskyttelsesutstyrModalComponentConfig.windowClass
    });

    modalRef.componentInstance.valgtUtstyr = JSON.parse(JSON.stringify(valgtUtstyr)) as ProtectiveEquipment;

    modalRef.componentInstance.visningsmodus = !this.erRedigeringsmodus;

    if (this.erRedigeringsmodus && valgtUtstyr.wasUsed) {
      if (valgtUtstyr.wasUsedCorrectly || valgtUtstyr.incorrectTypes.length > 0 || valgtUtstyr.comment !== '') {
        modalRef.componentInstance.visKnappForSlettingAvUtstyr = true;
      }
      else {
        modalRef.componentInstance.valgtUtstyr.wasUsedCorrectly = null;
      }
    }

    modalRef.result.then((result: ProtectiveEquipment) => {
      if (!this.erRedigeringsmodus) {
        return;
      }
      valgtUtstyr.isRequired = result.isRequired;
      valgtUtstyr.equipmentType.isRequired = result.isRequired;
      if (result.wasUsed === false) {
        this.nullstillUtstyr(valgtUtstyr);
      }
      else {
        valgtUtstyr.wasUsedCorrectly = result.wasUsedCorrectly;
        valgtUtstyr.comment = result.comment;
        valgtUtstyr.incorrectTypes = result.equipmentType.incorrectTypes.filter(x => x.erValgt);
        valgtUtstyr.wasUsed = result.wasUsedCorrectly || valgtUtstyr.incorrectTypes.length > 0 || valgtUtstyr.comment !== '';
      }
    }, (reason) => {
      if (this.erRedigeringsmodus) {
        valgtUtstyr.wasUsed = false;
        this.nullstillUtstyr(valgtUtstyr);
      }
    });
  }

  setAlleUtstyrTilRiktigBrukt(event) {
    this.beskyttelsesutstyrIndikert().forEach(x => {
      x.wasUsed = true;
      x.wasUsedCorrectly = true;
    });
  }

  nullstillUtstyr(valg: ProtectiveEquipment) {
    let valgIndex = this.beskyttelsesutstyr.findIndex(x => x.equipmentType.id === valg.equipmentType.id);
    this.beskyttelsesutstyr[valgIndex] = BeskyttelsesutstyrMapper.getBeskyttelsesutstyrvalg(this.observasjon.settingtype.equipmentTypes).find(x => x.equipmentType.id === valg.equipmentType.id);
  }

  visVisningsmodusModal(event, valg: ProtectiveEquipment) {
    if (!this.erRedigeringsmodus && valg.wasUsed) {
      event.stopPropagation();
      event.preventDefault();
      this.visModal(valg);
      return;
    }
  }

  kanLagre() {
    return this.sesjonService.antallKvalifisertUtstyr(this.observasjon.protectiveEquipmentList) > 0;
  }
}
