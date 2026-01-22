import { Component, Input } from '@angular/core';
import { faNewCardArrow } from '../../utils/customIcons';

@Component({
  selector: 'app-new-card-info',
  templateUrl: './new-card-info.component.html'
})
export class NewCardInfoComponent {

  constructor() { }

  faNewCardArrow = faNewCardArrow;

  @Input() showEmptyForShortText: boolean;

}
