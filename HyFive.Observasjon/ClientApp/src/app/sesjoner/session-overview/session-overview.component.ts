import { Component, OnInit } from '@angular/core';
import { Urls } from '../../constants/urls';

@Component({
  selector: 'app-session-overview',
  templateUrl: './session-overview.component.html'
})
export class SessionOverviewComponent implements OnInit {

  Urls = Urls;

  constructor() { }

  ngOnInit(): void {
  }

}
