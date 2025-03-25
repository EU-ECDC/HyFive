import { Component, Input, OnInit } from '@angular/core';
import { faNyttKortPil } from '../../utils/customIkoner';

@Component({
  selector: 'app-nytt-kort-info',
  templateUrl: './nytt-kort-info.component.html'
})
export class NyttKortInfoComponent implements OnInit {

  constructor() { }

  faNyttKortPil = faNyttKortPil;

  @Input("visTomForKortTekst") visTomForKortTekst: boolean;

  ngOnInit(): void {
  }

}
