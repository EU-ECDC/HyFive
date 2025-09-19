import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { FacilityService } from '../../services/data/facility.service';
import { FacilityReport } from '../../models/api/FacilityReport';
import { ObservationService } from '../../services/data/observation.service';
import { SessionType } from '../../models/api/SessionType';
import { QueryParameters } from '../../_common/konstanter/queryparameters';
import { UrlPaths } from '../../_common/konstanter/url-paths';
import { faArrowRight, faFileDownload, faFileExcel, faFilePdf } from '@fortawesome/free-solid-svg-icons';
import { FacilityOverviewReport } from "../../models/api/FacilityOverviewReport";
import { ToastrService } from "ngx-toastr";
import { AuthorizedRole } from 'src/app/_common/authorization/authorized-role';
import { AuthorizationService } from 'src/app/_common/services/authorization.service';
import { IColumnSortedEvent } from 'src/app/shared/sorting/sort.service';
import { DepartmentOverviewReport } from 'src/app/models/api/DepartmentOverviewReport';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-overview-observations',
  templateUrl: './overview-observations.component.html'
})
export class OverviewObservationsComponent implements OnInit, OnDestroy {

  SessionType = SessionType;
  faArrowRight = faArrowRight;
  faFileDownload = faFileDownload;
  faFileExcel = faFileExcel;
  faFilePdf = faFilePdf;

  sessiontypes = [
    {name: 'All', value: null},
    { name: 'Protective Equipment', value: SessionType.ProtectiveEquipment },
    { name: 'Five Indications', value: SessionType.FiveIndications },
    { name: 'Gloves', value: SessionType.Gloves },
    { name: 'Hand Jewelry', value: SessionType.HandJewelry }
  ];

  selectedSessiontype: SessionType = null;
  facilityIdForReportAsDownloaded = 0;

  fromDate: Date = null;
  toDate: Date = null;
  facilities: FacilityReport[] = [];
  selectedFacilityId: string = null;
  departmentIdForReportAsDownloaded = 0;

  SelectedFacilityFromListId: number = 0;

  facilityOverviewReportList: FacilityOverviewReport[] = [];
  canSelectFacility = false;
  searching = false;
  private selectedRole: AuthorizedRole;

  constructor(
    private facilityService: FacilityService,
    private observationService: ObservationService,
    private route: ActivatedRoute,
    private router: Router,
    private toastrService: ToastrService,
    private authorizationService: AuthorizationService,
    private httpClient: HttpClient) { }

  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();
    this.selectedFacilityId = this.getFacilityId();

    this.route
      .queryParams
      .subscribe(params => {
        if (Object.keys(params).length === 0) {
          return;
        }

        this.selectedSessiontype = parseInt(params[QueryParameters.SessionType], 10) || null;
        this.fromDate = params[QueryParameters.FromDate] || null;
        this.toDate = params[QueryParameters.ToDate] || null;
        if (this.selectedRole === AuthorizedRole.Administrator)
          this.selectedFacilityId = params[QueryParameters.facilityIdIsOk] || null;
        this.getFacilitiesWithSessions();
      });

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.canSelectFacility = true;
      this.getFacility();
    } else if (this.selectedRole === AuthorizedRole.Coordinator) {
      this.canSelectFacility = false;
    }
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  getFacilityId(): string {
    if(this.selectedRole === AuthorizedRole.Coordinator)
      return this.facilityService.getSelectedFacilityId().toString();
    return "null";
  }

  getFacility() {
    this.facilityService.getFacilities().subscribe(
      (facilities) => {
        this.facilities = [ 
                            { name: 'All', 
                              id: null,
                              abbreviation: null,
                              herId: null,
                              facilityType: null,
                              region: null,
                              municipality: null,
                              healthcareOrganization: null 
                            },
                            ...facilities
        ];
        this.selectedFacilityId = null;
      },
      error => {
        this.toastrService.error(error.error.message, 'Loading of facilities failed', {disableTimeOut: true});
      });
  }

  getFacilitiesWithSessions() {
    let selectedFacilityId = this.selectedFacilityId;
    if (this.selectedFacilityId === null || this.selectedFacilityId === 'null') {
      selectedFacilityId = null;
    }
    this.searching = true;
    this.facilityOverviewReportList = new Array<FacilityOverviewReport>();

    this.observationService.getFacilitiesWithSessions(
      selectedFacilityId,
      this.selectedSessiontype ? this.selectedSessiontype : null,
      this.fromDate,
      this.toDate,
      this.selectedRole
    ).subscribe((results) => {
      if(this.selectedSessiontype !== SessionType.FiveIndications && this.selectedSessiontype !== SessionType.HandJewelry &&
        this.selectedSessiontype !== SessionType.Gloves && this.selectedSessiontype !== SessionType.ProtectiveEquipment)
        this.selectedSessiontype = null;
      this.facilityOverviewReportList = results;
      this.searching = false;
    },
      error => {
        this.toastrService.error(error.error.message, 'Loading the list failed', {disableTimeOut: true});
        this.facilityOverviewReportList = new Array<FacilityOverviewReport>();
        this.searching = false;
      });

  }

  navigateToObservationsForTheDepartment(department) {
    this.router.navigate([`/${UrlPaths.observationsDepartment}`], {
      queryParams: this.trim({
        departmentid: department.id,
        sessiontype: this.selectedSessiontype,
        from: this.fromDate,
        to: this.toDate,
        facilityIdSearch: this.selectedFacilityId
      })
    });
  }

  reset(): void {
    this.selectedSessiontype = null;
    this.fromDate = null;
    this.toDate = null;

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedFacilityId = null;
    }

    this.facilityOverviewReportList = new Array<FacilityOverviewReport>();
  }

  resetSearchresults() {
    this.facilityOverviewReportList = new Array<FacilityOverviewReport>();
  }

  loadSelectedSessionTypeText(): string {
    if (this.selectedSessiontype == SessionType.ProtectiveEquipment)
      return "Protective Equipment";
    if (this.selectedSessiontype == SessionType.FiveIndications)
      return "Five Indications";
    if (this.selectedSessiontype == SessionType.HandJewelry)
      return "Hand Jewelry";
    if (this.selectedSessiontype == SessionType.Gloves)
      return "Gloves";

    return "All";
  }

  private trim(params: Params): Params {
    Object.keys(params).forEach((k) => (params[k] == null || params[k] === 'null') && delete params[k]);
    return params;
  }

  findId(item: any) {
    this.SelectedFacilityFromListId = item.id;
  }

  sort($event: IColumnSortedEvent) {

    event.stopPropagation();
    event.preventDefault();

    let propertyOf: (x: DepartmentOverviewReport) => any;
    switch ($event.columnName) {
      case "Name":
        propertyOf = (x: DepartmentOverviewReport) => x.name;
        break;
      default:
        throw new Error("Invalid sort column");
    }

    const sortOrder = $event.sortDirection === "asc" ? 1 : -1;

    const sortFunc = (a: DepartmentOverviewReport, b: DepartmentOverviewReport) => {
      const result = (propertyOf(a) < propertyOf(b)) ? -1 : (propertyOf(a) > propertyOf(b)) ? 1 : 0;
      return result * sortOrder;
    };

    let index = this.facilityOverviewReportList.findIndex(x => x.id == this.SelectedFacilityFromListId);

    if (index > -1) {
      this.facilityOverviewReportList[index].departments = this.facilityOverviewReportList[index].departments.sort(sortFunc);
    }
  }
}
