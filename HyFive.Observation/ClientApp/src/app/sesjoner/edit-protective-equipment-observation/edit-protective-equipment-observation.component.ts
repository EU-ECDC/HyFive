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
  selector: 'app-edit-protective-equipment-observation',
  templateUrl: './edit-protective-equipment-observation.component.html'
})
export class EditProtectiveEquipmentObservationComponent implements OnInit, OnDestroy {

  isEditMode: boolean = false;
  dialogueTexts = DialogueTexts;
  Colors = Colors;
  iconTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  protectiveEquipment: ProtectiveEquipment[] = [];
  comment: string;
  selectedEquipment = null;
  protectiveEquipmentSessionType: SessionType = SessionType.ProtectiveEquipment;

  canNotSaveMessage = DialogueTexts.CanNotSaveProtectiveEquipmentObservation;

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
  @Input("facilityid") facilityid: number;
  @Output("observationDeletedEvent") observationDeletedEvent = new EventEmitter();


  ngOnInit(): void {
    this.protectiveEquipment = this.observation.protectiveEquipmentList;
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  registerComment(comment: string) {
    this.observation.comment = comment;
  }

  saveObservation() {
    if(!this.canSave()){
      this.toastrService.error(this.canNotSaveMessage, '', {disableTimeOut: true})
      return;
    }
    if (this.isEditMode) {
      this.sessionService.changeObservation(this.observation);
    }
    this.isEditMode = false;
  }

  deleteObservation() {
    this.sessionService.deleteObservation(this.observation);
    this.observationDeletedEvent.emit();
  }

  protectiveEquipmentRequired(): ProtectiveEquipment[] {
    return this.protectiveEquipment.filter(b => b.isRequired);
  }

  protectiveEquipmentNotRequired(): ProtectiveEquipment[] {
    return this.protectiveEquipment.filter(b => b.isRequired === false);
  }

  changed(event, selection: ProtectiveEquipment) {
    event.srcElement.blur();
    event.preventDefault();

    selection.wasUsed = true;
    selection.equipmentType.misuseTypes.filter(fb => fb.isSelected == true).map(fb => fb.isSelected = false);

    selection.misuseTypes.forEach(f => {
      const index = selection.equipmentType.misuseTypes.findIndex(fb => fb.id == f.id);
      selection.equipmentType.misuseTypes[index].isSelected = true;
    });

    if (selection.wasUsed) {
      this.showModal(selection);
    }
  }

  showModal(selectedEquipment: ProtectiveEquipment) {

    const modalRef = this.modalService.open(ProtectiveEquipmentModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: ProtectiveEquipmentModalComponentConfig.windowClass
    });

    modalRef.componentInstance.selectedEquipment = JSON.parse(JSON.stringify(selectedEquipment)) as ProtectiveEquipment;

    modalRef.componentInstance.displayMode = !this.isEditMode;

    if (this.isEditMode && selectedEquipment.wasUsed) {
      if (selectedEquipment.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '') {
        modalRef.componentInstance.showEquipmentDeleteButton = true;
      }
      else {
        modalRef.componentInstance.selectedEquipment.wasUsedCorrectly = null;
      }
    }

    modalRef.result.then((result: ProtectiveEquipment) => {
      if (!this.isEditMode) {
        return;
      }
      selectedEquipment.isRequired = result.isRequired;
      selectedEquipment.equipmentType.isRequired = result.isRequired;
      if (result.wasUsed === false) {
        this.resetEquipment(selectedEquipment);
      }
      else {
        selectedEquipment.wasUsedCorrectly = result.wasUsedCorrectly;
        selectedEquipment.comment = result.comment;
        selectedEquipment.misuseTypes = result.equipmentType.misuseTypes.filter(x => x.isSelected);
        selectedEquipment.wasUsed = result.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '';
      }
    }, (reason) => {
      if (this.isEditMode) {
        selectedEquipment.wasUsed = false;
        this.resetEquipment(selectedEquipment);
      }
    });
  }

  setAllEquipmentToProperUsed(event) {
    this.protectiveEquipmentRequired().forEach(x => {
      x.wasUsed = true;
      x.wasUsedCorrectly = true;
    });
  }

  resetEquipment(selection: ProtectiveEquipment) {
    let selectedIndex = this.protectiveEquipment.findIndex(x => x.equipmentType.id === selection.equipmentType.id);
    this.protectiveEquipment[selectedIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.observation.settingType.equipmentTypes).find(x => x.equipmentType.id === selection.equipmentType.id);
  }

  showDisplayModeModal(event, selection: ProtectiveEquipment) {
    if (!this.isEditMode && selection.wasUsed) {
      event.stopPropagation();
      event.preventDefault();
      this.showModal(selection);
      return;
    }
  }

  canSave() {
    return this.sessionService.numberOfQualifiedEquipment(this.observation.protectiveEquipmentList) > 0;
  }
}
