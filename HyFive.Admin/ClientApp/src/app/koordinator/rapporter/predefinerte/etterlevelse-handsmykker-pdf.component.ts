import { Component } from '@angular/core';
import { SessionType } from 'src/app/models/api/SessionType';
@Component({
  selector: 'app-etterlevelse-handsmykker-pdf',
  templateUrl: './etterlevelse-handsmykker-pdf.component.html'
})
  
export class EtterlevelseHandsmykkerPdfComponent {
  constructor(
    ) { }

  sesjonType = SessionType.Handjewelry;
}
