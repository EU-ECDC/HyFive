import { Component, Input } from '@angular/core';
import { MainMenuEventService } from '../../services/events/main-menu-event.service';
import { Session } from '../../models/api/Session';
import { faPlus, faClipboard, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faObservationCard } from '../../utils/customIcons';

@Component({
  selector: 'app-observation-counter',
  templateUrl: './observation-counter.component.html'
})
export class ObservationCounterComponent {

  mainMenuIsOpen = false;

  faClipboard = faClipboard;
  faPlus = faPlus;
  faCircle = faCircle;
  faObservationCard = faObservationCard;

  @Input() sessionsdata: Session<any>;
  @Input() url;
  constructor(private readonly mainMenuEventService: MainMenuEventService) { }

  numberOfObservations() : number {
    let number =  this.sessionsdata?.observations?.length;
    if(number != null)
    {
      return number;
    }
    return 0;
  }
}
