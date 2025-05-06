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
import { ProtectiveEquipmentModalComponent, ProtectiveEquipmentModalComponentConfig } from '../../registrering/protective-equipment-modal/protective-equipment-modal.component';
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
  iconTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  protectiveEquipment: ProtectiveEquipment[] = [];
  comment: string;
  selectedEquipment = null;
  beskyttelsesutstyrsesjontype: SessionType = SessionType.ProtectiveEquipment;

  kanIkkeLagreMelding = DialogueTexts.CanNotSaveProtectiveEquipmentObservation;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private sessionService: ProtectiveEquipmentSessionService,
    private modalService: NgbModal,
    private toastrService: ToastrService) { }

  @Input("isReadonly") isReadonly: boolean = false;
  @Input("observation") observation: ProtectiveEquipmentObservation;
  @Input("department") department: Department;
  @Input("institutionid") institutionid: number;
  @Output("observasjonSlettetEvent") observasjonSlettetEvent = new EventEmitter();


  ngOnInit(): void {
    this.protectiveEquipment = this.observation.protectiveEquipmentList;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  registerComment(comment: string) {
    this.observation.comment = comment;
  }

  lagreObservasjon() {
    if(!this.canSave()){
      this.toastrService.error(this.kanIkkeLagreMelding, '', {disableTimeOut: true})
      return;
    }
    if (this.erRedigeringsmodus) {
      this.sessionService.changeObservation(this.observation);
    }
    this.erRedigeringsmodus = false;
  }

  deleteObservation() {
    this.sessionService.deleteObservation(this.observation);
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

  visModal(selectedEquipment: ProtectiveEquipment) {

    const modalRef = this.modalService.open(ProtectiveEquipmentModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: ProtectiveEquipmentModalComponentConfig.windowClass
    });

    modalRef.componentInstance.selectedEquipment = JSON.parse(JSON.stringify(selectedEquipment)) as ProtectiveEquipment;

    modalRef.componentInstance.displayMode = !this.erRedigeringsmodus;

    if (this.erRedigeringsmodus && selectedEquipment.wasUsed) {
      if (selectedEquipment.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '') {
        modalRef.componentInstance.showEquipmentDeleteButton = true;
      }
      else {
        modalRef.componentInstance.selectedEquipment.wasUsedCorrectly = null;
      }
    }

    modalRef.result.then((result: ProtectiveEquipment) => {
      if (!this.erRedigeringsmodus) {
        return;
      }
      selectedEquipment.isRequired = result.isRequired;
      selectedEquipment.equipmentType.isRequired = result.isRequired;
      if (result.wasUsed === false) {
        this.nullstillUtstyr(selectedEquipment);
      }
      else {
        selectedEquipment.wasUsedCorrectly = result.wasUsedCorrectly;
        selectedEquipment.comment = result.comment;
        selectedEquipment.misuseTypes = result.equipmentType.misuseTypes.filter(x => x.isSelected);
        selectedEquipment.wasUsed = result.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '';
      }
    }, (reason) => {
      if (this.erRedigeringsmodus) {
        selectedEquipment.wasUsed = false;
        this.nullstillUtstyr(selectedEquipment);
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
    this.protectiveEquipment[valgIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.observation.settingtype.equipmentTypes).find(x => x.equipmentType.id === valg.equipmentType.id);
  }

  visVisningsmodusModal(event, valg: ProtectiveEquipment) {
    if (!this.erRedigeringsmodus && valg.wasUsed) {
      event.stopPropagation();
      event.preventDefault();
      this.visModal(valg);
      return;
    }
  }

  canSave() {
    return this.sessionService.numberOfQualifiedEquipment(this.observation.protectiveEquipmentList) > 0;
  }
}
