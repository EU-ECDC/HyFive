import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { HandsmykkeSesjon } from '../../models/api/HandsmykkeSesjon';
import { HandsmykkeSesjonService } from '../../services/data/handsmykke-sesjon.service';
import { faCircle, faAngleLeft, faClock, faClipboard, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../konstanter/urls';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { HandsmykkeType } from 'src/app/models/api/HandsmykkeType';
import { HandsmykkeMapper } from 'src/app/utils/handsmykke-mapper';
import { HandsmykkeTypeService } from '../../services/data/handsmykketype.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-handsmykker',
  templateUrl: './handsmykker.component.html'
})
export class HandsmykkerComponent implements OnInit {

  sesjon: HandsmykkeSesjon;
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
  handsmykkeTyper: HandsmykkeType[] = [];

  constructor(
    private sesjonService: HandsmykkeSesjonService,
    private handsmykkeTypeService: HandsmykkeTypeService,
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
    this.handsmykkeTypeService.getHandsmykkeTyper().subscribe((handsmykkeTyper) => {
      this.handsmykkeTyper = handsmykkeTyper;
    });
  }
  
  sesjonSlettetEventHandler(id: string) {
    this.sesjonService.slettSesjon(id);
    this.router.navigate([Urls.IkkeSendteSesjonerUrl]);
  }

  navigerTilRegistreringssideForHandsmykker(sesjonId: string) {
    this.router.navigate([Urls.RegistrereHandsmykkerUrl], { queryParams: { sesjonId: sesjonId } });
  }

  observasjonSlettetEventHandler($event: FireIndikasjonerObservasjon) {
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }

  visHandsmykker(handsmykker: HandsmykkeType[]): string {
    return HandsmykkeMapper.getHandsmykkevalg(this.handsmykkeTyper, handsmykker.map(x => x.kode)).filter(h => h.erValgt == true).map(h => h.navn).join(', ');
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
      () => {this.sesjonSendesTilServer = false; this.sesjonErSendtTilServer = true});
  };

  navigerTilSendtSesjon() {
    this.router.navigate(['/' + Urls.SendteHandsmykkeSesjonUrl], { queryParams: { sesjonId: this.sesjon.id } })
  }
}
