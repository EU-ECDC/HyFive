import { Component, OnInit } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { FacilityReport } from '../../models/api/FacilityReport';

@Component({
  selector: 'app-editing-of-coordinators',
  templateUrl: './editing-of-coordinators.component.html'
})
export class EditingCoordinatorsComponent implements OnInit {

  facilityReport: FacilityReport = null;
  constructor(private facilityService: FacilityService) { }

  ngOnInit(): void {
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    this.facilityService.getFacility(selectedFacilityId).subscribe((result) => {
      this.facilityReport = {
        id: result.id,
        herId: result.herId,
        abbreviation: result.abbreviation,
        facilityType: result.facilityType,
        name: result.name,
        region: result.region,
        healthcareOrganization: result.healthcareOrganization
      } as FacilityReport;
    });
  }
}
