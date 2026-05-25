import { Component, OnInit, AfterViewChecked, ChangeDetectorRef, ViewChild, ElementRef, HostListener } from '@angular/core';
import { FhiDiagramOptions } from '@folkehelseinstituttet/angular-highcharts';
import { ToastrService } from 'ngx-toastr';
import { Role } from '../../../../models/api/Role';
import { FacilityService } from '../../../../services/data/facility.service';
import { ReportService } from '../../../../services/data/report.service';
import { RoleService } from '../../../../services/data/role.service';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { LoggedInUser } from 'src/app/models/api/LoggedinUser';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { DepartmentService } from 'src/app/services/data/department.service';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';
import { DownloadComplianceFacilitiesHelper } from 'src/app/utils/download-compliance-facilities-helper';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { OrganisationUnitType } from 'src/app/models/api/OrganisationUnitType';
import { FacilityReport } from 'src/app/models/api/FacilityReport';

@Component({
  selector: 'app-compliance',
  templateUrl: './compliance.component.html',
  styleUrls: ['./showGraphErrorStyle.scss']
})
export class ComplianceComponent implements OnInit, AfterViewChecked  {

  @ViewChild('dropdownRef', { static: false }) dropdownRef: ElementRef;
  isDropdownFocused: boolean = false;


  user: LoggedInUser = null;
  public selectedRole: AuthorizedRole;
  AuthorizedRole = AuthorizedRole;

