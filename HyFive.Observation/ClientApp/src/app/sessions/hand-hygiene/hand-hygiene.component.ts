import { Component, OnInit } from '@angular/core';
import { HandHygieneObservation } from '../../models/api/HandHygieneObservation';
import { ActivatedRoute, Router } from '@angular/router';
import { Urls } from '../../constants/urls';
import { Queryparameters } from '../../constants/queryparameters';
import { faArrowLeft, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { ToastrService } from 'ngx-toastr';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityType } from '../../models/api/ActivityType';
import { TranslateService } from '@ngx-translate/core';
import { HandHygieneSessionView } from 'src/app/models/registration/hand-hygiene-session-view.model';
import { HandHygieneSessionService } from 'src/app/services/data/hand-hygiene-session.service';
import { HandHygieneSession } from 'src/app/models/api/HandHygieneSession';

@Component({
  selector: 'app-hand-hygiene',
  templateUrl: './hand-hygiene.component.html',
})
export class HandHygieneComponent implements OnInit {

  session: HandHygieneSession;
  sessionView: HandHygieneSessionView;
  sessionIsSentToServer = false;
  sessionSentToServer = false;
  activityTypes: ActivityType[];
  isOnline: boolean;
  facilityid: number;

  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private readonly sessionService: HandHygieneSessionService,
    private readonly activityService: ActivityService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly toastrService: ToastrService,
  private readonly translate: TranslateService) {

  }

  ngOnInit(): void {
    this.isOnline=true;
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.session = this.sessionService.getSession(sessionId);
        this.facilityid = this.session.facilityId;
        this.sessionView = this.sessionService.getSessionViewForSession(sessionId);
        if (!this.session) this.router.navigate(['']);
      });
    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });
  }
  

  sessionDeletedEventHandler(id: string) {
    this.sessionService.deleteSession(id);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  navigateToRegistrationPageForHandHygiene(sessionId: string){
    this.router.navigate([Urls.RegisterHandHygieneUrl], {queryParams: { sessionId: sessionId}})
  }

  observationDeletedEventHandler($event: HandHygieneObservation) {
    this.session = this.sessionService.getSession(this.session.id);
  }

  getIngress(observation: HandHygieneObservation) {
    return this.activityTypes?.find(x => x.code === observation.activity.activityType?.code)?.name + ' - ' + observation.indicationTypes.map(i => i.name).join(', ');
  }

  sendToCoordinator() {
    this.sessionSentToServer = true;
    this.sessionService.sendToServer(this.session.id).subscribe(res => {
        this.toastrService.success(this.translate.instant("Session was sent to coordinator"));
        this.sessionService.deleteSession(this.session.id);
        this.sessionIsSentToServer = true;
      },
      error => {
        const message = this.translate.instant("Something went wrong while sending session to coordinator") + ": "+(error?.error ? error.error.substr(0, 300)+'...' : error);
        this.toastrService.error(message, '', { disableTimeOut: true});
      },
      () => this.sessionSentToServer = false);

  };

  navigateToSentSession() {
    this.router.navigate(['/'+Urls.SentHandHygieneSessionUrl], { queryParams: {sessionId: this.session.id}})
  }
}
