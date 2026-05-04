import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../constants/queryparameters';
import { ProtectiveEquipmentSession } from '../../models/api/ProtectiveEquipmentSession';
import { ProtectiveEquipmentSessionService } from '../../services/data/protectiveEquipment-session.service';
import { Urls } from '../../constants/urls';
import { faCircle, faAngleUp, faClipboard, faClock } from '@fortawesome/free-solid-svg-icons';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import {DialogueTexts} from '../../constants/dialogueTexts';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import {ToastrService} from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-protection-equipment',
  templateUrl: './protection-equipment.component.html'
})
export class ProtectiveEquipmentComponent implements OnInit {

  session: ProtectiveEquipmentSession;
  sessionIsSentToServer = false;
  sessionSentToServer = false;
  isOnline: boolean = true;
  facilityid: number;

  DialogueTexts = DialogueTexts;
  Urls = Urls;

  faCircle = faCircle;
  faClipboard = faClipboard;
  faClock = faClock;
  faCalendar = faCalendar;
  faAngleUp = faAngleUp;


  constructor(
    private readonly sessionService: ProtectiveEquipmentSessionService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly toastrService: ToastrService,
    private readonly translate: TranslateService) {

  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.session = this.sessionService.getSession(sessionId);
        this.facilityid = this.session.facilityId;
        if(!this.session) this.router.navigate(['']);
      }
    );
  }

  navigateToProtectiveEquipmentRegistrationPage(sessionId: string){
    this.router.navigate([Urls.RegisterProtectiveEquipmentUrl], {queryParams: {sessionId: sessionId}});
  }

  navigateToSentSessions() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }

  sessionDeletedEventHandler(sessionId: string) {
    this.sessionService.deleteSession(sessionId);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  showEquipment(protectiveEquipment: ProtectiveEquipment[]): string{
    if(protectiveEquipment?.length > 0){
      return protectiveEquipment.filter(b => b.wasUsed).map(b => b.equipmentType.name).join(', ');
    }
    return "";
  }

  observationDeletedEventHandler($event: ProtectiveEquipmentObservation) {
    this.session = this.sessionService.getSession(this.session.id);
  }

  sendToCoordinator() {
    this.sessionSentToServer = true;
    this.sessionService.sendToServer(this.session.id).subscribe(res => {
      this.toastrService.success(this.translate.instant("Session was sent to coordinator"));
        this.sessionService.deleteSession(this.session.id);
        this.sessionIsSentToServer = true;
        this.sessionSentToServer = false;
      },
      error => {
        this.sessionSentToServer = false;
        const message = this.translate.instant("Something went wrong while sending session to coordinator:") + " " + (error?.error ? error.error.substr(0, 300)+'...' : error);
        this.toastrService.error(message, '', { disableTimeOut: true});
      },
      () => this.sessionSentToServer = false);
  };

  navigateToSentSession() {
    this.router.navigate(['/'+Urls.SendProtectiveEquipmentSessionUrl], { queryParams: {sessionId: this.session.id}})
  }
}
