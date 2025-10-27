import {Component, Input, OnInit} from '@angular/core';
import {FiveIndicationsSession} from '../../../models/api/FiveIndicationsSession';
import {ActivityTypeConstants} from '../../../models/api/ActivityTypeConstants';

@Component({
  selector: 'app-session-statistics',
  templateUrl: './session-statistics.component.html'
})
export class SessionStatisticsComponent implements OnInit {

  @Input() session: FiveIndicationsSession;

  constructor() { }

  ngOnInit(): void {
  }

  calculateOpportunitiesComplied() : number{
    if(this.session.observations.length == 0)
      return 0;
    return this.session.observations.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotPerformed).length
  }

  calculateOpportunitiesCompliedPercent(): number {
    if (this.session.observations.length == 0)
      return 0;
    return (this.calculateOpportunitiesComplied() / this.session.observations.length) * 100;
  }

  calculateOpportunitiesOmitted() : number{
    if(this.session.observations.length == 0)
      return 0;
    return this.session.observations.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotPerformed).length
  }

  calculateOpportunitiesOmittedPercent() : number{
    if(this.session.observations.length == 0)
      return 0;
    return (this.calculateOpportunitiesOmitted() / this.session.observations.length)*100
  }

}
