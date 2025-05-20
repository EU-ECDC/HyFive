import { Component, OnInit } from '@angular/core';
import { FourIndicationsSessionService } from '../../services/data/four-indications-session.service';
import { FourIndicationsSession } from '../../models/api/FourIndicationsSession';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { ActivatedRoute, Router } from '@angular/router';
import { Urls } from '../../constants/urls';
import { Queryparameters } from '../../constants/queryparameters';
import { faArrowLeft, faTrashAlt, faCircle } from '@fortawesome/free-solid-svg-icons';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { ToastrService } from 'ngx-toastr';
import { ActivityService } from '../../services/data/activity.service';
import { ActivityType } from '../../models/api/ActivityType';
import { FourIndicationsSessionView } from 'src/app/models/registration/FourIndications-session-view.model';

@Component({
  selector: 'app-four-indications',
  templateUrl: './four-indications.component.html',
})
export class FourIndicationsComponent implements OnInit {

  session: FourIndicationsSession;
  sessionView: FourIndicationsSessionView;
  sessionIsSentToServer = false;
  sessionSentToServer = false;
  activityTypes: ActivityType[];
  isOnline: boolean;

  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private sessionService: FourIndicationsSessionService,
    private activityService: ActivityService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.isOnline=true;
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.session = this.sessionService.getSession(sessionId);
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

  navigateToRegistrationPageForFourIndications(sessionId: string){
    this.router.navigate([Urls.RegisterFourndicationsUrl], {queryParams: { sessionId: sessionId}})
  }

  observationDeletedEventHandler($event: FourIndicationsObservation) {
    // Mulig TODO: pop observasjonen rett fra lista istedet for å laste på nytt fra LocalStorage
    this.session = this.sessionService.getSession(this.session.id);
  }

  getIngress(observation: FourIndicationsObservation) {
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
    this.router.navigate(['/'+Urls.SentFourIndicationsSessionUrl], { queryParams: {sessionId: this.session.id}})
  }
}
