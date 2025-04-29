import { Component } from '@angular/core';
import { SessionType } from 'src/app/models/api/SessionType';
@Component({
  selector: 'app-compliance-four-indications-pdf',
  templateUrl: './compliance-four-indications-pdf.component.html'
})

export class ComplianceFourIndicationsPdfComponent {
  constructor(
    ) { }

  sessionType = SessionType.FourIndications;
}
