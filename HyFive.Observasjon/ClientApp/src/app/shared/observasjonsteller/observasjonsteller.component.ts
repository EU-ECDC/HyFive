import { Component, Input, OnInit } from '@angular/core';
import { MainMenuEventService } from '../../services/events/main-menu-event.service';
import { Sesjon } from '../../models/api/Sesjon';
import { faPlus, faClipboard, faCircle } from '@fortawesome/free-solid-svg-icons';
import { faObservasjonskort } from '../../utils/customIkoner';

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

  @Input() sesjonsdata: Sesjon<any>;
  @Input() url;
  constructor(private mainMenuEventService: MainMenuEventService) { }

  antallObservasjoner() : number {
    var antall =  this.sesjonsdata?.observasjoner?.length;
    if(antall != null)
    {
      return antall;
    }
    return 0;
  }
}
