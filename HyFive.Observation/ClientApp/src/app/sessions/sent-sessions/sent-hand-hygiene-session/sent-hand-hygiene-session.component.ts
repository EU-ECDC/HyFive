import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faTrashAlt, faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from '../../../constants/dialogueTexts';
import { Queryparameters } from '../../../constants/queryparameters';
import { Urls } from '../../../constants/urls';
import { HandHygieneObservation } from '../../../models/api/HandHygieneObservation';
import { ActivityType } from '../../../models/api/ActivityType';
import { ActivityService } from '../../../services/data/activity.service';
import { ActivityTypeConstants } from 'src/app/models/api/ActivityTypeConstants';
import { SentSessionsService } from '../../../services/data/sent-sessions.service';
import {ToastrService} from 'ngx-toastr';
import { HandHygieneSession } from 'src/app/models/api/HandHygieneSession';

@Component({
  selector: 'app-sent-hand-hygiene-session',
  templateUrl: './sent-hand-hygiene-session.component.html',
})
export class SentHandHygieneSessionComponent implements OnInit, OnDestroy {

  session: HandHygieneSession;
  sessionIsSentToServer = false;
  activityTypes: ActivityType[];
  isOnline: boolean = true;
  downloadAsExcel = false;

  faArrowLeft = faLongArrowAltLeft;
  faTrashAlt = faTrashAlt;
  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private readonly sessionService: SentSessionsService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly activityService: ActivityService,
    private readonly toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        if(sessionId === 0) this.router.navigate(['']);
        this.sessionService.getHandHygieneSession(sessionId).subscribe(
          (session) => {
            this.session = session;
            if (!this.session) this.router.navigate(['']);
          }
        );
      });

      this.activityService.getActivityTypes().subscribe((activityTypes) => {
        this.activityTypes = activityTypes;
      });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  navigateToSentSessions(){
    this.router.navigate([Urls.SentSessionsUrl])
  }

  calculateOpportunitiesComplied(session: HandHygieneSession) : number{
    if(session?.observations?.length == 0)
      return 0;
    return session?.observations?.filter(f => f.activity.activityType?.code != ActivityTypeConstants.NotPerformed).length
  }

  calculateOpportunitiesCompliedPercent(session: HandHygieneSession): number {
    if (session?.observations?.length == 0)
      return 0;
    return (this.calculateOpportunitiesComplied(session) / session?.observations?.length) * 100;
  }

  calculateOpportunitiesOmitted(session: HandHygieneSession) : number{
    if(session?.observations?.length == 0)
      return 0;
    return session?.observations?.filter(f => f.activity.activityType?.code == ActivityTypeConstants.NotPerformed).length
  }

  calculateOpportunitiesOmittedPercent(session: HandHygieneSession) : number{
    if(session?.observations?.length == 0)
      return 0;
    return (this.calculateOpportunitiesOmitted(session) / session?.observations?.length)*100
  }

  getIngress(observation: HandHygieneObservation) {
    return this.activityTypes?.find(x => x.code === observation.activity.activityType?.code)?.name + ' - ' + observation.indicationTypes.map(i => i.name).join(', ');
  }

  receivedInternetStatus(hasInternet: boolean){
    this.isOnline = hasInternet;
  }

  downloadAsExcelFnct() {
    this.downloadAsExcel = true;
    this.sessionService.downloadHandHygieneSessionAsExcel(this.session.facilityId, this.session.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, DialogueTexts.ErrorDuringDownloadSessionExcel, {disableTimeOut: true}),
      () => this.downloadAsExcel = false)
  }
}
