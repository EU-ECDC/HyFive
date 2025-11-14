import { Component, OnInit } from '@angular/core';
import { FiveIndicationsSessionService } from '../../services/data/five-indications-session.service';
import { FiveIndicationsSession } from '../../models/api/FiveIndicationsSession';
import { FiveIndicationsObservation } from '../../models/api/FiveIndicationsObservation';
import { ActivatedRoute, Router } from '@angular/router';
import { Urls } from '../../constants/urls';
import { Queryparameters } from '../../constants/queryparameters';
import { faArrowLeft, faTrashAlt } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { ToastrService } from 'ngx-toastr';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityType } from '../../models/api/ActivityType';
import { FiveIndicationsSessionView } from 'src/app/models/registration/FiveIndications-session-view.model';

@Component({
  selector: 'app-five-indications',
  templateUrl: './five-indications.component.html',
})
export class FiveIndicationsComponent implements OnInit {

  session: FiveIndicationsSession;
  sessionView: FiveIndicationsSessionView;
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
    private readonly sessionService: FiveIndicationsSessionService,
    private readonly activityService: ActivityService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.isOnline=true;
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.session = this.sessionService.getSession(sessionId);
        this.facilityid = this.session.department.facilityId;
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

  navigateToRegistrationPageForFiveIndications(sessionId: string){
    this.router.navigate([Urls.RegisterFiveIndicationsUrl], {queryParams: { sessionId: sessionId}})
  }

  observationDeletedEventHandler($event: FiveIndicationsObservation) {
    this.session = this.sessionService.getSession(this.session.id);
  }

  getIngress(observation: FiveIndicationsObservation) {
    return this.activityTypes?.find(x => x.code === observation.activity.activityType?.code)?.name + ' - ' + observation.indicationTypes.map(i => i.name).join(', ');
  }

  sendToCoordinator() {
    this.sessionSentToServer = true;
    this.sessionService.sendToServer(this.session.id).subscribe(res => {
        this.toastrService.success("Session was sent to coordinator");
        this.sessionService.deleteSession(this.session.id);
        this.sessionIsSentToServer = true;
      },
      error => {
        const message = "Something went wrong while sending session to coordinator: "+(error?.error ? error.error.substr(0, 300)+'...' : error);
        this.toastrService.error(message, '', { disableTimeOut: true});
      },
      () => this.sessionSentToServer = false);

  };

  navigateToSentSession() {
    this.router.navigate(['/'+Urls.SentFiveIndicationsSessionUrl], { queryParams: {sessionId: this.session.id}})
  }
}
