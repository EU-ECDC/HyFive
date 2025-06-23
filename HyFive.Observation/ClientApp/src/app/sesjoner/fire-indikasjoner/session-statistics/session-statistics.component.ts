import {Component, Input, OnInit} from '@angular/core';
import {FourIndicationsSession} from '../../../models/api/FourIndicationsSession';
import {ActivityTypeConstants} from '../../../models/api/ActivityTypeConstants';

@Component({
  selector: 'app-session-statistics',
  templateUrl: './session-statistics.component.html'
})
export class SessionStatisticsComponent implements OnInit {

  @Input() session: FourIndicationsSession;

  constructor() { }

  ngOnInit(): void {
  }

  calculateOccasionsComplied() : number{
    if(this.session.observations.length == 0)
      return 0;
    return this.session.observations.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotPerformed).length
  }

  calculateOccasionsCompliedPercent(): number {
    if (this.session.observations.length == 0)
      return 0;
    return (this.calculateOccasionsComplied() / this.session.observations.length) * 100;
  }

  calculateOccasionsOmitted() : number{
    if(this.session.observations.length == 0)
      return 0;
    return this.session.observations.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotPerformed).length
  }

  calculateOccasionsOmittedPercent() : number{
    if(this.session.observations.length == 0)
      return 0;
    return (this.calculateOccasionsOmitted() / this.session.observations.length)*100
  }

}
