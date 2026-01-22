import {Component, EventEmitter, Input, Output} from '@angular/core';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import {MenuParameters} from '../../constants/menu-parameters';

@Component({
  selector: 'app-navigation-link',
  templateUrl: './navigation-link.component.html',
})
export class NavigationLinkComponent {

  faChevronLeft = faChevronLeft;
  @Input() mobileCssClass = 'mt-n5';
  @Output() linkClicked = new EventEmitter<any>();

  constructor() {

  }

  isMobileMenu(): boolean{
    return window.innerWidth < MenuParameters.mobileMenuWidth;
  }
}
