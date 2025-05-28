import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Role} from '../../models/api/Role';
import { DepartmentService } from '../../services/data/department.service';

@Component({
  selector: 'app-role-selection-dropdown',
  templateUrl: './app-role-selection-dropdown.component.html'
})
export class RoleSelectionDropdownComponent implements OnInit{

  @Input('departmentId') departmentId : number;
  @Input('roleId') roleId: number;
  @Input() customClass = "";
  @Output('roleSelected') roleSelected: EventEmitter<Role> = new EventEmitter<Role>();
  selectedRole: Role;
  roles: Role[] = [];

  selectedRoleId: string;


  constructor(private departmentService: DepartmentService){}

  ngOnInit() {
    this.departmentService.getRole(this.departmentId).subscribe(
      (roles) => {
      this.roles = roles;
      this.selectedRoleId = this.roleId+'';
      this.selectRole();
      }
    );
  }

  selectedRoleChanged() {
    this.selectRole();
    this.roleSelected.emit(this.selectedRole);
  }

  selectRole() {
    if (this.roles){
      this.selectedRole = this.roles[this.roles.map(r => r.id).indexOf(parseInt(this.selectedRoleId))];
    }
  }
}
