import { Component, Input, OnInit } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { DepartmentService } from 'src/app/services/data/department.service';
import { Role } from 'src/app/models/api/Role';
import { RoleService } from 'src/app/services/data/role.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from 'src/app/services/events/key-event.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { TranslateService } from '@ngx-translate/core';
import { SortHelper } from 'src/app/utils/sort-helper';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';
import { UpdateDepartmentRequest } from 'src/app/models/api/UpdateDepartmentRequest';
@Component({
selector: 'app-editing-of-departments',
  templateUrl: './editing-of-departments.component.html'
})
export class EditingDepartmentsComponent implements OnInit {
  
  @Input() facilityId: number;

  departments: OrganisationUnit[] = [];
  filteredDepartments: OrganisationUnit[] = [];
  departmentId = 0;
  departmentAsChanged: OrganisationUnit;
  departmentTypes: OrganisationUnitType[];
  roles: Role[];
  selectedRoles: Role[] = [];
  canEdit: boolean;
  facilityName: string;
  keyword: string;
  loading: boolean = false;
  dropdownSettings: IDropdownSettings;
  showCreateDepartmentForm: boolean = false;

  constructor(private readonly facilityService: FacilityService,
            private readonly authorizationService: AuthorizationService,
            private readonly departmentService: DepartmentService,
            private readonly roleService: RoleService,
            private readonly toastrService: ToastrService,
            private readonly keyEventService: KeyEventService,
            private readonly translate: TranslateService) { }

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
      selectAllText: this.translate.instant('Select all'),
      unSelectAllText: this.translate.instant('Select all'),
      noDataAvailablePlaceholderText: this.translate.instant('No data available'),
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
    let selectedFacilityId = this.facilityId ?? this.facilityService.getSelectedFacilityId();
    this.showCreateDepartmentForm = false;
    this.facilityService.getFacility(selectedFacilityId).subscribe(
      (facility) => {
        this.facilityName = facility.name;
        this.facilityId = facility.id;
        this.departments = facility.children;
        this.filteredDepartments = this.departments;
      },
      (error) => {
        this.toastrService.error(error.error.message, this.translate.instant('Loading facilities failed'), {disableTimeOut: true});
    });
  }

  getDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = departmentTypes;
      },
      (error) => {
        this.toastrService.error(error.error.message, this.translate.instant('Loading departmentType failed'), {disableTimeOut: true});
    });
  }

  getRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => {
        this.roles = roles;
      },
      (error) => {
        this.toastrService.error(error.error.message, this.translate.instant('Loading roles failed'), {disableTimeOut: true});
    });
  }

  canCreate() : boolean {
    return (this.facilityId > 0 && this.departmentId == 0 && this.canEdit);
  }

  getRoleDescriptions(department: OrganisationUnit) {
    return department.roles?.map(r => r.name).join(', ');
  }

  filterDepartments() {
    if(this.keyword.length >= 2) {
      this.filteredDepartments = this.departments.filter(a => a.name.toLowerCase().includes(this.keyword.toLowerCase()) || 
                                                            a.type.name.toLowerCase().includes(this.keyword.toLowerCase()));
    }
    else if(this.keyword.length === 0)
      this.filteredDepartments = this.departments;
  }

  setDepartmentAsChanged(department: OrganisationUnit){
    if(!this.canEdit || this.departmentAsChanged?.id === department.id) return;

    this.resetSelectedRoles();
    for (const role of department.roles) {
      this.selectedRoles.push(role);
    }
    this.departmentAsChanged = structuredClone(department);
  }

  resetSelectedRoles() {
    this.selectedRoles.splice(0, this.selectedRoles.length);
  }

  updateDepartment(department: OrganisationUnit): void {
    department.name = this.sanitizeDepartmentName(department.name);

    if (!department.name) {
      return;
    }
    department.roles = this.selectedRoles;
    const updateDepartment: UpdateDepartmentRequest = {
      id: department.id,
      facilityId: this.facilityId,
      name: department.name,
      departmentTypeId: department.typeId,
      roles: department.roles
    }
    this.departmentService.updateDepartment(updateDepartment).subscribe(
      () => {
        this.departmentAsChanged = null;
        this.getDepartments();
        this.toastrService.success(this.translate.instant("Department updated"));
      },
      (error) => {
        this.toastrService.error(error.error.message, this.translate.instant('Department update failed'), { disableTimeOut: true});
      }
    );
  }

  deleteDepartment(department: OrganisationUnit): void {
    this.departmentService.hasTransferredSessionToFHI(department.id).subscribe(
      (result) => {
        if (result) 
        {
          this.toastrService.error(this.translate.instant('The department has observations and cannot be deleted'), this.translate.instant('Department deletion failed'), { disableTimeOut: true });
        }
        else
        {
          this.departmentService.deleteDepartment(department.id).subscribe(
            () => {
              this.getDepartments();
              this.toastrService.success(this.translate.instant("Department was deleted"));
            },
            (error) => {
              this.toastrService.error(error.error.message, this.translate.instant('Department deletion failed'), { disableTimeOut: true});
            }
          );
        }
      },
      (error) => {
        this.toastrService.error(error.error.message, this.translate.instant('Department deletion failed'), { disableTimeOut: true});
      });
  }

  canBeSaved(): boolean {
    if(this.departmentAsChanged?.name.length > 0
      && !this.filteredDepartments.filter(dep => dep.id !== this.departmentAsChanged.id).some(dep => dep.name == this.departmentAsChanged.name)
      && this.departmentAsChanged?.type.id > 0 
      && this.selectedRoles?.length > 0)
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
    const userSortConfig = {
      [this.translate.instant("Name")]: (x: OrganisationUnit) => x.name,
      [this.translate.instant("Department Type")]: (x: OrganisationUnit) => x.type.name,
    };

    this.filteredDepartments = SortHelper.sort(this.filteredDepartments, $event, userSortConfig);
  }

  omitSpecialChar(event) {   
    const char = event.key;

    // Allow letters, numbers, spaces, basic punctuation
    const allowed = /^[a-zA-Z0-9\s.,'-]$/;

    if (!allowed.test(char)) {
      event.preventDefault();
      return false;
  }

  return true;
  }

    toggleShowForm() {
    this.showCreateDepartmentForm = !this.showCreateDepartmentForm;
  }

  sanitizeDepartmentName(value: string): string {
    if (!value) {
      return value;
    }

    // Remove emojis, pictographs, symbols (covers paste & unicode images)
    const noEmojis = value.replaceAll(
      /[\p{Extended_Pictographic}\p{Emoji_Presentation}\p{Symbol}]/gu,
      ''
    );

    return noEmojis.trim();
  }

}
