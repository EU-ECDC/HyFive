import { Component, Input } from "@angular/core";
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { HandHygieneSession } from "src/app/models/api/HandHygieneSession";

@Component({
  selector: 'app-sent-session-overview',
  templateUrl: './sent-session-overview.component.html'
})

export class SentSessionOverviewComponent {

  faCalendar = faCalendar;
  @Input() session: HandHygieneSession;
}
