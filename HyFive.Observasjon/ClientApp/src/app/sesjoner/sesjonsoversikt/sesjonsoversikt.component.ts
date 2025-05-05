import { Component, OnInit } from '@angular/core';
import { Urls } from '../../constants/urls';

@Component({
  selector: 'app-sesjonsoversikt',
  templateUrl: './sesjonsoversikt.component.html'
})
export class SesjonsoversiktComponent implements OnInit {

  Urls = Urls;

  constructor() { }

  ngOnInit(): void {
  }

}
