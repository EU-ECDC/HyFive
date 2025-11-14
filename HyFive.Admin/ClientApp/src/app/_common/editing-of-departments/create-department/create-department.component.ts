import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChange, SimpleChanges, OnDestroy} from '@angular/core';
import {CreateDepartmentRequest} from '../../../models/api/CreateDepartmentRequest';
import {DepartmentService} from '../../../services/data/department.service';
import {Department} from '../../../models/api/Department';
import {ToastrService} from 'ngx-toastr';
import {RoleSelected} from '../../../models/code-work/roleSelected.model';
import {DepartmentType} from '../../../models/api/DepartmentType';
import { Role } from '../../../models/api/Role';
import { RoleService } from '../../../services/data/role.service';

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


  constructor(private roleService: RoleService, private departmentService: DepartmentService, private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.resetForm();
    this.loadDepartmentTypes();
    this.roleService.getRoles().subscribe(
      roles => {
      this.roles = roles;
      this.roleSelected = roles.map<RoleSelected>((r) => ({role: r, isSelected: true}) );
      },
      error => this.toastrService.error(`Something went wrong while loading roles for facility: ${error?.message ? error.message : error}`, '', { disableTimeOut: true})
    );
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  createDepartment() {
    this.newDepartment.roleIds = this.roleSelected.filter(r => r.isSelected).map(r => r.role.id);
    this.departmentService.createDepartment(this.newDepartment).subscribe((department) => {
        this.toastrService.success('Department created', `Department with ID: ${department.id} created`);
        this.roleSelected = this.roles.map<RoleSelected>((r) => ({role: r, isSelected: false}) );
        this.departmentCreatedEvent.emit(department);
      },
      (error) => this.toastrService.error(`An error occurred while creating the department. Error message from server: ${error?.message ? error.message : error}`, 'Error creating department', { disableTimeOut: true}),
      () => { this.resetForm();  }
    );
  }

  loadDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = [ { id: 0, code: '', name: 'Not selected' }, ...departmentTypes ];
      },
      (err) => this.toastrService.error(`Could not load roles: ${err?.message ? err.message : err}`, 'Technical error', { disableTimeOut: true})
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
      && this.departments.find(dep => dep.name == this.newDepartment.name) == undefined;
  }

  omitSpecialChar(event)
    {   
      let k;  
      k = event.charCode;  //         k = event.keyCode;  (Both can be used)
      return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
    }

}
