import {Component, EventEmitter, Output} from '@angular/core';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
})
export class NavigationLinkComponent {

  faChevronLeft = faChevronLeft;
  @Output('linkClicked') linkClicked = new EventEmitter<any>();


  constructor() {

  }

}
