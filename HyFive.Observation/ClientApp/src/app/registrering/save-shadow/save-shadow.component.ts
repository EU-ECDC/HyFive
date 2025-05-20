import { Component, Input, OnInit } from '@angular/core';
import { faSave } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-save-shadow',
  templateUrl: './save-shadow.component.html'
})
export class SaveShadowComponent implements OnInit {

  faSave = faSave;

  @Input("show") show: Boolean;

  constructor() { }

  ngOnInit(): void { }
}
