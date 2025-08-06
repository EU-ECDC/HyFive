import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { faCircle, faAngleLeft, faClock, faClipboard, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../constants/urls';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { GloveSession } from '../../models/api/GloveSession';
import { GloveSessionService } from '../../services/data/glove-session.service';
import { GloveObservation } from '../../models/api/GloveObservation';

@Component({
  selector: 'app-glove',
  templateUrl: './glove.component.html'
})
export class GloveComponent implements OnInit {

  session: GloveSession;
  sessionIsSentToServer = false;
  sessionSentToServer = false;
  comment: string;
  isOnline: boolean = true;
  institutionid: number;

  faCalendar = faCalendar;
  faAngleLeft = faAngleLeft;
  faClipboard = faClipboard;
  faCircle = faCircle;
  faClock = faClock;
  faAngleDown = faAngleDown;

  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private sessionService: GloveSessionService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.session = this.sessionService.getSession(sessionId);
        this.institutionid = this.session.department.institutionId;
        if (!this.session) this.router.navigate(['']);
      });
  }

 
  sessionDeletedEventHandler(id: string) {
    this.sessionService.deleteSession(id);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  navigateToRegistrationPageForGlove(sessionId: string) {
    this.router.navigate([Urls.RegisterGloveUrl], { queryParams: { sessionId: sessionId } });
  }

  observationDeletedEventHandler($event: GloveObservation) {
    this.session = this.sessionService.getSession(this.session.id);
  }

  navigateToSentSessions() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }

  showIndications(item: GloveObservation): string { // TODO Choose between displaying indications/types, or showing whether the observation was with or without indications
    if (item.gloveWithIndicationTypes.length) return item.gloveWithIndicationTypes.map(x => x.name).join(', ');;
    return item.gloveWithoutIndicationTypes.map(x => x.name).join(', ');
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
    this.router.navigate(['/' + Urls.SentGloveSessionUrl], { queryParams: { sessionId: this.session.id } })
  }
}
