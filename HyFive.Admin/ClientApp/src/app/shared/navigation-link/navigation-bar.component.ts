import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
})
export class NavigationLinkComponent implements OnInit {

  faChevronLeft = faChevronLeft;
  @Output('linkClicked') linkClicked = new EventEmitter<any>();


  constructor() {

  }

  ngOnInit(): void {
  }

}
