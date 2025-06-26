import { Component, OnInit, OnDestroy } from '@angular/core';
import { FhiDiagramOptions } from '@folkehelseinstituttet/angular-highcharts';
import { ToastrService } from 'ngx-toastr';
import { Department} from '../../../../models/api/Department';
import { Role } from '../../../../models/api/Role';
import { InstitutionService } from '../../../../services/data/institution.service';
import { ReportService } from '../../../../services/data/report.service';
import { RoleService } from '../../../../services/data/role.service';

@Component({
  selector: 'app-compliance',
  templateUrl: './compliance.component.html'
})
export class ComplianceComponent implements OnInit, OnDestroy {

  fromYear: number = 2024;
  toYear: number = 2024;
  fromMonth: number = 1;
  toMonth: number = 1;
  role: Role = null;
  department: Department= null;
  intervall: string = 'month';
  months: any [];

  showGraph = false;
  roles: Role[];
  departments: Department[];

  percentageDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };

  numberDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };
  constructor(
    private graphService: ReportService,
    private institutionService: InstitutionService,
    private roleService: RoleService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {

    this.months = this.initMonths();
    this.loadRoles();
    this.loadDepartments();
  }

  ngOnDestroy(): void {
  }

  loadRoles() {
    this.roleService.getRoles().subscribe(
      (roles) => this.roles = roles,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  loadDepartments() {
    var institutionId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getDepartments(institutionId).subscribe(
      (departments) => this.departments = departments,
      (error) => this.toastrService.error('An error occurred while loading roles: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  getComplianceForFiveIndications() {
    var institutionId = this.institutionService.getSelectedInstitutionId();
    this.graphService.getComplianceForFiveIndications(institutionId, this.intervall, this.fromMonth, this.fromYear, this.toMonth, this.toYear, this.role?.id, this.department?.id).subscribe(
      (grafer) => {

        let percentageGraph = grafer[0];
        this.savePercentageChartOptions(percentageGraph);
        let antallGraf = grafer[1];
        this.saveNumberDiagramOptions(antallGraf);
        this.showGraph = true;
      },
      (error) => this.toastrService.error('Error in generating graph data: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  savePercentageChartOptions(graph: any) {
    this.percentageDiagramOptions = {
      title: graph.title,
      diagramTypeId: 'line',
      series: graph.graphDataList,
      openSource: false,
      units: [{
        id: 'percent',
        decimals: 1,
        label: 'Compliance (%)',
        symbol: '%',
        position: 'end'
      }]
    };
  }

  saveNumberDiagramOptions(graph: any) {
    this.numberDiagramOptions = {
      title: graph.title,
      diagramTypeId: 'line',
      series: graph.graphDataList,
      openSource: false,
      units: [{
        id: 'number',
        label: 'Number',
        position: 'end'
      }]
    };
  }

  initMonths() {
    return [
      { value: 1, description: "January" },
      { value: 2, description: "February" },
      { value: 3, description: "March" },
      { value: 4, description: "April" },
      { value: 5, description: "May" },
      { value: 6, description: "June" },
      { value: 7, description: "July" },
      { value: 8, description: "August" },
      { value: 9, description: "September" },
      { value: 10, description: "October" },
      { value: 11, description: "November" },
      { value: 12, description: "December" }];
  }
}
