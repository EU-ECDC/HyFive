import { Component, Input, OnInit } from '@angular/core';
import { Department} from '../../models/api/Department';
import { InstitutionService } from '../../services/data/institution.service';
import { DepartmentType } from "../../models/api/DepartmentType";
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { DepartmentService } from 'src/app/services/data/department.service';
import { Role } from 'src/app/models/api/Role';
import { RoleService } from 'src/app/services/data/role.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
@Component({
selector: 'app-editing-of-departments',
  templateUrl: './editing-of-departments.component.html'
})
export class EditingDepartmentsComponent implements OnInit {
  
  @Input() institutionId: number;

  departments: Department[] = [];
  filteredDepartments: Department[] = [];
  departmentId = 0;
  departmentAsChanged: Department;
  departmentTypes: DepartmentType[];
  roles: Role[];
  selectedRoles: Role[] = [];
  canEdit: boolean;
  institutionName: string;
  keyword: string;
  loading: boolean = false;
  dropdownSettings: IDropdownSettings;

  constructor(private institutionService: InstitutionService,
    private authorizationService: AuthorizationService,
    private departmentService: DepartmentService,
    private roleService: RoleService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService) { }

  ngOnInit(): void {
    this.loading = true;
    
    this.canEdit = this.canUserEdit();

    this.getDepartments();
    this.getDepartmentTypes();
    this.getRoles();

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select all',
      unSelectAllText: 'Select all',
      itemsShowLimit: 5
    };

    this.loading = false;
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit(event);
    });
  }

  private canUserEdit() {
    let role = this.authorizationService.getSelectedRole();
    if(role === AuthorizedRole.Coordinator || role === AuthorizedRole.Administrator)
      return true;
    return false;
  }

  getDepartments() {
    let selectedInstitutionId = this.institutionId ?? this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(selectedInstitutionId).subscribe(
      (institution) => {
        this.institutionName = institution.name;
        this.institutionId = institution.id;
        this.departments = institution.departments;
        this.filteredDepartments = this.departments;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Loading institutions failed', {disableTimeOut: true});
    });
  }

  getDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = departmentTypes;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Loading departmentType failed', {disableTimeOut: true});
    });
  }

  getRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => {
        this.roles = roles;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Loading roles failed', {disableTimeOut: true});
    });
  }

  canCreate() : boolean {
    return (this.institutionId > 0 && this.departmentId == 0 && this.canEdit);
  }

  getRoleDescriptions(department: Department) {
    return department.roles?.map(r => r.name).join(', ');
  }

  filterDepartments() {
    if(this.keyword.length >= 2) {
      this.filteredDepartments = this.departments.filter(a => a.name.toLowerCase().includes(this.keyword.toLowerCase()) || 
                                                            a.departmentType.name.toLowerCase().includes(this.keyword.toLowerCase()));
    }
    else if(this.keyword.length === 0)
      this.filteredDepartments = this.departments;
  }

  setDepartmentAsChanged(department: Department){
    if(!this.canEdit || this.departmentAsChanged?.id === department.id) return;

    this.resetSelectedRoles();
    department.roles.forEach((role) => 
      this.selectedRoles.push(role)
    );
    this.departmentAsChanged = JSON.parse(JSON.stringify(department)) ;
  }

  resetSelectedRoles() {
    this.selectedRoles.splice(0, this.selectedRoles.length);
  }

  updateDepartment(department: Department): void {
    department.roles = this.selectedRoles;
    this.departmentService.updateDepartment(department).subscribe(
      () => {
        this.departmentAsChanged = null;
        this.getDepartments();
        this.toastrService.success("Department updated");
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Department update failed', { disableTimeOut: true});
      }
    );
  }

  deleteDepartment(department: Department): void {
    this.departmentService.hasTransferredSessionToFHI(department.id).subscribe(
      (result) => {
        if (result) 
        {
          this.toastrService.error('The department has observations and cannot be deleted', 'Department deletion failed', { disableTimeOut: true });
        }
        else
        {
          this.departmentService.deleteDepartment(department.id).subscribe(
            () => {
              this.getDepartments();
              this.toastrService.success("Department deleted");
            },
            (error) => {
              this.toastrService.error(error.error.message, 'Department deletion failed', { disableTimeOut: true});
            }
          );
        }
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Department deletion failed', { disableTimeOut: true});
      });
  }

  canBeSaved(): boolean {
    if(this.departmentAsChanged?.name.length > 0 && this.departmentAsChanged?.departmentTypeId > 0 && this.selectedRoles?.length > 0)
      return true;
    else
      return false;
  }

  cancelEdit($event: Event) {
    $event.stopPropagation();
    $event.preventDefault();
    this.departmentAsChanged = null;
  }

  sort($event: IColumnSortedEvent) {
    let propertyOf: (x: Department) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: Department) => x.name;
        break;
      case "Departmenttype":
        propertyOf = (x: Department) => x.departmentType.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: Department, b: Department) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    this.filteredDepartments = this.filteredDepartments.sort(sortFunc);
  }

}
