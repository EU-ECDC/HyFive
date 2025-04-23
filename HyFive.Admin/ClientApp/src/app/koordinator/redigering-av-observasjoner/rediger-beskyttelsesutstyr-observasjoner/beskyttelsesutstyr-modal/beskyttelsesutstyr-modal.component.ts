import { Component, Input, OnInit } from "@angular/core";

import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { ProtectiveEquipment } from 'src/app/models/api/ProtectiveEquipment';
import { MisuseType } from 'src/app/models/api/MisuseType';
import { ProtectiveEquipmentMapper } from 'src/app/utils/protective-equipment-mapper';
import {Farger} from "../../../../../../../../HyFive.Observasjon/ClientApp/src/app/utils/farger";

export const BeskyttelsesutstyrModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-beskyttelsesutstyr-modal',
  templateUrl: './beskyttelsesutstyr-modal.component.html',
})
export class BeskyttelsesutstyrModalComponent implements OnInit {

  faCircle = faCircle;
  farger = Farger;
  ikonTypeMap: Map<string, IconProp> = ProtectiveEquipmentMapper.getIconTypeMap();
  feilbrukTyper: MisuseType[] = [];

  closeResult = '';

  @Input() selectedEquipment: ProtectiveEquipment;
  @Input() visningsmodus = false;
  @Input() visKnappForSlettingAvUtstyr: boolean = false;

  constructor(private activeModal: NgbActiveModal) {
  }

  ngOnInit(): void {
  }

  close() {
    this.activeModal.close(this.selectedEquipment);
  }

  dismiss() {
    this.selectedEquipment.wasUsed = false;
    this.activeModal.dismiss('lukk');
  }

  nullstillFeilbrukOgMarkerBenyttet(val: boolean) {
    this.selectedEquipment.wasUsed = true;
    this.selectedEquipment.wasUsedProperly = val;
  }

  aktiverTilbakeTilKortVedFeilbruk(feilbruk: MisuseType) {
    if (feilbruk) {
      if (feilbruk.isSelected === false) {
        this.feilbrukTyper.push(feilbruk);
      }
      else {
        const index = this.feilbrukTyper.indexOf(feilbruk);
        this.feilbrukTyper.splice(index, 1);
      }
    }
  }

  kanLagre() : boolean {
    return this.selectedEquipment.wasUsedProperly || this.harRegistrertFeilbrukEllerKommentar()
  }

  harRegistrertFeilbrukEllerKommentar() : boolean {
    return this.selectedEquipment.equipmentTypeq.misuseTypes?.filter(fb => fb.isSelected).length > 0 || this.selectedEquipment.comment?.length > 0;
  }
}
