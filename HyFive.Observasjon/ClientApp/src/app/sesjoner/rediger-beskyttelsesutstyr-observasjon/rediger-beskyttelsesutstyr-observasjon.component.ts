import { Component, Input, OnInit, EventEmitter, Output, TemplateRef, ViewChild, OnDestroy } from '@angular/core';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from 'src/app/constants/dialogueTexts';
import { ProtectiveEquipmentMapper } from 'src/app/utils/protectiveEquipment-mapper';
import { Colors } from '../../utils/colors';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import { Department } from '../../models/api/Department';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { ProtectiveEquipmentSessionService } from '../../services/data/protectiveEquipment-session.service';
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
  dialogueTexts = DialogueTexts;
  Colors = Colors;
  ikonTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  protectiveEquipment: ProtectiveEquipment[] = [];
  comment: string;
  valgtUtstyr = null;
  beskyttelsesutstyrsesjontype: SessionType = SessionType.ProtectiveEquipment;

  kanIkkeLagreMelding = DialogueTexts.CanNotSaveProtectiveEquipmentObservation;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private sesjonService: ProtectiveEquipmentSessionService,
    private modalService: NgbModal,
    private toastrService: ToastrService) { }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observasjon") observasjon: ProtectiveEquipmentObservation;
  @Input("department") department: Department;
  @Input("institutionid") institutionid: number;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent = new EventEmitter();


  ngOnInit(): void {
    this.protectiveEquipment = this.observasjon.protectiveEquipmentList;
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
    return this.protectiveEquipment.filter(b => b.isRequired);
  }

  beskyttelsesutstyrIkkeIndikert(): ProtectiveEquipment[] {
    return this.protectiveEquipment.filter(b => b.isRequired === false);
  }

  changed(event, valg: ProtectiveEquipment) {
    event.srcElement.blur();
    event.preventDefault();

    valg.wasUsed = true;
    valg.equipmentType.misuseTypes.filter(fb => fb.isSelected == true).map(fb => fb.isSelected = false);

    valg.misuseTypes.forEach(f => {
      const index = valg.equipmentType.misuseTypes.findIndex(fb => fb.id == f.id);
      valg.equipmentType.misuseTypes[index].isSelected = true;
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
      if (valgtUtstyr.wasUsedCorrectly || valgtUtstyr.misuseTypes.length > 0 || valgtUtstyr.comment !== '') {
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
        valgtUtstyr.misuseTypes = result.equipmentType.misuseTypes.filter(x => x.isSelected);
        valgtUtstyr.wasUsed = result.wasUsedCorrectly || valgtUtstyr.misuseTypes.length > 0 || valgtUtstyr.comment !== '';
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
    let valgIndex = this.protectiveEquipment.findIndex(x => x.equipmentType.id === valg.equipmentType.id);
    this.protectiveEquipment[valgIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.observasjon.settingtype.equipmentTypes).find(x => x.equipmentType.id === valg.equipmentType.id);
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
    return this.sesjonService.numberOfQualifiedEquipment(this.observasjon.protectiveEquipmentList) > 0;
  }
}
