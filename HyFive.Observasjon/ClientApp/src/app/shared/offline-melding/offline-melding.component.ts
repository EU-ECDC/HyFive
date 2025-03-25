import {Component, Output, EventEmitter, OnInit, Input} from '@angular/core';
import { Observable, Subscription, fromEvent } from 'rxjs';

@Component({
  selector: 'app-offline-melding',
  templateUrl: './offline-melding.component.html'
})

export class OfflineMeldingComponent implements OnInit {

  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  harInternett: boolean = false;
  // Settes til 'true' hvis offline-meldingen skal brukes på en mørk bakgrunn.
  @Input() darkmode;
  @Output() harInternettEvent: EventEmitter<boolean> = new EventEmitter<boolean>();


  ngOnInit() {
    setTimeout(() => {
      if(navigator.onLine)
      {
        this.harInternett = true;
      }

      this.harInternettEvent.emit(this.harInternett);

      this.offlineEvent = fromEvent(window, 'offline');
      this.onlineEvent = fromEvent(window, 'online');

      this.subscriptions.push(this.offlineEvent.subscribe(
        e => {
          this.harInternett = false;
          this.harInternettEvent.emit(this.harInternett);
        }
      ));

      this.subscriptions.push(this.onlineEvent.subscribe(
        e => {
          this.harInternett = true;
          this.harInternettEvent.emit(this.harInternett);
        }
      ))
    });
  }
}
