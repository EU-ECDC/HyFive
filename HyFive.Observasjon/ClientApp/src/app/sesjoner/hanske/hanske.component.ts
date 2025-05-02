import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { faCircle, faAngleLeft, faClock, faClipboard, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../konstanter/urls';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { GloveSession } from '../../models/api/GloveSession';
import { HanskeSesjonService } from '../../services/data/hansker-sesjon.service';
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
  erOnline: boolean = true;

  faCalendar = faCalendar;
  faAngleLeft = faAngleLeft;
  faClipboard = faClipboard;
  faCircle = faCircle;
  faClock = faClock;
  faAngleDown = faAngleDown;

  Dialogtekster = Dialogtekster;
  Urls = Urls;

  constructor(
    private sesjonService: HanskeSesjonService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sessionId = params[Queryparameters.SesjonId] || 0;
        this.sesjon = this.sesjonService.hentSesjon(sessionId);
        if (!this.sesjon) this.router.navigate(['']);
      });
  }

 
  sesjonSlettetEventHandler(id: string) {
    this.sesjonService.slettSesjon(id);
    this.router.navigate([Urls.IkkeSendteSesjonerUrl]);
  }

  navigerTilRegistreringssideForHanske(sessionId: string) {
    this.router.navigate([Urls.RegistrereHanskeUrl], { queryParams: { sessionId: sessionId } });
  }

  observasjonSlettetEventHandler($event: GloveObservation) {
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }

  visIndikasjoner(item: GloveObservation): string { // TODO Velge mellom visning av indikasjoner/typer, eller vise om observasjon var med eller uten indikasjoner
    if (item.gloveWithIndicationTypes.length) return item.gloveWithIndicationTypes.map(x => x.name).join(', ');;
    return item.gloveWithoutIndicationTypes.map(x => x.name).join(', ');
  }

  sendTilKoordinator() {
    this.sesjonSendesTilServer = true;
    this.sesjonService.sendTilServer(this.sesjon.id).subscribe(res => {
      this.toastrService.success("Sesjon ble sendt til koordinator");
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
    this.router.navigate(['/' + Urls.SendteHanskeSesjonUrl], { queryParams: { sessionId: this.sesjon.id } })
  }
}