  d = new Date();
  fromYear: number = this.d.getFullYear();
  toYear: number = this.d.getFullYear();
  fromMonth: number = 1;
  toMonth: number = 12;
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
  selectedFacilityTypes: OrganisationUnitType[] = [];
  selectedFacilityType: number;
  selectedFacilities: FacilityReport[] = [];
  selectedFacilityId: number;
  selectedDepartments: OrganisationUnit[] = [];
  selectedDepartmentTypes: OrganisationUnitType[];
  selectedUnits: OrganisationUnit[] = [];
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
    {name: 'Transferred to Administrator', value: 1},
    {name: 'Not transferred to Administrator', value: 2}
  ];
  interval: string = 'year';
  months: any [];
  transferredTo: number = 0;
  canSelectFacility = true;
  intervalYearError: boolean = false;
  showQuarterError: boolean = false;
  fromYearDigitsError: boolean = false;
  toYearDigitsError: boolean = false;

  dropdownSettings: IDropdownSettings;
  
  showGraph = false;
  showGraphError = false;
  roles: Role[];
  departments: OrganisationUnit[];
  units: OrganisationUnit[];
  allDepartments: OrganisationUnit[];
  departmentTypes: OrganisationUnitType[];
  facilityTypes: OrganisationUnitType[];
  facilities: FacilityReport[];
  allFacilities: FacilityReport[];
  facility: OrganisationUnit;

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
    private readonly graphService: ReportService,
    private readonly facilityService: FacilityService,
    private readonly departmentService: DepartmentService,
    private readonly roleService: RoleService,
    private readonly toastrService: ToastrService,
    private readonly cdref: ChangeDetectorRef,
    private readonly translate: TranslateService) { }

  ngOnInit(): void {

    this.translate.get(this.intervalsList.map(item => item.name)).pipe(take(1)).subscribe(_res => {
      this.intervalsList = this.intervalsList.map( interval => {
        return {
          value: interval.value,
          name: this.translate.instant(interval.name)
        }
      });
    });

    this.translate.get(this.TransferredLists.map(item => item.name)).pipe(take(1)).subscribe(_res => {
      this.TransferredLists = this.TransferredLists.map( transferredItem => {
        return {
          value: transferredItem.value,
          name: this.translate.instant(transferredItem.name)
        }
      })
    });

    this.translate.get("Select all").pipe(take(1)).subscribe(_res => {
      this.dropdownSettings = {
        singleSelection: false,
        idField: 'id',
        textField: 'name',
        selectAllText: this.translate.instant('Select all'),
        unSelectAllText: this.translate.instant('Select all'),
        noDataAvailablePlaceholderText: this.translate.instant('No data available'),
        itemsShowLimit: 3
      };
    });

    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    },
      (error) => (this.toastrService.error(this.translate.instant("An error occurred while loading user:") + ' ' + error?.error.message ? error.error.message : error, '', {disableTimeOut: true}))
    );

    this.months = this.initMonths();
    this.translate.get(this.months.map(item => item.description)).pipe(take(1)).subscribe(_res => {
      this.months = this.months.map(month => {
        return {
          value: month.value,
          description: this.translate.instant(month.description)
        }
      })
    });
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
        this.allDepartments = this.allDepartments.filter(dep => this.selectedFacilities.some(inst => inst.id == dep.parentId));
        this.departments = this.allDepartments;
        this.departmentTypes = Array.from(
                                new Map(this.allDepartments.map(dep => [dep.type.id, dep.type])).values());
      }
      this.selectedDepartments = [];
      this.selectedDepartmentTypes = [];
      this.units = [];
      this.selectedUnits = [];

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
    this.units = [];
    this.selectedUnits = [];
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
    this.units = [];
    this.selectedUnits = [];
    if (this.selectedFacilities != null && this.selectedFacilities?.length > 0) {
      let facilityIds = this.selectedFacilities?.map(inst => inst.id);
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

  loadAdminFacilityTypes() {
    this.facilityService.getFacilityTypes().subscribe((result) => {
      this.facilityTypes = result,
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading facility types:') + ' ' + error?.error.message, '', { disableTimeOut: true })
    });
  }

  loadFacilitiesDepartments(facilityIds: number[]) {
  this.facilityService.getComplianceFacilities(facilityIds).subscribe(facilities => {

    this.departmentTypes = DownloadComplianceFacilitiesHelper.handleUniqueDepartmentTypes(facilities);
    this.departments = DownloadComplianceFacilitiesHelper.handleUniqueDepartments(facilities);
    this.allDepartments = this.departments;
  });
}

  filterFacilitiesByType() {
    this.selectedFacilities = [];
    this.selectedDepartments = [];
    this.selectedDepartmentTypes = [];
    if (this.selectedFacilityTypes?.length > 0) {
      this.facilities =  this.allFacilities?.filter(item => this.selectedFacilityTypes.some(si => si.id == item.type.id));
    } else {
      this.facilities = this.allFacilities;
    }
  }

  filterDepartmentsByType() {
    this.selectedDepartments = [];
    if (this.selectedDepartmentTypes?.length > 0) {
      this.departments =  this.allDepartments.filter(item => this.selectedDepartmentTypes.some(sd => sd.id == item.type.id));
    } else {
      this.departments = this.allDepartments;
    }
  }

    onDepartmentSelect() {
    if (this.selectedDepartments.length > 0) {
      const tempUnits: Set<OrganisationUnit> = new Set();
      const selectedDepartmentsidsList: Set<number> = new Set(this.selectedDepartments.map(obj => obj.id));
      this.departments.filter(dep => selectedDepartmentsidsList.has(dep.id))?.forEach(dep => {
        dep?.children?.forEach(unit => {
            tempUnits.add(unit);
        })
      });
      this.units = [...tempUnits];
      this.selectedUnits = this.selectedUnits.filter(selectedU => new Set(this.units.map(obj => obj.id)).has(selectedU.id));
    } else {
      this.units = [];
      this.selectedUnits = [];
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

  validateFromYearDigits() {
    if (this.fromYear <= 1900) {
      this.fromYearDigitsError = true;
    } else {
      this.fromYearDigitsError = false;
    }
  }

  validateToYearDigits() {
    if (this.toYear <= 1900) {
      this.toYearDigitsError = true;
    } else {
      this.toYearDigitsError = false;
    }
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
        this.departments = facility.children;
        this.allDepartments = this.departments;
        this.selectedFacilityTypes.push(facility.type);
      })
  };

  loadRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Roles:') + ' ' + error?.error.message, '', { disableTimeOut: true })
    );
  }

  loadDepartments() {
    let facilityIds = this.selectedFacilities.map(inst => inst.id);
    this.facilityService.getDepartmentsByFacilities(facilityIds).subscribe(
      (departments) => this.departments = departments,
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading departments:') + ' ' + error?.error.message, '', { disableTimeOut: true })
    );
  }

  loadDepartmentTypes() {
    this.departmentService.getDepartmentTypes().subscribe(
      (departmentTypes) => {
        this.departmentTypes = departmentTypes;
      },
      (error) => {
        this.toastrService.error(error.error.message, this.translate.instant('Loading Department Types failed'), {disableTimeOut: true});
    });
  }

  getComplianceForHandHygiene() {
    //if(this.selectedRole === AuthorizedRole.Coordinator)
      //let facilityId = this.facilityService.getSelectedFacilityId();
    this.showGraphError = false;

    const roleIds = this.selectedRoles?.map(role => role.id) ?? [];
    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    const facilityTypeIds = this.selectedFacilityTypes?.map(t => t.id) ?? [];
    const facilityIds = this.selectedFacilityId ? [this.selectedFacilityId] : this.selectedFacilities?.map(t => t.id) ?? [];
    const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];
    const unitIds = this.selectedUnits?.map(u => u.id) ?? [];

    this.graphService.getComplianceForHandHygiene({
        facilityIds,
        facilityTypeIds,
        interval: this.interval,
        fromMonth: this.fromMonth,
        fromYear: this.fromYear,
        fromQuarter: this.fromQuarter,
        toMonth: this.toMonth,
        toYear: this.toYear,       
        toQuarter: this.toQuarter,
        role: this.selectedRole,
        roleIds,
        departmentIds,
        departmentTypeIds,
        unitIds,
        transferredTo: this.transferredTo}).subscribe(
      (graphs) => {

        if (graphs.length > 0) {
          this.showFacilitiesAndPeriod();
          let percentageGraph = graphs[0];
          this.savePercentageChartOptions(percentageGraph);
          let antallGraf = graphs[1];
          this.saveNumberDiagramOptions(antallGraf);
          this.showGraph = true;
        }
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

  disableButtonGrpaph() {
    if (this.fromYearDigitsError || this.toYearDigitsError || this.intervalYearError || this.showQuarterError) {
      return true;
    } else {
      return false;
    }
  }
}
