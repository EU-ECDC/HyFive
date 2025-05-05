import { Component, Input, OnInit } from "@angular/core";
import { Colors } from "../../utils/colors";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { ProtectiveEquipmentMapper } from "../../utils/protectiveEquipment-mapper";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';

export const BeskyttelsesutstyrModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-beskyttelsesutstyr-modal',
  templateUrl: './beskyttelsesutstyr-modal.component.html',
})
export class BeskyttelsesutstyrModalComponent implements OnInit {

  faCircle = faCircle;
  farger = Colors;
  ikonTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();

  closeResult = '';

  @Input() valgtUtstyr: ProtectiveEquipment;
  @Input() visningsmodus = false;
  @Input() visKnappForSlettingAvUtstyr: boolean = false;

  constructor(private activeModal: NgbActiveModal) {
  }

  ngOnInit(): void {
  }

  close() {
    this.activeModal.close(this.valgtUtstyr);
  }

  dismiss() {
    this.valgtUtstyr.wasUsed == false;
    this.activeModal.dismiss('lukk');
  }

  nullstillFeilbrukOgMarkerBenyttet(val: boolean) {
    this.valgtUtstyr.wasUsed = true;
    this.valgtUtstyr.wasUsedCorrectly = val;
  }

  kanLagre() : boolean {
    return this.valgtUtstyr.wasUsedCorrectly || this.harRegistrertFeilbrukEllerKommentar()
  }

  harRegistrertFeilbrukEllerKommentar() : boolean {
    return this.valgtUtstyr.equipmentType.misuseTypes?.filter(fb => fb.isSelected).length > 0 || this.valgtUtstyr.comment?.length > 0;
  }
}
