import { Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { IDropdownSettings } from 'ng-multiselect-dropdown';
import { ToastrService } from 'ngx-toastr';
import { Observable, take } from 'rxjs';
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { Department} from 'src/app/models/api/Department';
import { DownloadExcelModel } from 'src/app/models/api/downloadExcelModel';
import { FacilityReport } from 'src/app/models/api/FacilityReport';
import { SessionType } from 'src/app/models/api/SessionType';
import { FacilityService } from 'src/app/services/data/facility.service';
import { ReportService } from 'src/app/services/data/report.service';
import { DateMomentHelper } from 'src/app/utils/date-moment-helper';
import { DownloadComplianceFacilitiesHelper } from 'src/app/utils/download-compliance-facilities-helper';
import { DownloadFileHelper } from 'src/app/utils/download-file-helper';

@Component({
  selector: 'app-compliance-pdf',
  templateUrl: './compliance-pdf.component.html'
})
export class CompliancePdfComponent implements OnInit {

  @Input() sessionType: SessionType;

  selectedFacilityId: number;
  // selectedFacilityTypes: FacilityType[] = [];
  selectedFacilities: FacilityReport[] = [];
  // selectedDepartmentTypes: DepartmentType[];
  selectedDepartments: Department[] = [];
  fromDate: Date = null;
  toDate: Date = null;
  facility: FacilityReport;
  // facilityTypes: FacilityType[];
  departments: Department[] = [];
  // departmentTypes: DepartmentType[] = [];
  allDepartments: Department[] = [];
  facilities: FacilityReport[] = [];
  allFacilities: FacilityReport[] = [];
  canSelectFacility = false;
  storedReport = false;
  createFacilityReport = false;

  private selectedRole: AuthorizedRole;

  dropdownSettings: IDropdownSettings;

  constructor(
    private readonly facilityService: FacilityService,
    private readonly reportService: ReportService,
    private readonly toastrService: ToastrService,
    private readonly authorizationService: AuthorizationService,
    private readonly translate: TranslateService) { }

  @ViewChild('dropdownRef', { static: false }) dropdownRef: ElementRef;
  isDropdownFocused: boolean = false;
  
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

  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();
    
    if (this.selectedRole === AuthorizedRole.Coordinator) {
      // this.loadDepartmentTypes();
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
      this.canSelectFacility = true;
      // this.loadAdminFacilityTypes();
      this.facilityService.getFacilities().subscribe(
        (facilities) => {
          this.facilities = facilities;
          this.allFacilities = facilities;
        });
    }

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
  }

  // filterFacilitiesByType() {
  //   this.selectedFacilities = [];
  //   this.selectedDepartments = [];
  //   this.selectedDepartmentTypes = [];
  //   if (this.selectedFacilityTypes?.length > 0) {
  //     this.facilities =  this.allFacilities?.filter(item => this.selectedFacilityTypes.some(si => si.id == item.facilityType.id));
  //   } else {
  //     this.facilities = this.allFacilities;
  //   }
  // }

  // filterDepartmentsByType() {
  //   this.selectedDepartments = [];
  //   if (this.selectedDepartmentTypes?.length > 0) {
  //     console
  //     this.departments =  this.allDepartments.filter(item => this.selectedDepartmentTypes.some(sd => sd.id == item.departmentTypeId));
  //   } else {
  //     this.departments = this.allDepartments;
  //   }
  // }

  onChangeModelFacility() {
    if (this.selectedFacilities.length > 0) {
      if (
        // this.departmentTypes.length > 0 && 
        this.allDepartments.length > 0) {
        this.allDepartments = this.allDepartments.filter(dep => this.selectedFacilities.some(inst => inst.id == dep.facilityId));
        this.departments = this.allDepartments;
        // this.departmentTypes = Array.from(
        //                         new Map(this.allDepartments.map(dep => [dep.departmentType.id, dep.departmentType])).values());
      }
      this.selectedDepartments = [];
      // this.selectedDepartmentTypes = [];
    } else {
      this.resetDepartments();
    }
  }

  resetDepartments() {
    this.departments = [];
    this.allDepartments = [];
    // this.departmentTypes = [];
    this.selectedDepartments = [];
    // this.selectedDepartmentTypes = [];
  }
      
  selectFacility(): void {
    this.departments = [];
    this.allDepartments = [];
    // this.selectedDepartmentTypes = [];
    this.selectedDepartments = [];
    if (this.selectedFacilities != null && this.selectedFacilities?.length > 0) {
      let facilityIds = this.selectedFacilities?.map(inst => inst.id);
      this.loadFacilitiesDepartments(facilityIds)
    }
  };

  reset(): void {
    this.selectedDepartments = [];
    this.selectedFacilities = [];
    // this.selectedDepartmentTypes = [];
    this.departments = [];
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedFacilityId = null;
    }
  }

  canCreateReport() {
    return (this.selectedDepartments.length > 0 && this.fromDate && this.toDate);
  }

  saveReport() {
    this.toastrService.clear();

    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    // const facilityTypeIds = this.selectedFacilityTypes?.map(t => t.id) ?? [];
    const facilityIds = this.selectedFacilityId ? [this.selectedFacilityId] : this.selectedFacilities?.map(t => t.id) ?? [];
    // const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];

    this.reportService.reportForSessionTypeHasData(
      {
      sessionType: this.sessionType,
      // facilityTypeIds,
      facilityIds,
      // departmentTypeIds,
      departmentIds,
      fromDate: DateMomentHelper.dateTimeToDate(this.fromDate, "YYYY-MM-DD"), 
      toDate: DateMomentHelper.dateTimeToDate(this.toDate, "YYYY-MM-DD"),
      roleId: this.selectedRole
      }).subscribe(
        reportHasData => {
          if (reportHasData) {
            this.storedReport = true;
            this.downloadPdf().subscribe(() => {
              this.storedReport = false
            },
              (error) => {
                this.storedReport = false
                this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant('Error while downloading report'), { disableTimeOut: true });
              });
          } else {
            this.toastrService.info(this.translate.instant('There are no observations for selected values'), '');
          }
        })
  }

  private downloadPdf(): Observable<any> {
    let url = '/api/v1/report/';

    if (this.sessionType == SessionType.FiveIndications) {
      url += 'fiveindications';
    } else if (this.sessionType == SessionType.HandJewelry) {
      url += 'handjewelry';
    }

    url += '/department/pdf/';
    const payload: DownloadExcelModel = {
      departmentIds:  this.selectedDepartments?.map(dep => dep.id) ?? [],
      facilityIds: this.selectedFacilityId ? [this.selectedFacilityId] : this.selectedFacilities?.map(t => t.id) ?? [],
      fromDate: DateMomentHelper.dateTimeToDate(this.fromDate, "YYYY-MM-DD"),
      toDate: DateMomentHelper.dateTimeToDate(this.toDate, "YYYY-MM-DD"),
      role: this.selectedRole
    } 

    return DownloadFileHelper.downloadFile(url, 'application/pdf, */*', payload);
  }

  // loadAdminFacilityTypes() {
  //   this.facilityService.getFacilityTypes().subscribe((result) => {
  //     this.facilityTypes = result,
  //     (error) => this.toastrService.error('An error occurred while loading facility types: ' + error?.message, '', { disableTimeOut: true })
  //   });
  // }

  // loadDepartmentTypes() {
  //   this.departmentService.getDepartmentTypes().subscribe(
  //     (departmentTypes) => {
  //       this.departmentTypes = departmentTypes;
  //     },
  //     (error) => {
  //       this.toastrService.error(error.error.message, 'Loading departmentType failed', {disableTimeOut: true});
  //   });
  // }

  loadCoordinatorFacilityDepartments(facilityId: number) {
    this.facilityService.getFacility(facilityId).subscribe(
      facility => {
        this.departments = facility.departments;
        this.allDepartments = this.departments;
        // this.selectedFacilityTypes.push(facility.facilityType
      })
  };

  loadFacilitiesDepartments(facilityIds: number[]) {
  this.facilityService.getComplianceFacilities(facilityIds).subscribe(facilities => {
    
      this.departments = DownloadComplianceFacilitiesHelper.handleUniqueDepartments(facilities);
      this.allDepartments = this.departments;
    });
  }
}
