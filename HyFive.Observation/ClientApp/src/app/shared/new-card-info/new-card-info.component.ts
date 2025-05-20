import { Component, Input, OnInit } from '@angular/core';
import { faNewCardArrow } from '../../utils/customIconer';

@Component({
  selector: 'app-new-card-info',
  templateUrl: './new-card-info.component.html'
})
export class NewCardInfoComponent implements OnInit {

  constructor() { }

  faNewCardArrow = faNewCardArrow;

  @Input("showEmptyForShortText") showEmptyForShortText: boolean;

  ngOnInit(): void {
  }

}
