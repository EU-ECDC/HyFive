import { Component } from '@angular/core';
import { SessionType } from 'src/app/models/api/SessionType';
@Component({
  selector: 'app-compliance-five-indications-pdf',
  templateUrl: './compliance-five-indications-pdf.component.html'
})

export class ComplianceFiveIndicationsPdfComponent {
  constructor(
    ) { }

  sessionType = SessionType.FiveIndications;
}
