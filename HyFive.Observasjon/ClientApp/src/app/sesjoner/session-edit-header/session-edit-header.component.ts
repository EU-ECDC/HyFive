import {Component, Input, OnInit} from '@angular/core';
import {Session} from '../../models/api/Session';
import { faCircle, faClipboard } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-session-edit-header',
  templateUrl: './session-edit-header.component.html'
})
export class SessionEditHeaderComponent implements OnInit {

  faClipboard = faClipboard;
  faCircle = faCircle;

  @Input() session: Session<any>;
  @Input() sessiontype: string;
  @Input() sessionIsSentToServer: boolean;
  @Input() header: string;
  constructor() { }

  ngOnInit(): void {
  }
}
