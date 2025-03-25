import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { faCircle, faAngleLeft, faClock, faClipboard, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../konstanter/urls';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { ToastrService } from 'ngx-toastr';
import { HanskeSesjon } from '../../models/api/HanskeSesjon';
import { HanskeSesjonService } from '../../services/data/hansker-sesjon.service';
import { HanskeObservasjon } from '../../models/api/HanskeObservasjon';

@Component({
  selector: 'app-hanske',
  templateUrl: './hanske.component.html'
})
export class HanskeComponent implements OnInit {

  sesjon: HanskeSesjon;
  sesjonErSendtTilServer = false;
  sesjonSendesTilServer = false;
  kommentar: string;
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
        const sesjonId = params[Queryparameters.SesjonId] || 0;
        this.sesjon = this.sesjonService.hentSesjon(sesjonId);
        if (!this.sesjon) this.router.navigate(['']);
      });
  }

 
  sesjonSlettetEventHandler(id: string) {
    this.sesjonService.slettSesjon(id);
    this.router.navigate([Urls.IkkeSendteSesjonerUrl]);
  }

  navigerTilRegistreringssideForHanske(sesjonId: string) {
    this.router.navigate([Urls.RegistrereHanskeUrl], { queryParams: { sesjonId: sesjonId } });
  }

  observasjonSlettetEventHandler($event: HanskeObservasjon) {
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }

  visIndikasjoner(item: HanskeObservasjon): string { // TODO Velge mellom visning av indikasjoner/typer, eller vise om observasjon var med eller uten indikasjoner
    if (item.hanskeMedIndikasjonTyper.length) return item.hanskeMedIndikasjonTyper.map(x => x.navn).join(', ');;
    return item.hanskeUtenIndikasjonTyper.map(x => x.navn).join(', ');
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
    this.router.navigate(['/' + Urls.SendteHanskeSesjonUrl], { queryParams: { sesjonId: this.sesjon.id } })
  }
}
