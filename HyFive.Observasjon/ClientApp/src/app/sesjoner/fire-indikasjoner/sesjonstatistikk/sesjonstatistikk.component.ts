import {Component, Input, OnInit} from '@angular/core';
import {FireIndikasjonerSesjon} from '../../../models/api/FireIndikasjonerSesjon';
import {ActivityTypeConstants} from '../../../models/api/ActivityTypeConstants';

@Component({
  selector: 'app-sesjonstatistikk',
  templateUrl: './sesjonstatistikk.component.html'
})
export class SesjonstatistikkComponent implements OnInit {

  @Input() sesjon: FireIndikasjonerSesjon;

  constructor() { }

  ngOnInit(): void {
  }

  beregnAnledningerEtterlevd() : number{
    if(this.sesjon.observasjoner.length == 0)
      return 0;
    return this.sesjon.observasjoner.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotExecuted).length
  }

  beregnAnledningerEtterlevdProsent(): number {
    if (this.sesjon.observasjoner.length == 0)
      return 0;
    return (this.beregnAnledningerEtterlevd() / this.sesjon.observasjoner.length) * 100;
  }

  beregnAnledningerUtelatt() : number{
    if(this.sesjon.observasjoner.length == 0)
      return 0;
    return this.sesjon.observasjoner.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotExecuted).length
  }

  beregnAnledningerUtelattProsent() : number{
    if(this.sesjon.observasjoner.length == 0)
      return 0;
    return (this.beregnAnledningerUtelatt() / this.sesjon.observasjoner.length)*100
  }

}
