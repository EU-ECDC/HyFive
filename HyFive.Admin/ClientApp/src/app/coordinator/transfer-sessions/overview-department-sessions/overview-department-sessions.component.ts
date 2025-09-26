import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QueryParameters } from '../../../_common/constants/queryparameters';
import { SessionType } from '../../../models/api/SessionType';
import { ObservationService } from '../../../services/data/observation.service';
import { SessionOverviewReport } from '../../../models/api/SessionOverviewReport';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { DatePipe } from '@angular/common';
import { DepartmentService } from '../../../services/data/department.service';
import { Department} from '../../../models/api/Department';
import { AuthorizedRole } from '../../../_common/authorization/authorized-role';
import { AuthorizationService } from '../../../_common/services/authorization.service';

@Component({
  selector: 'app-overview-department-sessions',
  templateUrl: './overview-department-sessions.component.html'
})
export class OverviewDepartmentSessionsComponent implements OnInit {

  departmentid: number;
  selectedSessiontype: SessionType = null;
  fromDate: Date;
  toDate: Date;
  selectedFacilityOptions: number = null;

  sessionTypeOptions = [
    { name: "FiveIndications", value: SessionType.FiveIndications },
    { name: "HandJewelry", value: SessionType.HandJewelry },
    { name: "ProtectiveEquipment", value: SessionType.ProtectiveEquipment },
    { name: "Gloves", value: SessionType.Gloves }
  ];

  department: Department;
  sessions: SessionOverviewReport[] = [];
  loading: boolean;
  selectedRole: AuthorizedRole;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private departmentService: DepartmentService,
    private observationService: ObservationService,
    private datepipe: DatePipe,
    private authorizationService: AuthorizationService  ) { }


  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();
    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.DepartmentId]) this.router.navigate([`/${UrlPaths.observations}`]);

        this.selectedSessiontype = parseInt(params[QueryParameters.SessionType]) || null;
        this.fromDate = params[QueryParameters.FromDate] || null;
        this.toDate = params[QueryParameters.ToDate] || null;
        this.departmentid = parseInt(params[QueryParameters.DepartmentId]) || null;
        this.selectedFacilityOptions = parseInt(params[QueryParameters.FacilityIdeas]) || null;

        this.getDepartment();
        this.getSessionsForDepartment();
      });
  }

  getDepartment() {
    this.departmentService.getDepartment(
      this.departmentid
    ).subscribe((result) => {
      this.department = result;
    });
  }

  getSessionsForDepartment() {
    this.loading = true;
    this.observationService.getSessionsForDepartment(
      this.departmentid,
      this.selectedSessiontype ? this.selectedSessiontype : null,
      this.fromDate,
      this.toDate,
      this.selectedRole
    ).subscribe((results) => {
      this.sessions = results;

      this.loading = false;
    });
  }

  showFormattedDateWithTime(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy, HH:mm:ss');
  }

  showFormattedDate(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy');
  }

  navigateToObservationsForFacilities() {
    this.router.navigate([`/${UrlPaths.observations}`], {
      queryParams: {
        SessionType: this.selectedSessiontype,
        FromDate: this.fromDate,
        ToDate: this.toDate,
        FacilityIdeas: this.selectedFacilityOptions
      }
    });
  }
}

