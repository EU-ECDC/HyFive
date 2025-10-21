import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Role } from '../../../models/api/Role';
import { RoleService } from '../../../services/data/role.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-roles',
  templateUrl: './editing-of-roles.component.html'

})
export class EditingOfRolesComponent implements OnInit, OnDestroy {

  roles: Role[];

  roleAsChanged: Role = null;
  newRole: Role = this.createEmptyRole();
  disableCreate: boolean = true;


  constructor(
    private roleService: RoleService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadRoles();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  createEmptyRole() {
    return {
      id: 0,
      name: '',
      description: '',
      institusjonIder: []
    } as Role;
  }

  createRole() {
    if (this.newRole.description == "") {
      this.newRole.description = null;
    }
    this.roleService.createRole(this.newRole).subscribe(
      (opprettetRolle) => this.toastrService.success('Role created'),
      error => this.toastrService.error('An error occurred while creating the role: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.newRole = this.createEmptyRole(); this.loadRoles(); }
    );
  }

  selectedRole(role: Role): void {
    if (this.roleAsChanged?.id == role.id) return;
    this.roleAsChanged = JSON.parse(JSON.stringify(role));
  }

  updateRole(role: Role) {
        if (role.description == "") {
      role.description = null;
    }
    this.roleService.updateRole(role).subscribe(
      (updateRole) => {
        this.toastrService.success("Role updated");
        this.loadRoles();
      },
      error => this.toastrService.error('An error occurred while updating role: ' + error?.error, '', { disableTimeOut: true}),
      () => this.roleAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.roleAsChanged = null;
  }
}
