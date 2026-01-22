import { Component, Input } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-delete-shadow',
  templateUrl: './delete-shadow.component.html'
})
export class DeleteShadowComponent {

  @Input() show: boolean;

  faTrashAlt = faTrashAlt;

  constructor() { }

}
