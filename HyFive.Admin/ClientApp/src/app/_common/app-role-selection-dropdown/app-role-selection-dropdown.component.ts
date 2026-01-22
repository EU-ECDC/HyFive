import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Role} from '../../models/api/Role';
import { DepartmentService } from '../../services/data/department.service';

@Component({
  selector: 'app-role-selection-dropdown',
  templateUrl: './app-role-selection-dropdown.component.html'
})
export class RoleSelectionDropdownComponent implements OnInit{

  @Input() departmentId : number;
  @Input() roleId: number;
  @Input() customClass = "";
  @Output() roleSelected: EventEmitter<Role> = new EventEmitter<Role>();
  selectedRole: Role;
  roles: Role[] = [];

  selectedRoleId: number;


  constructor(private readonly departmentService: DepartmentService){}

  ngOnInit() {
    this.departmentService.getRole(this.departmentId).subscribe(
      (roles) => {
      this.roles = roles;
      this.selectedRoleId = this.roleId;
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
      this.selectedRole = this.roles[this.roles.map(r => r.id).indexOf(this.selectedRoleId)];
    }
  }
}
