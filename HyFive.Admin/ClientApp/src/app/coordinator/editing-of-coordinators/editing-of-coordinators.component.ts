import { Component, OnInit } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { FacilityReport } from '../../models/api/FacilityReport';

@Component({
  selector: 'app-editing-of-coordinators',
  templateUrl: './editing-of-coordinators.component.html'
})
export class EditingCoordinatorsComponent implements OnInit {

  facilityReport: FacilityReport = null;
  constructor(private readonly facilityService: FacilityService) { }

  ngOnInit(): void {
    let selectedFacilityId = this.facilityService.getSelectedFacilityId();
    this.facilityService.getFacility(selectedFacilityId).subscribe((result) => {
      this.facilityReport = {
        id: result.id,
        abbreviation: result.abbreviation,
        type: result.type,
        name: result.name,
        city: null
      } as FacilityReport;
    });
  }
}
