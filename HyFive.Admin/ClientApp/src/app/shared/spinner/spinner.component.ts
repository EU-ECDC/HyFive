import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html'
})
export class SpinnerComponent {
  @Input() wrapperStyleClasses = 'd-flex justify-content-center pt-7';
}
