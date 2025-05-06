import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Colors} from '../../utils/colors'
import {Role} from '../../models/api/Role';
import {Uuid} from '../../utils/uuid';
import { faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-rolle-valg-dropdown',
  templateUrl: './rolle-valg-dropdown.component.html'
})
export class RolleValgDropdownComponent implements OnInit{

  @Input('isReadonly') isReadonly = false;
  @Input('roleSelected') roleSelected : Role[];
  @Input('rolleId') rolleId: number;
  @Output('rolleValgt') rolleValgt: EventEmitter<Role> = new EventEmitter<Role>();
  valgtRolle: Role;

  valgtRolleId: string;
  colors = Colors;
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
    if (this.roleSelected){
      this.valgtRolle = this.roleSelected[this.roleSelected.map(r => r.id).indexOf(parseInt(this.valgtRolleId))];
    }

  }

  hentFarge() : string {
    return this.valgtRolle && this.isReadonly ? this.colors.getFhiColorHexFromText(this.valgtRolle.name) : 'white';
  }
}
