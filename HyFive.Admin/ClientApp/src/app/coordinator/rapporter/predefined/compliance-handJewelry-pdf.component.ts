import { Component } from '@angular/core';
import { SessionType } from 'src/app/models/api/SessionType';
@Component({
  selector: 'app-compliance-handJewelry-pdf',
  templateUrl: './compliance-handJewelry-pdf.component.html'
})
  
export class ComplianceHandJewelryPdfComponent {
  constructor(
    ) { }

  sessionType = SessionType.HandJewelry;
}
