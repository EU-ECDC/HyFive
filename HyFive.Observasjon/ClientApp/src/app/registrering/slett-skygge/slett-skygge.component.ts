import { Component, Input, OnInit } from '@angular/core';
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-slett-skygge',
  templateUrl: './slett-skygge.component.html'
})
export class SlettSkyggeComponent implements OnInit {

  @Input("vis") vis: Boolean;

  faTrashAlt = faTrashAlt;

  constructor() { }

  ngOnInit(): void {
  }
}
