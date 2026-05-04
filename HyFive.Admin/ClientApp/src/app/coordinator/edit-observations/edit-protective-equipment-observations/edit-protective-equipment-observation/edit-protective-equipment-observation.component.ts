import { Component, Input, OnInit, EventEmitter, Output } from '@angular/core';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProtectiveEquipment } from 'src/app/models/api/ProtectiveEquipment';
import { ProtectiveEquipmentObservation } from "src/app/models/api/ProtectiveEquipmentObservation";
import { ProtectiveEquipmentMapper } from 'src/app/utils/protective-equipment-mapper';
import {ToastrService} from "ngx-toastr";
import { ProtectiveEquipmentModalComponent, ProtectiveEquipmentModalComponentConfig } from '../protective-equipment-modal/protective-equipment-modal.component';
import { ObservationService } from 'src/app/services/data/observation.service';
import { Role } from "../../../../models/api/Role";
import { Colors } from 'src/app/utils/Colors';
import {ProtectiveEquipmentSettingTypesService} from "../../../../services/data/protectiveEquipmentSettingTypes.service";
import {ProtectiveEquipmentSettingType} from "../../../../models/api/ProtectiveEquipmentSettingType";


@Component({
  selector: 'app-edit-protective-equipment-observation',
  templateUrl: './edit-protective-equipment-observation.component.html'
})
export class EditProtectiveEquipmentObservationComponent implements OnInit {

  Colors = Colors;
  iconTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  protectiveEquipment: ProtectiveEquipment[] = [];
  settings: ProtectiveEquipmentSettingType[];
  selectedSetting: ProtectiveEquipmentSettingType;
  comment: string;
  selectedEquipment = null;
  observation: ProtectiveEquipmentObservation;
  observationLoading: boolean = true;

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faCircle = faCircle;
  faCheck = faCheck;

  constructor(
    private readonly modalService: NgbModal,
    private readonly observationService: ObservationService,
    private readonly settingService: ProtectiveEquipmentSettingTypesService,
    private readonly toastrService: ToastrService) { }

  @Input() isReadonly: boolean = false;
  @Input() observationId: string;

  @Input() departmentId: number;
  @Input() facilityId: number;
  @Input() sessionId: string;
  @Output() observationDeletedEvent = new EventEmitter();
  @Output() observationUpdatedEvent = new EventEmitter<ProtectiveEquipmentObservation>();

  ngOnInit(): void {
    this.observationService.getProtectiveEquipmentObservation(this.observationId, this.sessionId).subscribe(
      (o) => {
        this.observation = o;
        this.protectiveEquipment = this.observation.protectiveEquipmentList;
        this.observation.sessionId = this.sessionId;
        this.settingService.getProtectiveEquipmentSettingTypes().subscribe((settings) => {
          this.settings = settings;
          this.selectedSetting = this.observation.settingType;
        })
      },
      (error) => this.toastrService.error("An error occurred while loading observation with id " + this.observationId, '', {disableTimeOut: true}),
      () => this.observationLoading = false
    );

  }

  registerComment(comment: string) {
    this.observation.comment = comment;
    this.update();
  }

  protectiveEquipmentIndicated(): ProtectiveEquipment[] {
    return this.protectiveEquipment.filter(b => b.isRequired);
  }

  protectiveEquipmentNotIndicated(): ProtectiveEquipment[] {
    return this.protectiveEquipment.filter(b => b.isRequired === false);
  }

  changed(event, select: ProtectiveEquipment) {
    event.srcElement.blur();
    event.preventDefault();

    select.wasUsed = true;
    for (const type of select.equipmentType.misuseTypes.filter(fb => fb.isSelected === true)) {
      type.isSelected = false;
    }

    for (const f of select.misuseTypes) {
      const index = select.equipmentType.misuseTypes.findIndex(fb => fb.id == f.id);
      select.equipmentType.misuseTypes[index].isSelected = true;
    };

    if (select.wasUsed) {
      this.showModal(select);
    }
    this.update();
  }

  showModal(selectedEquipment: ProtectiveEquipment) {

    const modalRef = this.modalService.open(ProtectiveEquipmentModalComponent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: ProtectiveEquipmentModalComponentConfig.windowClass
    });

    modalRef.componentInstance.selectedEquipment = structuredClone(selectedEquipment);

    modalRef.componentInstance.displayMode = false;

    if (selectedEquipment.wasUsed) {
      if (selectedEquipment.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '') {
        modalRef.componentInstance.showEquipmentDeleteButton = true;
      }
      else {
        modalRef.componentInstance.selectedEquipment.wasUsedCorrectly = null;
      }
    }

    modalRef.result.then((result: ProtectiveEquipment) => {
      selectedEquipment.isRequired = result.isRequired;
      if (result.wasUsed === false) {
        this.resetEquipment(selectedEquipment);
      }
      else {
        selectedEquipment.wasUsedCorrectly = result.wasUsedCorrectly;
        selectedEquipment.comment = result.comment;
        selectedEquipment.misuseTypes = result.equipmentType.misuseTypes.filter(x => x.isSelected);
        selectedEquipment.wasUsed = result.wasUsedCorrectly || selectedEquipment.misuseTypes.length > 0 || selectedEquipment.comment !== '';
      }
      this.update();
    }, (error) => {
      selectedEquipment.wasUsed = false;
      this.resetEquipment(selectedEquipment);
    });
  }

  setAllEquipmentToProperUsed(event) {
    for (const x of this.protectiveEquipmentIndicated()) {
      x.wasUsed = true;
      x.wasUsedCorrectly = true;
    };
    this.update();
  }

  resetEquipment(select: ProtectiveEquipment) {
    let selectIndex = this.protectiveEquipment.findIndex(x => x.equipmentType.id === select.equipmentType.id);
    this.protectiveEquipment[selectIndex] = ProtectiveEquipmentMapper.getProtectiveEquipmentSelection(this.observation.settingType.equipmentTypes, select).find(x => x.equipmentType.id === select.equipmentType.id);
  }

  selectRole($event: Role) {
    this.observation.role = $event;
    this.update();
  }

  selectSetting() {
    this.observation.settingType = this.selectedSetting;
    this.update();
  }

  update(){
    this.observationUpdatedEvent.emit(this.observation);
  }
  sortedSettings() : ProtectiveEquipmentSettingType[] {
    let combinedSettings = this.settings.filter(s => s.code != this.selectedSetting.code)
    combinedSettings.push(this.selectedSetting);
    combinedSettings = combinedSettings.toSorted((a, b) => {
      if(a.name < b.name) { return -1; }
      if(a.name > b.name) { return 1; }
      return 0;});

    return combinedSettings;
  }
}
