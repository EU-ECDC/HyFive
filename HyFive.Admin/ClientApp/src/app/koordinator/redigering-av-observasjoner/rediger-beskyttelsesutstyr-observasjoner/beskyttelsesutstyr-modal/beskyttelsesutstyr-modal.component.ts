import { Component, Input, OnInit } from "@angular/core";

import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { Beskyttelsesutstyr } from 'src/app/models/api/Beskyttelsesutstyr';
import { FeilbrukType } from 'src/app/models/api/FeilbrukType';
import { BeskyttelsesutstyrMapper } from 'src/app/utils/beskyttelsesutstyrmapper';
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
  ikonTypeMap: Map<string, IconProp> = BeskyttelsesutstyrMapper.getIkontypeMap();
  feilbrukTyper: FeilbrukType[] = [];

  closeResult = '';

  @Input() valgtUtstyr: Beskyttelsesutstyr;
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
    this.valgtUtstyr.bleBenyttet = false;
    this.activeModal.dismiss('lukk');
  }

  nullstillFeilbrukOgMarkerBenyttet(val: boolean) {
    this.valgtUtstyr.bleBenyttet = true;
    this.valgtUtstyr.bleBenyttetRiktig = val;
  }

  aktiverTilbakeTilKortVedFeilbruk(feilbruk: FeilbrukType) {
    if (feilbruk) {
      if (feilbruk.erValgt === false) {
        this.feilbrukTyper.push(feilbruk);
      }
      else {
        const index = this.feilbrukTyper.indexOf(feilbruk);
        this.feilbrukTyper.splice(index, 1);
      }
    }
  }

  kanLagre() : boolean {
    return this.valgtUtstyr.bleBenyttetRiktig || this.harRegistrertFeilbrukEllerKommentar()
  }

  harRegistrertFeilbrukEllerKommentar() : boolean {
    return this.valgtUtstyr.utstyrstype.feilbruktyper?.filter(fb => fb.erValgt).length > 0 || this.valgtUtstyr.kommentar?.length > 0;
  }
}
