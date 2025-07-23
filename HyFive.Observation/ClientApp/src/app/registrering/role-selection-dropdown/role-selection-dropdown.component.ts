import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Colors} from '../../utils/colors'
import {Role} from '../../models/api/Role';
import {Uuid} from '../../utils/uuid';
import { faAngleDown } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-role-selection-dropdown',
  templateUrl: './role-selection-dropdown.component.html',
  styleUrls: ['role-selection-dropdown.component.scss']
})
export class RoleSelectDropdownComponent implements OnInit{

  @Input('isReadonly') isReadonly = false;
  @Input('roleSelectedI') roleSelectedI : Role[];
  @Input('roleId') roleId: number;
  @Output('roleSelectedEvent') roleSelectedEvent: EventEmitter<Role> = new EventEmitter<Role>();
  selectedRole: Role;

  selectedRoleId: number;
  colors = Colors;
  id: string = Uuid.generateUUID().substr(4);
  faAngleDown = faAngleDown;

  ngOnInit() {
    this.selectedRoleId = this.roleId;
    this.selectRole();
  }

  selectedRoleChanged() {
    this.selectRole();
    this.roleSelectedEvent.emit(this.selectedRole);
  }

  selectRole() {
    if (this.roleSelectedI){
      this.selectedRole = this.roleSelectedI[this.roleSelectedI.map(r => r.id).indexOf(this.selectedRoleId)];
    }

  }

  getColor() : string {
    return this.selectedRole && this.isReadonly ? this.colors.getFhiColorHexFromText(this.selectedRole.name) : 'white';
  }
}
