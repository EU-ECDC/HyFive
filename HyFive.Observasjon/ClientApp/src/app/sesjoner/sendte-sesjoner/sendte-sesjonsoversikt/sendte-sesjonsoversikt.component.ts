import { Component, Input } from "@angular/core";
import { FireIndikasjonerSesjon } from '../../../models/api/FireIndikasjonerSesjon';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-sendte-sesjonsoversikt',
  templateUrl: './sendte-sesjonsoversikt.component.html'
})

export class SendteSesjonsoversiktComponent {

  faCalendar = faCalendar;
  @Input("sesjon") sesjon: FireIndikasjonerSesjon;
}
