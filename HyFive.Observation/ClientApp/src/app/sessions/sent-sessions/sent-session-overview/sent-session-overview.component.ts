import { Component, Input } from "@angular/core";
import { FiveIndicationsSession } from '../../../models/api/FiveIndicationsSession';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-sent-session-overview',
  templateUrl: './sent-session-overview.component.html'
})

export class SentSessionOverviewComponent {

  faCalendar = faCalendar;
  @Input() session: FiveIndicationsSession;
}
