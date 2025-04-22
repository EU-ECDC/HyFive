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
  valgtSesjontype: SessionType = null;
  fraDato: Date;
  tilDato: Date;
  valgtInstitusjonId: number = null;
  institusjonsidISok: number;

  sesjontypeAlternativer = [
    { name: "FourIndications", verdi: SessionType.FourIndications, type: SessionType[SessionType.FourIndications] },
    { name: "Håndsmykker", verdi: SessionType.Handjewelry, type: SessionType[SessionType.Handjewelry] },
    { name: "Gloves", verdi: SessionType.Gloves, type: SessionType[SessionType.Gloves] },
    { name: "ProtectiveEquipment", verdi: SessionType.ProtectiveEquipment, type: SessionType[SessionType.ProtectiveEquipment] },
  ];

  avdeling: Department;
  session: SessionOverviewReport[] = [];
  laster: boolean;
  valgtRolle: AuthorizedRole;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private departmentService: DepartmentService,
    private observationService: ObservationService,
    private datepipe: DatePipe,
    private authorizationService: AuthorizationService,
    private institusjonService: InstitutionService
  ) { }


  ngOnInit(): void {
    this.laster = true;
    this.valgtRolle = this.authorizationService.hentValgtRolle();
    this.valgtInstitusjonId = this.hentInstitusjonId();

    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.DepartmentId]) this.router.navigate([`/${UrlPaths.observations}`]);

        this.valgtSesjontype = parseInt(params[QueryParameters.SessionType]) || null;
        this.fraDato = params[QueryParameters.FromDate] || null;
        this.tilDato = params[QueryParameters.ToDate] || null;
        this.institusjonsidISok = params[QueryParameters.InstitutionIdIsOk] || null;
        this.avdelingsid = parseInt(params[QueryParameters.DepartmentId]) || null;
      });

      const avdelingSesjonerRequest = [
        this.departmentService.getDepartment(this.avdelingsid),
        this.observationService.getSessionsForDepartment(
          this.avdelingsid,
          this.valgtSesjontype ? this.valgtSesjontype : null,
          this.fraDato,
          this.tilDato,
          this.valgtRolle
        )
      ]

      forkJoin(avdelingSesjonerRequest).subscribe((result) => {
        let i = 0;
        this.avdeling = result[i++] as Department;
        this.session = result[i++] as SessionOverviewReport[];

        this.laster = false;
        
        if(this.erKoordinatorByttetInstitusjon())
        {
          this.router.navigate([`/${UrlPaths.observations}`], {
            queryParams: {
              sesjontype: null,
              fra: null,
              til: null,
            }
            });
        }
      });
  }

  hentInstitusjonId(): number {
    if(this.valgtRolle === AuthorizedRole.Coordinator) 
      return this.institusjonService.hentValgtInstitusjonId()
    return null;
  }

  erKoordinatorByttetInstitusjon() {
    return this.avdeling.institutionId !== this.valgtInstitusjonId && this.valgtRolle == AuthorizedRole.Coordinator;
  }

  getSessionsForDepartment() {
    
    this.observationService.getSessionsForDepartment(
      this.avdelingsid,
      this.valgtSesjontype ? this.valgtSesjontype : null,
      this.fraDato,
      this.tilDato,
      this.valgtRolle
    ).subscribe((resultater) => {
      this.session = resultater; 
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
        sesjontype: this.valgtSesjontype,
        fra: this.fraDato,
        til: this.tilDato,
        institusjonsidISok: this.institusjonsidISok
      }
    });
  }
}
