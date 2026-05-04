import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { DepartmentService } from '../../../services/data/department.service';
import { KeyEventService } from '../../../services/events/key-event.service';
import { TranslateService } from '@ngx-translate/core';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';

@Component({
  selector: 'app-editing-of-departmentstype',
  templateUrl: './editing-of-departmentstype.component.html'
})
export class EditingOfDepartmentTypesComponent implements OnInit, OnDestroy {

  departmentTypes: OrganisationUnitType[] = [];
  newDepartmentType: OrganisationUnitType = this.emptyRequest();
  departmentTypeAsChanged: OrganisationUnitType = null;

  constructor(
    private readonly departmentService: DepartmentService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService

  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadDepartmentTypes();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (result) => this.departmentTypes = result,
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Department Types:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): OrganisationUnitType {
    return {
      id: 0,
      code: null,
      name: null,
      description: null
    };
  }

  createDepartmentType() {
    if (!this.newDepartmentType.code.startsWith("D_")) {
      this.newDepartmentType = {...this.newDepartmentType, code: 'D_' + this.newDepartmentType.code};
    }
    this.departmentService.createDepartmentType(this.newDepartmentType).subscribe(
      (departmenttype) => this.toastrService.success(this.translate.instant('Department Type with name') + ' ' + `${departmenttype.name}` + ' ' +  this.translate.instant('was created')),
      error => this.toastrService.error(this.translate.instant('An error occurred while creating Department Type') + ' ' +  `${this.newDepartmentType.name}.` + this.translate.instant('Error: ') + `${error.error.message}`, '', { disableTimeOut: true}),
      () => { this.newDepartmentType = this.emptyRequest(); this.loadDepartmentTypes(); }
    );
  }

  selectedDepartmentType(departmenttype: OrganisationUnitType): void {
    if (this.departmentTypeAsChanged?.id == departmenttype.id) return;
    this.departmentTypeAsChanged = structuredClone(departmenttype);
  }

  updateDepartmentType(departmenttype: OrganisationUnitType): void {
    if(!departmenttype.code.startsWith("D_")) {
        departmenttype = {...departmenttype, code: 'D_' + departmenttype.code};
    }
    this.departmentService.updateDepartmentType(departmenttype).subscribe(
      (result) => {
        this.toastrService.success(this.translate.instant('Department Type was updated'));
        this.loadDepartmentTypes();
      },
      (error) => {
        this.toastrService.error(this.translate.instant('An error occurred while updating department type:') + ' ' + error?.error.message, '', { disableTimeOut: true});
      },
      () => this.departmentTypeAsChanged = null
    );
  }
  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.departmentTypeAsChanged = null;
  }
}
