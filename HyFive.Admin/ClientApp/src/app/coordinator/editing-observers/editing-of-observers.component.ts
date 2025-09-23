import { Component, OnInit } from '@angular/core';
import { FacilityService } from '../../services/data/facility.service';
import { QueryParameters } from '../../_common/konstanter/queryparameters';
import { FacilityReport } from '../../models/api/FacilityReport';

@Component({
  selector: 'app-editing-of-observers',
  templateUrl: './editing-of-observers.component.html'
})
export class EditingOfObserversComponent implements OnInit {

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
        name: result.name
      } as FacilityReport;
    });
  }

}
