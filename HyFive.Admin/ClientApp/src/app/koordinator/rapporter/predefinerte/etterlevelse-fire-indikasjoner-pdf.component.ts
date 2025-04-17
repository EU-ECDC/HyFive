import { Component } from '@angular/core';
import { SessionType } from 'src/app/models/api/SessionType';

@Component({
  selector: 'app-etterlevelse-fire-indikasjoner-pdf',
  templateUrl: './etterlevelse-fire-indikasjoner-pdf.component.html'
})

export class EtterlevelseFireIndikasjonerPdfComponent {
  constructor(
    ) { }

  sesjonType = SessionType.FireIndikasjoner;
}
