import { Component, Input, OnInit } from "@angular/core";
import { Colors } from "../../utils/colors";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { ProtectiveEquipmentMapper } from "../../utils/protectiveEquipment-mapper";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';

export const ProtectiveEquipmentModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-protective-equipment-modal',
  templateUrl: './protective-equipment-modal.component.html',
})
export class ProtectiveEquipmentModalComponent implements OnInit {

  faCircle = faCircle;
  colors = Colors;
  iconTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();

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
    this.selectedEquipment.wasUsed == false;
    this.activeModal.dismiss('lukk');
  }

  resetIncorrectUseAndMarkUsed(val: boolean) {
    this.selectedEquipment.wasUsed = true;
    this.selectedEquipment.wasUsedCorrectly = val;
  }

  canSave() : boolean {
    return this.selectedEquipment.wasUsedCorrectly || this.hasRegisteredMisuseOrComment()
  }

  hasRegisteredMisuseOrComment() : boolean {
    return this.selectedEquipment.equipmentType.misuseTypes?.filter(fb => fb.isSelected).length > 0 || this.selectedEquipment.comment?.length > 0;
  }
}
