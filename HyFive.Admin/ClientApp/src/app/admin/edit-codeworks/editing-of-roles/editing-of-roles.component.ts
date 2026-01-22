import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Role } from '../../../models/api/Role';
import { RoleService } from '../../../services/data/role.service';
import { KeyEventService } from '../../../services/events/key-event.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-editing-of-roles',
  templateUrl: './editing-of-roles.component.html'

})
export class EditingOfRolesComponent implements OnInit, OnDestroy {

  roles: Role[];

  roleAsChanged: Role = null;
  newRole: Role = this.createEmptyRole();
  disableCreate: boolean = true;


  constructor(
    private readonly roleService: RoleService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService
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
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Roles:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  createEmptyRole() {
    return {
      id: 0,
      name: '',
      description: '',
      institutionIds: []
    } as Role;
  }

  createRole() {
    if (this.newRole.description == "") {
      this.newRole.description = null;
    }
    this.roleService.createRole(this.newRole).subscribe(
      (createdRole) => this.toastrService.success(this.translate.instant('Role created')),
      error => this.toastrService.error(this.translate.instant('An error occurred while creating the Role:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
      () => { this.newRole = this.createEmptyRole(); this.loadRoles(); }
    );
  }

  selectedRole(role: Role): void {
    if (this.roleAsChanged?.id == role.id) return;
    this.roleAsChanged = structuredClone(role);
  }

  updateRole(role: Role) {
        if (role.description == "") {
      role.description = null;
    }
    this.roleService.updateRole(role).subscribe(
      (updateRole) => {
        this.toastrService.success(this.translate.instant("Role updated"));
        this.loadRoles();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating Role:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
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
