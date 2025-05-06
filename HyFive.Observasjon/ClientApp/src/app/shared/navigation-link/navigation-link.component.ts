import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import {MenuParameters} from '../../constants/menu-parameters';

@Component({
  selector: 'app-navigation-link',
  templateUrl: './navigation-link.component.html',
})
export class NavigationLinkComponent implements OnInit {

  faChevronLeft = faChevronLeft;
  @Input('mobileCssClass') mobileCssClass = 'mt-n5';
  @Output('linkClicked') linkClicked = new EventEmitter<any>();

  constructor() {

  }

  ngOnInit(): void {
  }

  isMobileMenu(): boolean{
    return window.innerWidth < MenuParameters.mobileMenuWidth;
  }
}
