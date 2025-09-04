import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { HandJewelrySession } from '../../models/api/HandJewelrySession';
import { HandJewelrySessionService } from '../../services/data/hand-Jewelry-session.service';
import { faCircle, faAngleLeft, faClock, faClipboard, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../constants/urls';
import { FiveIndicationsObservation } from '../../models/api/FiveIndicationsObservation';
import { DialogueTexts } from '../../constants/dialogueTexts';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { HandJewelryType } from 'src/app/models/api/HandJewelryType';
import { HandJewelryMapper } from 'src/app/utils/handJewelry-mapper';
import { HandJewelryTypeService } from '../../services/data/hand-jewelry-type.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-handJewelry',
  templateUrl: './handJewelry.component.html'
})
export class HandJewelryComponent implements OnInit {

  session: HandJewelrySession;
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
  handJewelryTypes: HandJewelryType[] = [];

  constructor(
    private sessionService: HandJewelrySessionService,
    private handJewelryTypeService: HandJewelryTypeService,
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
    this.handJewelryTypeService.getHandJewelryTypes().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
    });
  }
  
  sessionDeletedEventHandler(id: string) {
    this.sessionService.deleteSession(id);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  navigateToRegistrationPageForHandJewelry(sessionId: string) {
    this.router.navigate([Urls.RegisterHandJewelryUrl], { queryParams: { sessionId: sessionId } });
  }

  observationDeletedEventHandler($event: FiveIndicationsObservation) {
    this.session = this.sessionService.getSession(this.session.id);
  }

  navigateToSentSessions() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }

  showHandJewelry(handJewelry: HandJewelryType[]): string {
    return HandJewelryMapper.getHandjewelrySelection(this.handJewelryTypes, handJewelry.map(x => x.code)).filter(h => h.isSelected == true).map(h => h.name).join(', ');
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
      () => {this.sessionSentToServer = false; this.sessionIsSentToServer = true});
  };

  navigateToSentSession() {
    this.router.navigate(['/' + Urls.SentHandJewelrySessionUrl], { queryParams: { sessionId: this.session.id } })
  }
}
