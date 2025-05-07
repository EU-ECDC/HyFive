import {Component, Input, OnInit} from '@angular/core';
import {Session} from '../../models/api/Session';
import { faCircle, faClipboard } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-sesjonsredigering-overskrift',
  templateUrl: './sesjonsredigering-overskrift.component.html'
})
export class SesjonsredigeringOverskriftComponent implements OnInit {

  faClipboard = faClipboard;
  faCircle = faCircle;

  @Input() session: Session<any>;
  @Input() sessiontype: string;
  @Input() sessionIsSentToServer: boolean;
  @Input() overskrift: string;
  constructor() { }

  ngOnInit(): void {
  }
}
