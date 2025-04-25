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
  selector: 'app-oversikt-avdeling-sesjoner',
  templateUrl: './oversikt-avdeling-sesjoner.component.html'
})
export class OversiktAvdelingSesjonerComponent implements OnInit {

  avdelingsid: number;
  selectedSessiontype: SessionType = null;
  fromDate: Date;
  toDate: Date;
  selectedInstitutionId: number = null;
  institutionIdSearch: number;

  sesjontypeAlternativer = [
    { name: "FourIndications", value: SessionType.FourIndications, type: SessionType[SessionType.FourIndications] },
    { name: "Håndsmykker", value: SessionType.Handjewelry, type: SessionType[SessionType.Handjewelry] },
    { name: "Gloves", value: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "ProtectiveEquipment", value: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
  ];

  avdeling: Department;
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
        this.avdelingsid = parseInt(params[QueryParameters.DepartmentId]) || null;
      });

      const avdelingSesjonerRequest = [
        this.departmentService.getDepartment(this.avdelingsid),
        this.observationService.getSessionsForDepartment(
          this.avdelingsid,
          this.selectedSessiontype ? this.selectedSessiontype : null,
          this.fromDate,
          this.toDate,
          this.selectedRole
        )
      ]

      forkJoin(avdelingSesjonerRequest).subscribe((result) => {
        let i = 0;
        this.avdeling = result[i++] as Department;
        this.session = result[i++] as SessionOverviewReport[];

        this.loading = false;
        
        if(this.erKoordinatorByttetInstitusjon())
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

  erKoordinatorByttetInstitusjon() {
    return this.avdeling.institutionId !== this.selectedInstitutionId && this.selectedRole == AuthorizedRole.Coordinator;
  }

  getSessionsForDepartment() {
    
    this.observationService.getSessionsForDepartment(
      this.avdelingsid,
      this.selectedSessiontype ? this.selectedSessiontype : null,
      this.fromDate,
      this.toDate,
      this.selectedRole
    ).subscribe((results) => {
      this.session = results; 
    });
  }

  visFormatedDatoMedTidspunkt(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy, HH:mm:ss');
  }

  visFormatedDato(date: Date) {
    return this.datepipe.transform(date, 'dd.MM.yyyy');
  }

  navigerTilObservasjonerForInstitusjoner() {
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
