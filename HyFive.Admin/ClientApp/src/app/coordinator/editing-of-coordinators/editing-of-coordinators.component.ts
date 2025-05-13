import { Component, OnInit } from '@angular/core';
import { InstitutionService } from '../../services/data/institution.service';
import { InstitutionReport } from '../../models/api/InstitutionReport';

@Component({
  selector: 'app-editing-of-coordinators',
  templateUrl: './editing-of-coordinators.component.html'
})
export class EditingCoordinatorsComponent implements OnInit {

  institutionReport: InstitutionReport = null;
  constructor(private institutionService: InstitutionService) { }

  ngOnInit(): void {
    let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
    this.institutionService.getInstitution(selectedInstitutionId).subscribe((result) => {
      this.institutionReport = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        institutionType: result.institutionType,
        name: result.name,
        region: result.region,
        healthcareOrganization: result.healthcareOrganization
      } as InstitutionReport;
    });
  }
}
