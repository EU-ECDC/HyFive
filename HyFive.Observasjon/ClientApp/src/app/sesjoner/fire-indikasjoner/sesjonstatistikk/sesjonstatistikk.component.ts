import {Component, Input, OnInit} from '@angular/core';
import {FourIndicationsSession} from '../../../models/api/FourIndicationsSession';
import {ActivityTypeConstants} from '../../../models/api/ActivityTypeConstants';

@Component({
  selector: 'app-sesjonstatistikk',
  templateUrl: './sesjonstatistikk.component.html'
})
export class SesjonstatistikkComponent implements OnInit {

  @Input() session: FourIndicationsSession;

  constructor() { }

  ngOnInit(): void {
  }

  beregnAnledningerEtterlevd() : number{
    if(this.session.observations.length == 0)
      return 0;
    return this.session.observations.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotExecuted).length
  }

  beregnAnledningerEtterlevdProsent(): number {
    if (this.session.observations.length == 0)
      return 0;
    return (this.beregnAnledningerEtterlevd() / this.session.observations.length) * 100;
  }

  beregnAnledningerUtelatt() : number{
    if(this.session.observations.length == 0)
      return 0;
    return this.session.observations.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotExecuted).length
  }

  beregnAnledningerUtelattProsent() : number{
    if(this.session.observations.length == 0)
      return 0;
    return (this.beregnAnledningerUtelatt() / this.session.observations.length)*100
  }

}
