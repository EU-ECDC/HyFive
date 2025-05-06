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

@Component({
  selector: 'app-beskyttelsesutstyr',
  templateUrl: './beskyttelsesutstyr.component.html'
})
export class BeskyttelsesutstyrComponent implements OnInit {

  sesjon: ProtectiveEquipmentSession;
  sesjonErSendtTilServer = false;
  sesjonSendesTilServer = false;
  isOnline: boolean = true;
  institutionid: number;

  DialogueTexts = DialogueTexts;
  Urls = Urls;

  faCircle = faCircle;
  faClipboard = faClipboard;
  faClock = faClock;
  faCalendar = faCalendar;
  faAngleUp = faAngleUp;


  constructor(
    private sessionService: ProtectiveEquipmentSessionService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.sesjon = this.sessionService.getSession(sessionId);
        this.institutionid = this.sesjon.department.institutionId;
        if(!this.sesjon) this.router.navigate(['']);
      }
    );
  }

  navigerTilRegistreringssideForBeskyttelsesutstyr(sessionId: string){
    this.router.navigate([Urls.RegisterProtectiveEquipmentUrl], {queryParams: {sessionId: sessionId}});
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }

  sesjonSlettetEventHandler(sessionId: string) {
    this.sessionService.slettSesjon(sessionId);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  visUtstyr(protectiveEquipment: ProtectiveEquipment[]): string{
    if(protectiveEquipment?.length > 0){
      return protectiveEquipment.filter(b => b.wasUsed).map(b => b.equipmentType.name).join(', ');
    }
    return "";
  }

  observasjonSlettetEventHandler($event: ProtectiveEquipmentObservation) {
    this.sesjon = this.sessionService.getSession(this.sesjon.id);
  }

  sendTilKoordinator() {
    this.sesjonSendesTilServer = true;
    this.sessionService.sendToServer(this.sesjon.id).subscribe(res => {
        this.toastrService.success("Session ble sendt til koordinator");
        this.sessionService.slettSesjon(this.sesjon.id);
        this.sesjonErSendtTilServer = true;
        this.sesjonSendesTilServer = false;
      },
      error => {
        this.sesjonSendesTilServer = false;
        const message = "Noe galt skjedde ved sending av sesjon til koordinator: "+(error?.error ? error.error.substr(0, 300)+'...' : error);
        this.toastrService.error(message, '', { disableTimeOut: true});
      },
      () => this.sesjonSendesTilServer = false);
  };

  navigerTilSendtSesjon() {
    this.router.navigate(['/'+Urls.SendProtectiveEquipmentSessionUrl], { queryParams: {sessionId: this.sesjon.id}})
  }
}
