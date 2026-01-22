import { Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
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
import { SessionTypeReportUrlMapper } from 'src/app/utils/sessionstype-report-url-mapper';
import { SessionTypes } from 'src/app/utils/sessionTypes';

@Component({
  selector: 'app-download-excel',
  templateUrl: './download-excel.component.html',
  styleUrls: ['download-excel.component.scss']
})

export class DownloadExcelComponent implements OnInit {
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
      this.selectedFacilityId = this.facilityService.getSelectedFacilityId();
      this.getFacility(this.selectedFacilityId);
      this.loadCoordinatorFacilityDepartments(this.selectedFacilityId)
    }
    else if (this.selectedRole === AuthorizedRole.Administrator) {
      this.canSelectFacility = true;

      this.facilityService.getFacilities().subscribe(
        (facilities) => {
          this.facilities = facilities;
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

  sessionTypes = [...SessionTypes.GetSessionTypes()];

  selectedSessiontype: SessionType = null;
  fromDate: Date = null;
  toDate: Date = null;
  
  departments: Department[];
  facilities: FacilityReport[] = [];
  canSelectFacility = false;
  storedReport = false;
  selectedFacilityId: number;
  selectedFacilities: FacilityReport[] = [];
  // selectedFacilityTypes: FacilityType[] = [];
  // selectedDepartmentTypes: DepartmentType[];
  selectedDepartments: Department[] = [];
  // facilityTypes: FacilityType[];
  // departmentTypes: DepartmentType[] = [];
  allDepartments: Department[] = [];
  allFacilities: FacilityReport[] = [];
  createFacilityReport = false;

  private selectedRole: AuthorizedRole;
  dropdownSettings: IDropdownSettings;

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

  loadFacilitiesDepartments(facilityIds: number[]) {
    this.facilityService.getComplianceFacilities(facilityIds).subscribe(facilities => {

      this.departments = DownloadComplianceFacilitiesHelper.handleUniqueDepartments(facilities);
      this.allDepartments = this.departments;
    });
  }

  reset(): void {
    this.selectedFacilities = [];
    this.selectedDepartments = [];
    // this.selectedDepartmentTypes = [];
    this.selectedSessiontype = null;
    this.createFacilityReport = false;
    this.fromDate = null;
    this.toDate = null;
    this.toastrService.clear();

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedFacilityId = null;
    }
  }

  selectCreateFacilityReport() {
    this.selectedDepartments = [];
  }

  canCreateReport() {
    // return (((
    //   this.selectedFacilityId && this.selectedDepartmentId) ||
    //   (this.selectedFacilityId && this.createFacilityReport)) &&
    //   this.selectedSessiontype && this.fromDate && this.toDate);

    return (
      (
        ((this.selectedFacilityId || this.selectedFacilities.length > 0) && this.selectedDepartments.length > 0) ||
        ((this.selectedFacilityId || this.selectedFacilities.length > 0) && this.createFacilityReport)
      ) && this.selectedSessiontype && this.fromDate && this.toDate);
  }

  saveReport() {
    this.toastrService.clear();
    
    const departmentIds = this.selectedDepartments?.map(dep => dep.id) ?? [];
    // const facilityTypeIds = this.selectedFacilityTypes?.map(t => t.id) ?? [];
    const facilityIds = this.selectedFacilityId ? [this.selectedFacilityId] : this.selectedFacilities?.map(t => t.id) ?? [];
    // const departmentTypeIds = this.selectedDepartmentTypes?.map(t => t.id) ?? [];

    this.reportService.reportForSessionTypeHasData(
      {
        sessionType: this.selectedSessiontype,
        // facilityTypeIds: [], 
        facilityIds: facilityIds,
        // departmentTypeIds: [],
        departmentIds: departmentIds,
                fromDate: DateMomentHelper.dateTimeToDate(this.fromDate, "YYYY-MM-DD"),
        toDate: DateMomentHelper.dateTimeToDate(this.toDate, "YYYY-MM-DD"),
        roleId: this.selectedRole
      }).subscribe(
        reportHasData => {
          if (reportHasData) {
            this.storedReport = true;

            let baseUrl = SessionTypeReportUrlMapper.getReportUrlMap().get(this.selectedSessiontype);
            let url = `${baseUrl}`;
                const payload: DownloadExcelModel = {
                  departmentIds: departmentIds,
                  facilityIds: facilityIds,
                  fromDate: DateMomentHelper.dateTimeToDate(this.fromDate, "YYYY-MM-DD"),
                  toDate: DateMomentHelper.dateTimeToDate(this.toDate, "YYYY-MM-DD"),
                  role: this.selectedRole
                } 
        
            this.downloadExcel(url, payload).subscribe(() => {
              this.storedReport = false;
            },
              (error) => {
                this.storedReport = false;
                this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant('An error occurred during download'), { disableTimeOut: true });
              });
          } else {
            // this.toastrService.info('There are no observations for selected values', '', { positionClass: 'toast-center-center' });
            this.toastrService.info(this.translate.instant('There are no observations for selected values'), '');

          }
        })
  }

  loadCoordinatorFacilityDepartments(facilityId: number) {
    this.facilityService.getFacility(facilityId).subscribe(
      facility => {
        this.departments = facility.departments;
        this.allDepartments = this.departments;
        // this.selectedFacilityTypes.push(facility.facilityType);
      })
  };

  private downloadExcel(url: string, payload: DownloadExcelModel): Observable<any> {
    return DownloadFileHelper.downloadFile(url, 'application/xlsx, */*', payload)
  }

  private getFacility(facilityId: number) {
    this.facilityService.getFacility(facilityId).subscribe(
      facility => {
        this.departments = facility.departments;
      })
  };
}
