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
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { OrganisationUnit } from 'src/app/models/api/OrganisationUnit';
import { UnitService } from 'src/app/services/data/unit.service';
import { FacilityService } from 'src/app/services/data/facility.service';

@Component({
  selector: 'app-overview-department-sessions',
  templateUrl: './overview-department-sessions.component.html'
})
export class OverviewDepartmentSessionsComponent implements OnInit {

  unitId: number;
  selectedSessiontype: SessionType = null;
  fromDate: Date;
  toDate: Date;
  selectedFacilityOptions: number = null;
  selectedFacilityId: number = null;

  sessionTypeOptions = [
    { name: "HandHygiene", value: SessionType.FiveIndications },
    { name: "BareBelowElbows", value: SessionType.HandJewelry },
    { name: "ProtectiveEquipment", value: SessionType.ProtectiveEquipment },
    { name: "Gloves", value: SessionType.Gloves }
  ];

  unit: OrganisationUnit;
  sessions: SessionOverviewReport[] = [];
  loading: boolean;
  selectedRole: AuthorizedRole;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly UnitService: UnitService,
    private readonly observationService: ObservationService,
    private readonly datepipe: DatePipe,
    private readonly authorizationService: AuthorizationService,
    private readonly dialogMessageService: DialogMessageService,
    private readonly facilityService: FacilityService ) {
      this.selectedFacilityId = this.getFacilityId();
     }


  ngOnInit(): void {
    this.selectedRole = this.authorizationService.getSelectedRole();
    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.UnitId]) this.router.navigate([`/${UrlPaths.observations}`]);

        this.selectedSessiontype = Number.parseInt(params[QueryParameters.SessionType]) || null;
        this.fromDate = params[QueryParameters.FromDate] || null;
        this.toDate = params[QueryParameters.ToDate] || null;
        this.unitId = Number.parseInt(params[QueryParameters.UnitId]) || null;
        this.selectedFacilityOptions = Number.parseInt(params[QueryParameters.FacilityIdeas]) || null;

        this.getUnit();
        this.getSessionsForUnit();
      });
  }

  getUnit() {
    this.UnitService.getUnit(
      this.unitId,
      this.selectedFacilityId
    ).subscribe((result) => {
      this.unit = result;
    });
  }

    getFacilityId(): number {
    if(this.selectedRole === AuthorizedRole.Coordinator) 
      return this.facilityService.getSelectedFacilityId()
    return null;
  }

  getSessionsForUnit() {
    this.loading = true;
    this.observationService.getSessionsForUnit(
      this.unitId,
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

