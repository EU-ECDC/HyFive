import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { QueryParameters } from '../../../_felles/konstanter/queryparameters';
import { SessionType } from '../../../models/api/SessionType';
import { ObservationService } from '../../../services/data/observation.service';
import { SessionOverviewReport } from '../../../models/api/SessionOverviewReport';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { DatePipe } from '@angular/common';
import { AvdelingService } from '../../../services/data/avdeling.service';
import { Department} from '../../../models/api/Department';
import { AuthorizedRole } from '../../../_felles/authorization/authorized-role';
import { AuthorizationService } from '../../../_felles/services/authorization.service';

@Component({
  selector: 'app-oversikt-avdeling-sesjoner',
  templateUrl: './oversikt-avdeling-sesjoner.component.html'
})
export class OversiktAvdelingSesjonerComponent implements OnInit {

  avdelingsid: number;
  valgtSesjontype: SessionType = null;
  fraDato: Date;
  tilDato: Date;
  valgteInstitusjonAlternativer: number = null;

  sesjontypeAlternativer = [
    { name: "FourIndications", verdi: SessionType.FourIndications },
    { name: "Håndsmykker", verdi: SessionType.Handjewelry },
    { name: "ProtectiveEquipment", verdi: SessionType.ProtectiveEquipment },
    { name: "Gloves", verdi: SessionType.Gloves }
  ];

  avdeling: Department;
  sessions: SessionOverviewReport[] = [];
  laster: boolean;
  valgtRolle: AuthorizedRole;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private avdelingService: AvdelingService,
    private observationService: ObservationService,
    private datepipe: DatePipe,
    private authorizationService: AuthorizationService  ) { }


  ngOnInit(): void {
    this.valgtRolle = this.authorizationService.hentValgtRolle();
    this.route
      .queryParams
      .subscribe(params => {
        if (!params[QueryParameters.DepartmentId]) this.router.navigate([`/${UrlPaths.observations}`]);

        this.valgtSesjontype = parseInt(params[QueryParameters.SessionType]) || null;
        this.fraDato = params[QueryParameters.FromDate] || null;
        this.tilDato = params[QueryParameters.ToDate] || null;
        this.avdelingsid = parseInt(params[QueryParameters.DepartmentId]) || null;
        this.valgteInstitusjonAlternativer = parseInt(params[QueryParameters.InstitutionIdeas]) || null;

        this.hentAvdeling();
        this.getSessionsForDepartment();
      });
  }

  hentAvdeling() {
    this.avdelingService.hentAvdeling(
      this.avdelingsid
    ).subscribe((resultat) => {
      this.avdeling = resultat;
    });
  }

  getSessionsForDepartment() {
    this.laster = true;
    this.observationService.getSessionsForDepartment(
      this.avdelingsid,
      this.valgtSesjontype ? this.valgtSesjontype : null,
      this.fraDato,
      this.tilDato,
      this.valgtRolle
    ).subscribe((resultater) => {
      this.sessions = resultater;

      this.laster = false;
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
        institusjonider: this.valgteInstitusjonAlternativer
      }
    });
  }
}

