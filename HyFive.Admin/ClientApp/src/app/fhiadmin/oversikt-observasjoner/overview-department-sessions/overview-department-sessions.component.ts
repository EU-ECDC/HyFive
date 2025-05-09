import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QueryParameters } from '../../../_felles/konstanter/queryparameters';
import { SessionType } from '../../../models/api/SessionType';
import { ObservationService } from '../../../services/data/observation.service';
import { SessionOverviewReport } from '../../../models/api/SessionOverviewReport';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { DatePipe } from '@angular/common';
import { DepartmentService } from '../../../services/data/department.service';
import { Department} from '../../../models/api/Department';
import { AuthorizedRole } from '../../../_felles/authorization/authorized-role';
import { AuthorizationService } from '../../../_felles/services/authorization.service';
import { InstitutionService } from 'src/app/services/data/institution.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-overview-department-sessions',
  templateUrl: './overview-department-sessions.component.html'
})
export class OverviewDepartmentSessionsComponent implements OnInit {

  departmentid: number;
  selectedSessiontype: SessionType = null;
  fromDate: Date;
  toDate: Date;
  selectedInstitutionId: number = null;
  institutionIdSearch: number;

  sessionTypeOptions = [
    { name: "FourIndications", value: SessionType.FourIndications, type: SessionType[SessionType.FourIndications] },
    { name: "HandJewelry", value: SessionType.HandJewelry, type: SessionType[SessionType.HandJewelry] },
    { name: "Gloves", value: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "ProtectiveEquipment", value: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
  ];

  department: Department;
  session: SessionOverviewReport[] = [];
  loading: boolean;
  selectedRole: AuthorizedRole;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private departmentService: DepartmentService,
    private observationService: ObservationService,
    private datepipe: DatePipe,
    private authorizationService: AuthorizationService,
    private institutionService: InstitutionService
  ) { }


  ngOnInit(): void {
    this.loading = true;
    this.selectedRole = this.authorizationService.getSelectedRole();
    this.selectedInstitutionId = this.getInstitutionId();

    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.DepartmentId]) this.router.navigate([`/${UrlPaths.observations}`]);

        this.selectedSessiontype = parseInt(params[QueryParameters.SessionType]) || null;
        this.fromDate = params[QueryParameters.FromDate] || null;
        this.toDate = params[QueryParameters.ToDate] || null;
        this.institutionIdSearch = params[QueryParameters.InstitutionIdIsOk] || null;
        this.departmentid = parseInt(params[QueryParameters.DepartmentId]) || null;
      });

      const departmentSessionsRequest = [
        this.departmentService.getDepartment(this.departmentid),
        this.observationService.getSessionsForDepartment(
          this.departmentid,
          this.selectedSessiontype ? this.selectedSessiontype : null,
          this.fromDate,
          this.toDate,
          this.selectedRole
        )
      ]

      forkJoin(departmentSessionsRequest).subscribe((result) => {
        let i = 0;
        this.department = result[i++] as Department;
        this.session = result[i++] as SessionOverviewReport[];

        this.loading = false;
        
        if(this.isCoordinatorChangedInstitution())
        {
          this.router.navigate([`/${UrlPaths.observations}`], {
            queryParams: {
              sessiontype: null,
              fra: null,
              til: null,
            }
            });
        }
      });
  }

  getInstitutionId(): number {
    if(this.selectedRole === AuthorizedRole.Coordinator) 
      return this.institutionService.getSelectedInstitutionId()
    return null;
  }

  isCoordinatorChangedInstitution() {
    return this.department.institutionId !== this.selectedInstitutionId && this.selectedRole == AuthorizedRole.Coordinator;
  }

  getSessionsForDepartment() {
    
    this.observationService.getSessionsForDepartment(
      this.departmentid,
      this.selectedSessiontype ? this.selectedSessiontype : null,
      this.fromDate,
      this.toDate,
      this.selectedRole
    ).subscribe((results) => {
      this.session = results; 
    });
  }

  showFormattedDateWithTime(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy, HH:mm:ss');
  }

  showFormattedDate(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy');
  }

  navigateToObservationsForInstitutions() {
    this.router.navigate([`/${UrlPaths.observations}`], {
      queryParams: {
        sessiontype: this.selectedSessiontype,
        fra: this.fromDate,
        til: this.toDate,
        institutionIdSearch: this.institutionIdSearch
      }
    });
  }
}
