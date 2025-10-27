import { Component, OnInit, OnDestroy, AfterViewChecked, ChangeDetectorRef, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FhiDiagramOptions } from '@folkehelseinstituttet/angular-highcharts';
import { ToastrService } from 'ngx-toastr';
import { Department} from '../../../../models/api/Department';
import { Role } from '../../../../models/api/Role';
import { FacilityService } from '../../../../services/data/facility.service';
import { ReportService } from '../../../../services/data/report.service';
import { RoleService } from '../../../../services/data/role.service';
import { FacilityType } from 'src/app/models/api/FacilityType';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { FacilityReport } from 'src/app/models/api/FacilityReport';
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
  public selectedRole: AuthorizedRole;
  AuthorizedRole = AuthorizedRole;

  d = new Date();
  fromYear: number = this.d.getFullYear();
  toYear: number = this.d.getFullYear();
  fromMonth: number = 1;
  toMonth: number = 1;
  fromQuarter: number = 1;
  toQuarter: number = 1;
  showFromMonth: number = 1;
  showToMonth: number = 1;
  showFromYear: number = this.d.getFullYear();
  showToYear: number = this.d.getFullYear();
  showFromQuarter: number = 1;
  showToQuarter: number = 1;
  showSelectedFacilities: FacilityReport[] = [];
  selectedRoles: Role[] = [];
  selectedFacilityTypes: FacilityType[] = [];
  selectedFacilityType: number;
  selectedFacilities: FacilityReport[] = [];
  selectedFacilityId: number;
  selectedDepartments: Department[] = [];
  selectedDepartmentTypes: DepartmentType[];
  intervalsList = [
    {name: 'Year', value: 'year'},
    {name: 'Quarter', value: 'quarter'},
    {name: 'Month', value: 'month'}
  ];
  quartersList = [
    {name: '1', value: 1},
    {name: '2', value: 2},
    {name: '3', value: 3},
    {name: '4', value: 4}
  ];
  TransferredLists = [
    {name: 'All', value: 0},
    {name: 'Transferred to Admin', value: 1},
    {name: 'Not transferred to Admin', value: 2}
  ];
  interval: string = 'year';
  months: any [];
  transferredTo: number = 0;
  canSelectFacility = true;
  intervalYearError: boolean = false;
  showQuarterError: boolean = false;

  dropdownSettings: IDropdownSettings;
  
  showGraph = false;
  showGraphError = false;
  roles: Role[];
  departments: Department[];
  allDepartments: Department[];
  departmentTypes: DepartmentType[];
  facilityTypes: FacilityType[];
  facilities: FacilityReport[];
  allFacilities: FacilityReport[];
  facility: FacilityReport;

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
    private facilityService: FacilityService,
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
            this.canSelectFacility = false;
            this.selectedFacilityId = this.facilityService.getSelectedFacilityId();
            this.facilityService.getFacility(this.selectedFacilityId)
              .subscribe(
                (facility) => {
                  this.facility = facility;
                }
              )
            this.loadCoordinatorFacilityDepartments(this.selectedFacilityId)
          }
          else if (this.selectedRole === AuthorizedRole.Administrator) {  
            this.loadAdminFacilityTypes();          
            this.facilityService.getFacilities().subscribe(
              (facilities) => {
                this.facilities = facilities;
                this.allFacilities = facilities;
              });
            }
    
  }

  onChangeModelFacility() {

    if (this.selectedFacilities.length > 0) {

      if (this.departmentTypes.length > 0 && this.allDepartments.length > 0) {
        this.allDepartments = this.allDepartments.filter(dep => this.selectedFacilities.some(inst => inst.id == dep.facilityId));
        this.departments = this.allDepartments;
        this.departmentTypes = Array.from(
                                new Map(this.allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values());
      }
      this.selectedDepartments = [];
      this.selectedDepartmentTypes = [];

    } else {
      this.resetDepartments();
    }
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
    this.selectFacility();
  }
  }

  selectFacility(): void {
    this.departments = [];
    this.departmentTypes = [];
    this.allDepartments = [];
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
    if (this.selectedFacilities != null && this.selectedFacilities?.length > 0) {
      var facilityIds = this.selectedFacilities?.map(inst => inst.id);
      this.loadFacilitiesDepartments(facilityIds)
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

  loadAdminFacilityTypes() {
    this.facilityService.getFacilityTypes().subscribe((result) => {
      this.facilityTypes = result,
      (error) => this.toastrService.error('An error occurred while loading facility types: ' + error?.message, '', { disableTimeOut: true })
    });
  }

  loadFacilitiesDepartments(facilityIds: number[]) {
  this.facilityService.getComplianceFacilities(facilityIds).subscribe(facilities => {
    const allDepartments = facilities.reduce((all, inst) => {
      return all.concat(inst.departments);
    }, []);

    let uniqueDepartments = Array.from(
      new Map(allDepartments.map(dep => [dep.id, dep])).values()
    );

    const uniqueDepartmentTypes = Array.from(
      new Map(allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values()
    );
      uniqueDepartments = uniqueDepartments
                          .sort((a,b) => a.name.localeCompare(b.name, undefined, { sensitivity: "base" }));
    this.departmentTypes = uniqueDepartmentTypes;
    this.departments = uniqueDepartments;
    this.allDepartments = uniqueDepartments;
  });
}

  filterFacilitiesByType() {
    this.selectedFacilities = [];
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
    if (this.selectedFacilityTypes?.length > 0) {
      this.facilities =  this.allFacilities?.filter(item => this.selectedFacilityTypes.some(si => si.id == item.facilityType.id));
    } else {
      this.facilities = this.allFacilities;
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
    this.validateQuarters();
  }

  validateQuarters() {
    if (this.interval == 'quarter') {
      if (this.fromYear == this.toYear) {
        if (this.fromQuarter > this.toQuarter) {
          this.showQuarterError = true;
        } else {
          this.showQuarterError = false;
        }
      } else {
        this.showQuarterError = false;
      }
    }
  }

  onChangeInterval() {
    this.fromQuarter = 1;
    this.toQuarter = 1;
    this.showQuarterError = false;
  }

  loadCoordinatorFacilityDepartments(facilityId: number) {
    this.facilityService.getFacility(facilityId).subscribe(
      facility => {
        this.departments = facility.departments;
        this.allDepartments = this.departments;
        this.selectedFacilityTypes.push(facility.facilityType);
      })
  };

  loadRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  loadDepartments() {
    var facilityIds = this.selectedFacilities.map(inst => inst.id);
    this.facilityService.getDepartmentsByFacilities(facilityIds).subscribe(
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
      //var facilityId = this.facilityService.getSelectedFacilityId();
    this.showGraphError = false;

    const roleIds = this.selectedRoles?.map(role => role.id) ?? [];
    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    const facilityTypeIds = this.selectedFacilityTypes?.map(t => t.id) ?? [];
    const facilityIds = this.selectedFacilityId ? [this.selectedFacilityId] : this.selectedFacilities?.map(t => t.id) ?? [];
    const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];

    this.graphService.getComplianceForFiveIndications({
        facilityIds,
        facilityTypeIds,
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

        this.showFacilitiesAndPeriod();
        let percentageGraph = graphs[0];
        this.savePercentageChartOptions(percentageGraph);
        let antallGraf = graphs[1];
        this.saveNumberDiagramOptions(antallGraf);
        this.showGraph = true;
      },
      (error) => {
        this.showGraphError = true;
      }
    );
  }

  showFacilitiesAndPeriod() {
    this.showFromMonth = this.fromMonth;
    this.showToMonth = this.toMonth;
    this.showFromYear = this.fromYear;
    this.showToYear = this.toYear;
    this.showFromQuarter = this.fromQuarter;
    this.showToQuarter = this.toQuarter;
    this.showSelectedFacilities = this.selectedFacilities;
  }

  savePercentageChartOptions(graph: any) {
    this.percentageDiagramOptions = {
      title: graph.title,
      diagramTypeId: 'column',
      series: graph.graphDataList,
      openSource: false,
      units: [{
        id: 'percent',
        decimals: 1,
        label: 'Compliance (%)',
        symbol: '%',
        position: 'end',
        yAxis: 100
      }]
    };
  }

  saveNumberDiagramOptions(graph: any) {
    this.numberDiagramOptions = {
      title: graph.title,
      diagramTypeId: 'column',
      series: graph.graphDataList,
      openSource: false,
      units: [{
        id: 'number',
        label: 'Indications (N)',
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
