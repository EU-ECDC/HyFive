import { Component } from '@angular/core';
import { SessionType } from 'src/app/models/api/SessionType';
@Component({
  selector: 'app-compliance-hand-hygiene-pdf',
  templateUrl: './compliance-hand-hygiene-pdf.component.html'
})

export class ComplianceHandHygienePdfComponent {
  constructor(
    ) { }

  sessionType = SessionType.FiveIndications;
}
