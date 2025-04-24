import { Component, OnInit } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';

@Component({
  selector: 'app-redigering-av-koordinatorer',
  templateUrl: './redigering-av-koordinatorer.component.html'
})
export class RedigeringAvKoordinatorerComponent implements OnInit {

  institusjonRapport: InstitutionReport = null;
  constructor(private institutionService: InstitutionService) { }

  ngOnInit(): void {
    let valgtInstitusjonsId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(valgtInstitusjonsId).subscribe((result) => {
      this.institusjonRapport = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        institutionType: result.institutionType,
        name: result.name,
        region: result.region,
        healthcareCompany: result.healthcareCompany
      } as InstitutionReport;
    });
  }
}
