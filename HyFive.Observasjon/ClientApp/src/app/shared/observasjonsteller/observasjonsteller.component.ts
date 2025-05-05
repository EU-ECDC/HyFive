import { Component, Input, OnInit } from '@angular/core';
import { MainMenuEventService } from '../../services/events/main-menu-event.service';
import { Session } from '../../models/api/Session';
import { faPlus, faClipboard, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faObservasjonskort } from '../../utils/customIconer';

@Component({
  selector: 'app-observasjonsteller',
  templateUrl: './observasjonsteller.component.html'
})
export class ObservasjonstellerComponent {

  mainMenuIsOpen = false;

  faClipboard = faClipboard;
  faPlus = faPlus;
  faCircle = faCircle;
  faObservasjonskort = faObservasjonskort;

  @Input() sessionsdata: Session<any>;
  @Input() url;
  constructor(private mainMenuEventService: MainMenuEventService) { }

  numberOfObservations() : number {
    var antall =  this.sessionsdata?.observations?.length;
    if(antall != null)
    {
      return antall;
    }
    return 0;
  }
}
