import { Component, Input } from "@angular/core";
import { FourIndicationsSession } from '../../../models/api/FourIndicationsSession';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-sendte-sesjonsoversikt',
  templateUrl: './sendte-sesjonsoversikt.component.html'
})

export class SendteSesjonsoversiktComponent {

  faCalendar = faCalendar;
  @Input("sesjon") sesjon: FourIndicationsSession;
}
