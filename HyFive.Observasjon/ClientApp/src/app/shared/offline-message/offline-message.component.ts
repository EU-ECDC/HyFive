import {Component, Output, EventEmitter, OnInit, Input} from '@angular/core';
import { Observable, Subscription, fromEvent } from 'rxjs';

@Component({
  selector: 'app-offline-message',
  templateUrl: './offline-message.component.html'
})

export class OfflineMessageComponent implements OnInit {

  offlineEvent: Observable<Event>;
  onlineEvent: Observable<Event>;
  subscriptions: Subscription[] = [];
  hasInternet: boolean = false;
// Set to 'true' if the offline message should be used on a dark background.
  @Input() darkMode;
  @Output() hasInternetEvent: EventEmitter<boolean> = new EventEmitter<boolean>();


  ngOnInit() {
    setTimeout(() => {
      if(navigator.onLine)
      {
        this.hasInternet = true;
      }

      this.hasInternetEvent.emit(this.hasInternet);

      this.offlineEvent = fromEvent(window, 'offline');
      this.onlineEvent = fromEvent(window, 'online');

      this.subscriptions.push(this.offlineEvent.subscribe(
        e => {
          this.hasInternet = false;
          this.hasInternetEvent.emit(this.hasInternet);
        }
      ));

      this.subscriptions.push(this.onlineEvent.subscribe(
        e => {
          this.hasInternet = true;
          this.hasInternetEvent.emit(this.hasInternet);
        }
      ))
    });
  }
}
