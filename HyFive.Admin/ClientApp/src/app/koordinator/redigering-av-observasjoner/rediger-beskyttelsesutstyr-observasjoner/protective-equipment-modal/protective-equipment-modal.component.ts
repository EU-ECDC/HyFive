import { Component, Input, OnInit } from "@angular/core";

import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ProtectiveEquipment } from 'src/app/models/api/ProtectiveEquipment';
import { MisuseType } from 'src/app/models/api/MisuseType';
import { ProtectiveEquipmentMapper } from 'src/app/utils/protective-equipment-mapper';
import {Farger} from "../../../../../../../../HyFive.Observasjon/ClientApp/src/app/utils/farger";

export const ProtectiveEquipmentModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-protective-equipment-modal',
  templateUrl: './protective-equipment-modal.component.html',
})
export class ProtectiveEquipmentModalComponent implements OnInit {

  faCircle = faCircle;
  farger = Farger;
  iconTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  misuseTypes: MisuseType[] = [];

  closeResult = '';

  @Input() selectedEquipment: ProtectiveEquipment;
  @Input() displayMode = false;
  @Input() showEquipmentDeleteButton: boolean = false;

  constructor(private activeModal: NgbActiveModal) {
  }

  ngOnInit(): void {
  }

  close() {
    this.activeModal.close(this.selectedEquipment);
  }

  dismiss() {
    this.selectedEquipment.wasUsed = false;
    this.activeModal.dismiss('lock');
  }

  resetErrorAndMarkerUsed(selection: boolean) {
    this.selectedEquipment.wasUsed = true;
    this.selectedEquipment.wasUsedProperly = selection;
  }

  enableBackToCardOnError(misuse: MisuseType) {
    if (misuse) {
      if (misuse.isSelected === false) {
        this.misuseTypes.push(misuse);
      }
      else {
        const index = this.misuseTypes.indexOf(misuse);
        this.misuseTypes.splice(index, 1);
      }
    }
  }

  canSave() : boolean {
    return this.selectedEquipment.wasUsedProperly || this.hasRegisteredMisuseOrComment()
  }

  hasRegisteredMisuseOrComment() : boolean {
    return this.selectedEquipment.equipmentTypeq.misuseTypes?.filter(fb => fb.isSelected).length > 0 || this.selectedEquipment.comment?.length > 0;
  }
}
