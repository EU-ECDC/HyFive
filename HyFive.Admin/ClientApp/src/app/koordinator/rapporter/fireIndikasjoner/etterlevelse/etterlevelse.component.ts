import { Component, OnInit, OnDestroy } from '@angular/core';
import { FhiDiagramOptions } from '@folkehelseinstituttet/angular-highcharts';
import { ToastrService } from 'ngx-toastr';
import { Department} from '../../../../models/api/Department';
import { Role } from '../../../../models/api/Role';
import { InstitutionService } from '../../../../services/data/institution.service';
import { ReportService } from '../../../../services/data/report.service';
import { RoleService } from '../../../../services/data/role.service';

@Component({
  selector: 'app-etterlevelse',
  templateUrl: './etterlevelse.component.html'
})
export class EtterlevelseComponent implements OnInit, OnDestroy {

  fromYear: number = 2024;
  toYear: number = 2024;
  fromMonth: number = 1;
  toMonth: number = 1;
  rolle: Role = null;
  avdeling: Department= null;
  intervall: string = 'maned';
  maneder: any [];

  visGraf = false;
  roles: Role[];
  departments: Department[];

  prosentDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };

  antallDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };
  constructor(
    private grafService: ReportService,
    private institutionService: InstitutionService,
    private roleService: RoleService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {

    this.maneder = this.initManeder();
    this.loadRoles();
    this.lastAvdelinger();
  }

  ngOnDestroy(): void {
  }

  loadRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  lastAvdelinger() {
    var institutionId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getDepartments(institutionId).subscribe(
      (departments) => this.departments = departments,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  getComplianceForFourIndications() {
    var institutionId = this.institutionService.getSelectedInstitutionId();
    this.grafService.getComplianceForFourIndications(institutionId, this.intervall, this.fromMonth, this.fromYear, this.toMonth, this.toYear, this.rolle?.id, this.avdeling?.id).subscribe(
      (grafer) => {

        let prosentGraf = grafer[0];
        this.lagProsentDiagramOptions(prosentGraf);
        let antallGraf = grafer[1];
        this.lagAntallDiagramOptions(antallGraf);
        this.visGraf = true;
      },
      (error) => this.toastrService.error('Error i generering av grafdata: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  lagProsentDiagramOptions(graf: any) {
    this.prosentDiagramOptions = {
      title: graf.tittel,
      diagramTypeId: 'line',
      series: graf.grafDataListe,
      openSource: false,
      units: [{
        id: 'prosent',
        decimals: 1,
        label: 'Etterlevelse (%)',
        symbol: '%',
        position: 'end'
      }]
    };
  }

  lagAntallDiagramOptions(graf: any) {
    this.antallDiagramOptions = {
      title: graf.tittel,
      diagramTypeId: 'line',
      series: graf.grafDataListe,
      openSource: false,
      units: [{
        id: 'antall',
        label: 'Antall',
        position: 'end'
      }]
    };
  }

  initManeder() {
    return [
      { value: 1, description: "Januar" },
      { value: 2, description: "Februar" },
      { value: 3, description: "Mars" },
      { value: 4, description: "April" },
      { value: 5, description: "Mai" },
      { value: 6, description: "Juni" },
      { value: 7, description: "Juli" },
      { value: 8, description: "August" },
      { value: 9, description: "September" },
      { value: 10, description: "Oktober" },
      { value: 11, description: "November" },
      { value: 12, description: "Desember" }];
  }
}
