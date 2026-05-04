import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QueryParameters } from '../../../_common/constants/queryparameters';
import { SessionType } from '../../../models/api/SessionType';
import { ObservationService } from '../../../services/data/observation.service';
import { SessionOverviewReport } from '../../../models/api/SessionOverviewReport';
import { UrlPaths } from '../../../_common/constants/url-paths';
import { DatePipe } from '@angular/common';
import { AuthorizedRole } from '../../../_common/authorization/authorized-role';
import { AuthorizationService } from '../../../_common/services/authorization.service';
import { FacilityService } from 'src/app/services/data/facility.service';
import { forkJoin } from 'rxjs';
import { UnitService } from 'src/app/services/data/unit.service';
import { OrganisationUnitResponse } from 'src/app/models/api/OrganisationUnitResponse';

@Component({
  selector: 'app-overview-department-sessions',
  templateUrl: './overview-department-sessions.component.html',
  styleUrls: ['./overview-department-sessions.component.scss']
})
export class OverviewDepartmentSessionsComponent implements OnInit {

  unitId: number;
  selectedSessiontype: SessionType = null;
  fromDate: Date;
  toDate: Date;
  selectedFacilityId: number = null;
  facilityIdSearch: number;

  sessionTypeOptions = [
    { name: "HandHygiene", value: SessionType.FiveIndications, type: SessionType[SessionType.FiveIndications] },
    { name: "BareBelowElbows", value: SessionType.HandJewelry, type: SessionType[SessionType.HandJewelry] },
    { name: "Gloves", value: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "ProtectiveEquipment", value: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
  ];

  unit: OrganisationUnitResponse;
  session: SessionOverviewReport[] = [];
  loading: boolean;
  selectedRole: AuthorizedRole;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly unitService: UnitService,
    private readonly observationService: ObservationService,
    private readonly datepipe: DatePipe,
    private readonly authorizationService: AuthorizationService,
    private readonly facilityService: FacilityService
  ) { }


  ngOnInit(): void {
    this.loading = true;
    this.selectedRole = this.authorizationService.getSelectedRole();
    this.selectedFacilityId = this.getFacilityId();

    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.UnitId]) this.router.navigate([`/${UrlPaths.observations}`]);

        this.selectedSessiontype = Number.parseInt(params[QueryParameters.SessionType]) || null;
        this.fromDate = params[QueryParameters.FromDate] || null;
        this.toDate = params[QueryParameters.ToDate] || null;
        this.facilityIdSearch = params[QueryParameters.facilityIdSearch] || null;
        this.unitId = Number.parseInt(params[QueryParameters.UnitId]) || null;
      });

      const departmentSessionsRequest = [
        this.unitService.getUnit(this.unitId, this.selectedFacilityId),
        this.observationService.getSessionsForUnit(
          this.unitId,
          this.selectedSessiontype ? this.selectedSessiontype : null,
          this.fromDate,
          this.toDate,
          this.selectedRole
        )
      ]

      forkJoin(departmentSessionsRequest).subscribe((result) => {
        let i = 0;
        this.unit = result[i++] as OrganisationUnitResponse;
        this.session = result[i++] as SessionOverviewReport[];

        this.loading = false;
        
        if(this.isCoordinatorChangedFacility())
        {
          this.router.navigate([`/${UrlPaths.observations}`], {
            queryParams: {
              sessiontype: null,
              from: null,
              to: null,
            }
            });
        }
      });
  }

  getFacilityId(): number {
    if(this.selectedRole === AuthorizedRole.Coordinator) 
      return this.facilityService.getSelectedFacilityId()
    return null;
  }

  isCoordinatorChangedFacility() {
    return this?.unit?.facilityId !== this?.selectedFacilityId && this.selectedRole == AuthorizedRole.Coordinator;
  }

  getSessionsForUnit() {
    
    this.observationService.getSessionsForUnit(
      this.unitId,
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

  navigateToObservationsForFacilities() {
    this.router.navigate([`/${UrlPaths.observations}`], {
      queryParams: {
        sessiontype: this.selectedSessiontype,
        from: this.fromDate,
        to: this.toDate,
        facilityIdSearch: this.facilityIdSearch
      }
    });
  }
}
