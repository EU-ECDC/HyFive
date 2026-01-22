import {Component, EventEmitter, Input, OnInit, Output, OnDestroy} from '@angular/core';
import {CreateDepartmentRequest} from '../../../models/api/CreateDepartmentRequest';
import {DepartmentService} from '../../../services/data/department.service';
import {Department} from '../../../models/api/Department';
import {ToastrService} from 'ngx-toastr';
import {RoleSelected} from '../../../models/code-work/roleSelected.model';
import {DepartmentType} from '../../../models/api/DepartmentType';
import { Role } from '../../../models/api/Role';
import { RoleService } from '../../../services/data/role.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-create-department',
  templateUrl: './create-department.component.html'
})
export class CreateDepartmentComponent implements OnInit, OnDestroy {

  newDepartment: CreateDepartmentRequest;
  roleSelected: RoleSelected[] = [];
  departmentTypes: DepartmentType[];
  roles: Role[] = [];

  @Input() facilityId: number;
  @Input() departments: Department[] = [];
  @Output() departmentCreatedEvent: EventEmitter<Department> = new EventEmitter<Department>();
  @Output() resetFormEvent: EventEmitter<boolean> = new EventEmitter<boolean>();


  constructor(private readonly roleService: RoleService,
              private readonly departmentService: DepartmentService, 
              private readonly toastrService: ToastrService,
              private readonly translate: TranslateService) { }

  ngOnInit(): void {
    this.resetForm();
    this.loadDepartmentTypes();
    this.roleService.getRoles().subscribe(
      roles => {
      this.roles = roles;
      this.roleSelected = roles.map<RoleSelected>((r) => ({role: r, isSelected: true}) );
      },
      error => this.toastrService.error(this.translate.instant('Something went wrong while loading roles for facility:') + `${error?.error.message ? error.message : error}`, '', { disableTimeOut: true})
    );
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  createDepartment() {
    this.newDepartment.name =
    this.sanitizeDepartmentName(this.newDepartment.name);

    if (!this.newDepartment.name) {
      return;
    }
    
    this.newDepartment.roleIds = this.roleSelected.filter(r => r.isSelected).map(r => r.role.id);
    this.departmentService.createDepartment(this.newDepartment).subscribe((department) => {
        this.toastrService.success(this.translate.instant('Department created'), this.translate.instant('Department with ID:') + ' ' + `${department.id}` + ' ' + this.translate.instant('created'));
        this.roleSelected = this.roles.map<RoleSelected>((r) => ({role: r, isSelected: false}) );
        this.departmentCreatedEvent.emit(department);
      },
      (error) => this.toastrService.error(this.translate.instant('An error occurred while creating the department. Error message from server:') + ' ' + ` ${error?.error.message ? error.message : error}`, this.translate.instant('Error creating department'), { disableTimeOut: true}),
      () => { this.resetForm();  }
    );
  }

  loadDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = [ { id: 0, code: '', name: this.translate.instant('Not selected') }, ...departmentTypes ];
      },
      (err) => this.toastrService.error(this.translate.instant('Could not load roles:') + `${err?.error.message ? err?.error.message : err}`, this.translate.instant('Technical error'), { disableTimeOut: true})
    );
  }

  resetForm() {
    this.newDepartment = {
      name: null,
      facilityId: this.facilityId,
      departmentTypeId: 0,
      roleIds: []
    };
  }

  canNotCreateDepartment(): boolean {
    return this.canCreateDepartment() === false;
  }

  canCreateDepartment(): boolean{
    return this.newDepartment.facilityId > 0
      && this.newDepartment.departmentTypeId > 0
      && this.roleSelected?.filter(r => r.isSelected)?.length > 0
      && this.newDepartment.name?.length > 0
      && !this.departments.some(dep => dep.name == this.newDepartment.name)
  }

  omitSpecialChar(event)
  {   
       const char = event.key;

    // Allow letters, numbers, spaces, basic punctuation
    const allowed = /^[a-zA-Z0-9\s.,'-]$/;

    if (!allowed.test(char)) {
      event.preventDefault();
      return false;
    }

    return true;
  }

  sanitizeDepartmentName(value: string): string {
    if (!value) {
      return value;
    }

    // Remove emojis & symbols (Unicode ranges)
    const noEmojis = value.replaceAll(
      /[\p{Extended_Pictographic}\p{Emoji_Presentation}\p{Symbol}]/gu,
      ''
      );

    return noEmojis.trim();
  }

  cancelForm() {
    this.resetFormEvent.emit(true);
  }
}
