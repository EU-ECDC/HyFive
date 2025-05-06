import { Component, Input, OnInit } from '@angular/core';
import { faSave } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-lagre-skygge',
  templateUrl: './lagre-skygge.component.html'
})
export class LagreSkyggeComponent implements OnInit {

  faSave = faSave;

  @Input("show") show: Boolean;

  constructor() { }

  ngOnInit(): void { }
}
