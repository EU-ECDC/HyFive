import { Component, OnInit } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { QueryParameters } from '../../_felles/konstanter/queryparameters';
import { InstitutionReport } from '../../models/api/InstitutionReport';

@Component({
  selector: 'app-redigering-av-observatorer',
  templateUrl: './redigering-av-observatorer.component.html'
})
export class RedigeringAvObservatorerComponent implements OnInit {

  institusjonRapport: InstitutionReport = null;
  constructor(private institutionService: InstitutionService) { }

  ngOnInit(): void {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(selectedInstitutionId).subscribe((result) => {
      this.institusjonRapport = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        institutionType: result.institutionType,
        name: result.name,
        region: result.region
      } as InstitutionReport;
    });
  }

}
