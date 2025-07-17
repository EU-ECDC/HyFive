import { Component, OnInit, OnDestroy, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { FhiDiagramOptions } from '@folkehelseinstituttet/angular-highcharts';
import { ToastrService } from 'ngx-toastr';
import { Department} from '../../../../models/api/Department';
import { Role } from '../../../../models/api/Role';
import { InstitutionService } from '../../../../services/data/institution.service';
import { ReportService } from '../../../../services/data/report.service';
import { RoleService } from '../../../../services/data/role.service';
import { InstitutionType } from 'src/app/models/api/InstitutionType';
import { Institution } from 'src/app/models/api/Institution';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { InstitutionReport } from 'src/app/models/api/InstitutionReport';
import { DepartmentType } from 'src/app/models/api/DepartmentType';
import { LoggedInUser } from 'src/app/models/api/LoggedInUser';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { DepartmentService } from 'src/app/services/data/department.service';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';

@Component({
  selector: 'app-compliance',
  templateUrl: './compliance.component.html',
  styleUrls: ['./showGraphErrorStyle.scss']
})
export class ComplianceComponent implements OnInit, OnDestroy, AfterViewChecked  {

  user: LoggedInUser = null;
  private selectedRole: AuthorizedRole;

  fromYear: number = 2024;
  toYear: number = 2024;
  fromMonth: number = 1;
  toMonth: number = 1;
  fromQuarter: number = 1;
  toQuarter: number = 1;
  selectedRoles: Role[];
  selectedInstitutionTypes: InstitutionType[];
  selectedInstitutionType: number;
  selectedInstitutions: InstitutionReport[];
  selectedInstitutionId: number;
  selectedDepartments: Department[];
  selectedDepartmentTypes: DepartmentType[];
  interval: string = 'year';
  months: any [];
  transferredTo: number = 1;
  canSelectInstitution = true;

  dropdownSettings: IDropdownSettings;
  
  showGraph = false;
  showGraphError = false;
  roles: Role[];
  departments: Department[];
  departmentTypes: DepartmentType[];
  institutionTypes: InstitutionType[];
  institutions: InstitutionReport[];

  percentageDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };

  numberDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };

  
  
  constructor(
    public authorizationService: AuthorizationService,
    private graphService: ReportService,
    private institutionService: InstitutionService,
    private departmentService: DepartmentService,
    private roleService: RoleService,
    private toastrService: ToastrService,
    private cdref: ChangeDetectorRef) { }

  ngOnInit(): void {

    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    },
      (error) => (this.toastrService.error("An error occurred while loading user: " + error?.message ? error.message : error, '', {disableTimeOut: true}))
    );
    this.dropdownSettings = {
      singleSelection: false,
      idField: 'id',
      textField: 'name',
      selectAllText: 'Select all',
      unSelectAllText: 'Select all',
      itemsShowLimit: 3
    };

    this.months = this.initMonths();
    this.loadRoles();
    this.loadInstitutionsTypes();
    this.loadDepartmentTypes();

    this.selectedRole = this.authorizationService.getSelectedRole();
          
          if (this.selectedRole === AuthorizedRole.Coordinator) {
            this.canSelectInstitution = false;
            this.selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
            this.loadInstitution(this.selectedInstitutionId)
          }
          else if (this.selectedRole === AuthorizedRole.Administrator) {            
            this.institutionService.getInstitutions().subscribe(
              (institutions) => {
                this.institutions = institutions;
              });
            }
    
  }

  selectInstitution(): void {
    this.departments = null;
    this.selectedDepartments = null;
    if (this.selectedInstitutions != null) {
      var institutionIds = this.selectedInstitutions.map(inst => inst.id);
      this.loadInstitutions(institutionIds)
    }
  };

    ngAfterViewChecked() {
    const alert = document.querySelector('.alert.alert-warning');
    if (alert) {
      // (alert as HTMLElement).style.visibility = 'hidden';
      this.showGraph = false;
      this.showGraphError = true;
      this.cdref.detectChanges();
    }
  }

  ngOnDestroy(): void {
  }

  loadInstitutionsTypes() {
    this.institutionService.getInstitutionTypes().subscribe((result) => {
      this.institutionTypes = result,
      (error) => this.toastrService.error('An error occurred while loading institution types: ' + error?.message, '', { disableTimeOut: true })
    });
  }

  loadInstitutions(institutionIds: number[]) {
  this.institutionService.getComplianceInstitutions(institutionIds).subscribe(institutions => {
    const allDepartments = institutions.reduce((all, inst) => {
      return all.concat(inst.departments);
    }, []);

    const uniqueDepartments = Array.from(
      new Map(allDepartments.map(dep => [dep.id, dep])).values()
    );

    this.departments = uniqueDepartments;
  });
}

  loadInstitution(institutionId: number) {
    this.institutionService.getInstitution(institutionId).subscribe(
      institution => {
        this.departments = institution.departments;
        this.selectedInstitutionTypes.push(institution.institutionType);
      })
  };

  loadRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  loadDepartments() {
    var institutionIds = this.selectedInstitutions.map(inst => inst.id);
    this.institutionService.getDepartmentsByInstitutions(institutionIds).subscribe(
      (departments) => this.departments = departments,
      (error) => this.toastrService.error('An error occurred while loading departments: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  loadDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = departmentTypes;
      },
      (error) => {
        this.toastrService.error(error.error.message, 'Loading departmentType failed', {disableTimeOut: true});
    });
  }

  getComplianceForFiveIndications() {
    //if(this.selectedRole === AuthorizedRole.Coordinator)
      //var institutionId = this.institutionService.getSelectedInstitutionId();
    this.showGraphError = false;

    const roleIds = this.selectedRoles?.map(role => role.id) ?? [];
    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    const institutionTypeIds = this.selectedInstitutionTypes?.map(t => t.id) ?? [];
    const institutionIds = this.selectedInstitutionId ? [this.selectedInstitutionId] : this.selectedInstitutions?.map(t => t.id) ?? [];
    const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];

    this.graphService.getComplianceForFiveIndications({
        institutionIds,
        institutionTypeIds,
        interval: this.interval,
        fromMonth: this.fromMonth,
        fromYear: this.fromYear,
        fromQuarter: this.fromQuarter,
        toMonth: this.toMonth,
        toYear: this.toYear,       
        toQuarter: this.toQuarter,
        roleIds, 
        departmentIds,
        departmentTypeIds,
        transferredTo: this.transferredTo}).subscribe(
      (graphs) => {

        let percentageGraph = graphs[0];
        this.savePercentageChartOptions(percentageGraph);
        let antallGraf = graphs[1];
        this.saveNumberDiagramOptions(antallGraf);
        this.showGraph = true;
      },
      (error) => this.toastrService.error('Error in generating graph data: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  savePercentageChartOptions(graph: any) {
    this.percentageDiagramOptions = {
      title: graph.title,
      diagramTypeId: 'line',
      series: graph.graphDataList,
      openSource: false,
      units: [{
        id: 'percent',
        decimals: 1,
        label: 'Compliance (%)',
        symbol: '%',
        position: 'end'
      }]
    };
  }

  saveNumberDiagramOptions(graph: any) {
    this.numberDiagramOptions = {
      title: graph.title,
      diagramTypeId: 'line',
      series: graph.graphDataList,
      openSource: false,
      units: [{
        id: 'number',
        label: 'Number',
        position: 'end'
      }]
    };
  }

  initMonths() {
    return [
      { value: 1, description: "January" },
      { value: 2, description: "February" },
      { value: 3, description: "March" },
      { value: 4, description: "April" },
      { value: 5, description: "May" },
      { value: 6, description: "June" },
      { value: 7, description: "July" },
      { value: 8, description: "August" },
      { value: 9, description: "September" },
      { value: 10, description: "October" },
      { value: 11, description: "November" },
      { value: 12, description: "December" }];
  }
}
