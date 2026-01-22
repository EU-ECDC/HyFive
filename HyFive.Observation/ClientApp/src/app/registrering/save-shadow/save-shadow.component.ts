import { Component, Input } from '@angular/core';
import { faSave } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-save-shadow',
  templateUrl: './save-shadow.component.html'
})
export class SaveShadowComponent {

  faSave = faSave;

  @Input() show: boolean;

  constructor() { }

}
