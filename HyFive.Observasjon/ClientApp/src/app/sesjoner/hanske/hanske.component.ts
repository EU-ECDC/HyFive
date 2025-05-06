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
  selector: 'app-hanske',
  templateUrl: './hanske.component.html'
})
export class HanskeComponent implements OnInit {

  sesjon: GloveSession;
  sesjonErSendtTilServer = false;
  sesjonSendesTilServer = false;
  comment: string;
  isOnline: boolean = true;

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
        this.sesjon = this.sessionService.getSession(sessionId);
        if (!this.sesjon) this.router.navigate(['']);
      });
  }

 
  sesjonSlettetEventHandler(id: string) {
    this.sessionService.deleteSession(id);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  navigerTilRegistreringssideForHanske(sessionId: string) {
    this.router.navigate([Urls.RegisterGloveUrl], { queryParams: { sessionId: sessionId } });
  }

  observasjonSlettetEventHandler($event: GloveObservation) {
    this.sesjon = this.sessionService.getSession(this.sesjon.id);
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SentSessionsUrl]);
  }

  visIndikasjoner(item: GloveObservation): string { // TODO Velge mellom visning av indikasjoner/typer, eller vise om observation var with eller uten indikasjoner
    if (item.gloveWithIndicationTypes.length) return item.gloveWithIndicationTypes.map(x => x.name).join(', ');;
    return item.gloveWithoutIndicationTypes.map(x => x.name).join(', ');
  }

  sendTilKoordinator() {
    this.sesjonSendesTilServer = true;
    this.sessionService.sendToServer(this.sesjon.id).subscribe(res => {
      this.toastrService.success("Session ble sendt til koordinator");
      this.sessionService.deleteSession(this.sesjon.id);
      this.sesjonErSendtTilServer = true;
    },
      error => {
        const message = "Noe galt skjedde ved sending av sesjon til koordinator: "+(error?.error ? error.error.substr(0, 300)+'...' : error);
        this.toastrService.error(message, '', { disableTimeOut: true});
      },
      () => this.sesjonSendesTilServer = false);
  };

  navigerTilSendtSesjon() {
    this.router.navigate(['/' + Urls.SentGloveSessionUrl], { queryParams: { sessionId: this.sesjon.id } })
  }
}
