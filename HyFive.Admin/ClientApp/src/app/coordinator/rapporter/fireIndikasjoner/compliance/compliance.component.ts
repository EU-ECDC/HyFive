import { Component, OnInit, OnDestroy, AfterViewChecked, ChangeDetectorRef, ViewChild, ElementRef, HostListener } from '@angular/core';
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

  @ViewChild('dropdownRef', { static: false }) dropdownRef: ElementRef;
  isDropdownFocused: boolean = false;


  user: LoggedInUser = null;
  private selectedRole: AuthorizedRole;

  fromYear: number = 2024;
  toYear: number = 2024;
  fromMonth: number = 1;
  toMonth: number = 1;
  fromQuarter: number = 1;
  toQuarter: number = 1;
  selectedRoles: Role[] = [];
  selectedInstitutionTypes: InstitutionType[] = [];
  selectedInstitutionType: number;
  selectedInstitutions: InstitutionReport[] = [];
  selectedInstitutionId: number;
  selectedDepartments: Department[] = [];
  selectedDepartmentTypes: DepartmentType[];
  interval: string = 'year';
  months: any [];
  transferredTo: number = 1;
  canSelectInstitution = true;
  intervalYearError: boolean = false;

  dropdownSettings: IDropdownSettings;
  
  showGraph = false;
  showGraphError = false;
  roles: Role[];
  departments: Department[];
  allDepartments: Department[];
  departmentTypes: DepartmentType[];
  institutionTypes: InstitutionType[];
  institutions: InstitutionReport[];
  allInstitutions: InstitutionReport[];

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

    this.selectedRole = this.authorizationService.getSelectedRole();
          
          if (this.selectedRole === AuthorizedRole.Coordinator) {
            this.loadDepartmentTypes();
            this.canSelectInstitution = false;
            this.selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
            this.loadCoordinatorInstitutionDepartments(this.selectedInstitutionId)
          }
          else if (this.selectedRole === AuthorizedRole.Administrator) {  
            this.loadAdminInstitutionsTypes();          
            this.institutionService.getInstitutions().subscribe(
              (institutions) => {
                this.institutions = institutions;
                this.allInstitutions = institutions;
              });
            }
    
  }

  resetDropdownFilters() {
    this.selectedInstitutionTypes = [];
    this.selectedInstitutions = [];
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
    this.selectedRoles = [];
  }

  resetDepartments() {
    this.departments = [];
    this.allDepartments = [];
    this.departmentTypes = []
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
  const clickedInside = this.dropdownRef?.nativeElement.contains(event.target);
  
  if (clickedInside) {
    this.isDropdownFocused = true;
  } else if (this.isDropdownFocused) {
    this.isDropdownFocused = false;
    this.selectInstitution();
  }
  }

  selectInstitution(): void {
    this.departments = [];
    this.departmentTypes = [];
    this.allDepartments = [];
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
    if (this.selectedInstitutions != null && this.selectedInstitutions?.length > 0) {
      var institutionIds = this.selectedInstitutions?.map(inst => inst.id);
      this.loadInstitutionsDepartments(institutionIds)
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

  loadAdminInstitutionsTypes() {
    this.institutionService.getInstitutionTypes().subscribe((result) => {
      this.institutionTypes = result,
      (error) => this.toastrService.error('An error occurred while loading institution types: ' + error?.message, '', { disableTimeOut: true })
    });
  }

  loadInstitutionsDepartments(institutionIds: number[]) {
  this.institutionService.getComplianceInstitutions(institutionIds).subscribe(institutions => {
    const allDepartments = institutions.reduce((all, inst) => {
      return all.concat(inst.departments);
    }, []);

    const uniqueDepartments = Array.from(
      new Map(allDepartments.map(dep => [dep.id, dep])).values()
    );

    const uniqueDepartmentTypes = Array.from(
      new Map(allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values()
    );

    this.departmentTypes = uniqueDepartmentTypes;
    this.departments = uniqueDepartments;
    this.allDepartments = uniqueDepartments;
  });
}

  filterInstitutionsByType() {
    this.selectedInstitutions = [];
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
    if (this.selectedInstitutionTypes?.length > 0) {
      this.institutions =  this.allInstitutions?.filter(item => this.selectedInstitutionTypes.some(si => si.id == item.institutionType.id));
    } else {
      this.institutions = this.allInstitutions;
    }
  }

  filterDepartmentsByType() {
    this.selectedDepartments = [];
    if (this.selectedDepartmentTypes?.length > 0) {
      console
      this.departments =  this.allDepartments.filter(item => this.selectedDepartmentTypes.some(sd => sd.id == item.departmentTypeId));
    } else {
      this.departments = this.allDepartments;
    }
  }

  validateIntervalYear() {
    if (this.fromYear > this.toYear) {
      this.intervalYearError = true;
    } else {
      this.intervalYearError = false;
    }
  }

  loadCoordinatorInstitutionDepartments(institutionId: number) {
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
