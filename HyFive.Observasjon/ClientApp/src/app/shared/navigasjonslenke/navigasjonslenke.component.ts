import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { faChevronLeft } from '@fortawesome/free-solid-svg-icons';
import {MenuParameters} from '../../konstanter/menu-parameters';

@Component({
  selector: 'app-navigasjonslenke',
  templateUrl: './navigasjonslenke.component.html',
})
export class NavigasjonslenkeComponent implements OnInit {

  faChevronLeft = faChevronLeft;
  @Input('mobileCssClass') mobileCssClass = 'mt-n5';
  @Output('lenkeKlikket') lenkeKlikket = new EventEmitter<any>();

  constructor() {

  }

  ngOnInit(): void {
  }

  isMobileMenu(): boolean{
    return window.innerWidth < MenuParameters.mobileMenuWidth;
  }
}
