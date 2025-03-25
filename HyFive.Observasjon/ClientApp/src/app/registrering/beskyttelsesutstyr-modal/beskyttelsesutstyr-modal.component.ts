import { Component, Input, OnInit } from "@angular/core";
import { Farger } from "../../utils/farger";
import { IconProp } from "@fortawesome/fontawesome-svg-core";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { BeskyttelsesutstyrMapper } from "../../utils/beskyttelsesutstyrmapper";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { Beskyttelsesutstyr } from '../../models/api/Beskyttelsesutstyr';

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
    this.valgtUtstyr.bleBenyttet == false;
    this.activeModal.dismiss('lukk');
  }

  nullstillFeilbrukOgMarkerBenyttet(val: boolean) {
    this.valgtUtstyr.bleBenyttet = true;
    this.valgtUtstyr.bleBenyttetRiktig = val;
  }

  kanLagre() : boolean {
    return this.valgtUtstyr.bleBenyttetRiktig || this.harRegistrertFeilbrukEllerKommentar()
  }

  harRegistrertFeilbrukEllerKommentar() : boolean {
    return this.valgtUtstyr.utstyrstype.feilbruktyper?.filter(fb => fb.erValgt).length > 0 || this.valgtUtstyr.kommentar?.length > 0;
  }
}
