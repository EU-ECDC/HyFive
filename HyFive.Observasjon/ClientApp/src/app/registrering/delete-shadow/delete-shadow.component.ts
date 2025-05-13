import { Component, Input, OnInit } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-delete-shadow',
  templateUrl: './delete-shadow.component.html'
})
export class DeleteShadowComponent implements OnInit {

  @Input("show") show: Boolean;

  faTrashAlt = faTrashAlt;

  constructor() { }

  ngOnInit(): void {
  }
}
