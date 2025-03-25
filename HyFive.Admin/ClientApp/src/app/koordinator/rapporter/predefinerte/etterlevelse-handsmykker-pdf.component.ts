import { Component } from '@angular/core';
import { SesjonType } from 'src/app/models/api/SesjonType';

@Component({
  selector: 'app-etterlevelse-handsmykker-pdf',
  templateUrl: './etterlevelse-handsmykker-pdf.component.html'
})
  
export class EtterlevelseHandsmykkerPdfComponent {
  constructor(
    ) { }

  sesjonType = SesjonType.Handsmykker;
}
