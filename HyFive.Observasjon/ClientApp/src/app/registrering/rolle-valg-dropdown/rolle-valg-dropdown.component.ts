import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Farger} from '../../utils/farger'
import {Rolle} from '../../models/api/Rolle';
import {Uuid} from '../../utils/uuid';
import { faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-rolle-valg-dropdown',
  templateUrl: './rolle-valg-dropdown.component.html'
})
export class RolleValgDropdownComponent implements OnInit{

  @Input('isReadonly') isReadonly = false;
  @Input('rollevalg') rollevalg : Rolle[];
  @Input('rolleId') rolleId: number;
  @Output('rolleValgt') rolleValgt: EventEmitter<Rolle> = new EventEmitter<Rolle>();
  valgtRolle: Rolle;

  valgtRolleId: string;
  farger = Farger;
  id: string = Uuid.generateUUID().substr(4);
  faAngleDown = faAngleDown;

  ngOnInit() {
    this.valgtRolleId = this.rolleId+'';
    this.velgRolle();
  }

  valgtRolleEndret() {
    this.velgRolle();
    this.rolleValgt.emit(this.valgtRolle);
  }

  velgRolle() {
    if (this.rollevalg){
      this.valgtRolle = this.rollevalg[this.rollevalg.map(r => r.id).indexOf(parseInt(this.valgtRolleId))];
    }

  }

  hentFarge() : string {
    return this.valgtRolle && this.isReadonly ? this.farger.hentFhiFargeHexFraTekst(this.valgtRolle.navn) : 'white';
  }
}
