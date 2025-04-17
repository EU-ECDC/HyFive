import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Role} from '../../models/api/Role';
import { AvdelingService } from '../../services/data/avdeling.service';

@Component({
  selector: 'app-rolle-valg-dropdown',
  templateUrl: './rolle-valg-dropdown.component.html'
})
export class RolleValgDropdownComponent implements OnInit{

  @Input('avdelingId') avdelingId : number;
  @Input('rolleId') rolleId: number;
  @Input() customClass = "";
  @Output('rolleValgt') rolleValgt: EventEmitter<Role> = new EventEmitter<Role>();
  valgtRolle: Role;
  roller: Role[] = [];

  valgtRolleId: string;


  constructor(private avdelingService: AvdelingService){}

  ngOnInit() {
    this.avdelingService.hentRoller(this.avdelingId).subscribe(
      (roller) => {
      this.roller = roller;
      this.valgtRolleId = this.rolleId+'';
      this.velgRolle();
      }
    );
  }

  valgtRolleEndret() {
    this.velgRolle();
    this.rolleValgt.emit(this.valgtRolle);
  }

  velgRolle() {
    if (this.roller){
      this.valgtRolle = this.roller[this.roller.map(r => r.id).indexOf(parseInt(this.valgtRolleId))];
    }
  }
}
