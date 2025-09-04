import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';
import { ObservationService } from '../../services/data/observation.service';
import { SessionType } from '../../models/api/SessionType';
import { QueryParameters } from '../../_common/konstanter/queryparameters';
import { UrlPaths } from '../../_common/konstanter/url-paths';
import { faArrowRight, faFileDownload, faFileExcel, faFilePdf } from '@fortawesome/free-solid-svg-icons';
import { InstitutionOverviewReport } from "../../models/api/InstitutionOverviewReport";
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
  institutionIdForReportAsDownloaded = 0;

  fromDate: Date = null;
  toDate: Date = null;
  institutions: InstitutionReport[] = [];
  selectedInstitutionId: string = null;
  departmentIdForReportAsDownloaded = 0;

  selectedInstitutionFromListId: number = 0;

  institutionOverviewReportList: InstitutionOverviewReport[] = [];
  canSelectInstitution = false;
  searching = false;
  private selectedRole: AuthorizedRole;

  constructor(
    private institutionService: InstitutionService,
    private observationService: ObservationService,
    private route: ActivatedRoute,
    private router: Router,
    private toastrService: ToastrService,
    private authorizationService: AuthorizationService,
    private httpClient: HttpClient) { }

  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();
    this.selectedInstitutionId = this.getInstitutionId();

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
          this.selectedInstitutionId = params[QueryParameters.InstitutionIdIsOk] || null;
        this.getInstitutionsWithSessions();
      });

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.canSelectInstitution = true;
      this.getInstitution();
    } else if (this.selectedRole === AuthorizedRole.Coordinator) {
      this.canSelectInstitution = false;
    }
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  getInstitutionId(): string {
    if(this.selectedRole === AuthorizedRole.Coordinator)
      return this.institutionService.getSelectedInstitutionId().toString();
    return "null";
  }

  getInstitution() {
    this.institutionService.getInstitutions().subscribe(
      (institutions) => {
        this.institutions = [ 
                            { name: 'All', 
                              id: null,
                              abbreviation: null,
                              herId: null,
                              institutionType: null,
                              region: null,
                              municipality: null,
                              healthcareOrganization: null 
                            },
                            ...institutions
        ];
        this.selectedInstitutionId = null;
      },
      error => {
        this.toastrService.error(error.error.message, 'Loading of institutions failed', {disableTimeOut: true});
      });
  }

  getInstitutionsWithSessions() {
    let selectedInstitutionId = this.selectedInstitutionId;
    if (this.selectedInstitutionId === null || this.selectedInstitutionId === 'null') {
      selectedInstitutionId = null;
    }
    this.searching = true;
    this.institutionOverviewReportList = new Array<InstitutionOverviewReport>();

    this.observationService.getInstitutionsWithSessions(
      selectedInstitutionId,
      this.selectedSessiontype ? this.selectedSessiontype : null,
      this.fromDate,
      this.toDate,
      this.selectedRole
    ).subscribe((results) => {
      if(this.selectedSessiontype !== SessionType.FiveIndications && this.selectedSessiontype !== SessionType.HandJewelry &&
        this.selectedSessiontype !== SessionType.Gloves && this.selectedSessiontype !== SessionType.ProtectiveEquipment)
        this.selectedSessiontype = null;
      this.institutionOverviewReportList = results;
      this.searching = false;
    },
      error => {
        this.toastrService.error(error.error.message, 'Loading the list failed', {disableTimeOut: true});
        this.institutionOverviewReportList = new Array<InstitutionOverviewReport>();
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
        institutionIdSearch: this.selectedInstitutionId
      })
    });
  }

  reset(): void {
    this.selectedSessiontype = null;
    this.fromDate = null;
    this.toDate = null;

    if (this.selectedRole === AuthorizedRole.Administrator) {
      this.selectedInstitutionId = null;
    }

    this.institutionOverviewReportList = new Array<InstitutionOverviewReport>();
  }

  resetSearchresults() {
    this.institutionOverviewReportList = new Array<InstitutionOverviewReport>();
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
    this.selectedInstitutionFromListId = item.id;
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

    let index = this.institutionOverviewReportList.findIndex(x => x.id == this.selectedInstitutionFromListId);

    if (index > -1) {
      this.institutionOverviewReportList[index].departments = this.institutionOverviewReportList[index].departments.sort(sortFunc);
    }
  }
}
