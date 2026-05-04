import {Component, Input } from '@angular/core';
import {ActivityTypeConstants} from '../../../models/api/ActivityTypeConstants';
import { HandHygieneSession } from 'src/app/models/api/HandHygieneSession';

@Component({
  selector: 'app-session-statistics',
  templateUrl: './session-statistics.component.html'
})
export class SessionStatisticsComponent {

  @Input() session: HandHygieneSession;

  constructor() { }

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
