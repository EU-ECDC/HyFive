import { Component } from '@angular/core';
import { SesjonType } from 'src/app/models/api/SesjonType';

@Component({
  selector: 'app-etterlevelse-fire-indikasjoner-pdf',
  templateUrl: './etterlevelse-fire-indikasjoner-pdf.component.html'
})

export class EtterlevelseFireIndikasjonerPdfComponent {
  constructor(
    ) { }

  sesjonType = SesjonType.FireIndikasjoner;
}
