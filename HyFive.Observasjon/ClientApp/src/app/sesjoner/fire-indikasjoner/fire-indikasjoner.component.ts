import { Component, OnInit } from '@angular/core';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
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
  selector: 'app-fire-indikasjoner',
  templateUrl: './fire-indikasjoner.component.html',
})
export class FireIndikasjonerComponent implements OnInit {

  sesjon: FourIndicationsSession;
  sesjonvisning: FourIndicationsSessionView;
  sesjonErSendtTilServer = false;
  sesjonSendesTilServer = false;
  activityTypes: ActivityType[];
  erOnline: boolean;

  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  DialogueTexts = DialogueTexts;
  Urls = Urls;

  constructor(
    private sesjonService: FireIndikasjonerSesjonService,
    private activityService: ActivityService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.erOnline=true;
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SessionId] || 0;
        this.sesjon = this.sesjonService.hentSesjon(sessionId);
        this.sesjonvisning = this.sesjonService.hentSesjonsvisningForSesjon(sessionId);
        if (!this.sesjon) this.router.navigate(['']);
      });
    this.activityService.getActivityTypes().subscribe((activityTypes) => {
      this.activityTypes = activityTypes;
    });
  }
  

  sesjonSlettetEventHandler(id: string) {
    this.sesjonService.slettSesjon(id);
    this.router.navigate([Urls.NotSentSessionsUrl]);
  }

  navigerTilRegistreringssideForFireIndikasjoner(sessionId: string){
    this.router.navigate([Urls.RegisterFourndicationsUrl], {queryParams: { sessionId: sessionId}})
  }

  observasjonSlettetEventHandler($event: FourIndicationsObservation) {
    // Mulig TODO: pop observasjonen rett fra lista istedet for å laste på nytt fra LocalStorage
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  hentIngress(observasjon: FourIndicationsObservation) {
    return this.activityTypes?.find(x => x.code === observasjon.activity.activityType?.code)?.name + ' - ' + observasjon.indicationTypes.map(i => i.name).join(', ');
  }

  sendTilKoordinator() {
    this.sesjonSendesTilServer = true;
    this.sesjonService.sendTilServer(this.sesjon.id).subscribe(res => {
        this.toastrService.success("Session ble sendt til koordinator");
        this.sesjonService.slettSesjon(this.sesjon.id);
        this.sesjonErSendtTilServer = true;
      },
      error => {
        const message = "Noe galt skjedde ved sending av sesjon til koordinator: "+(error?.error ? error.error.substr(0, 300)+'...' : error);
        this.toastrService.error(message, '', { disableTimeOut: true});
      },
      () => this.sesjonSendesTilServer = false);

  };

  navigerTilSendtSesjon() {
    this.router.navigate(['/'+Urls.SentFourIndicationsSessionUrl], { queryParams: {sessionId: this.sesjon.id}})
  }
}
