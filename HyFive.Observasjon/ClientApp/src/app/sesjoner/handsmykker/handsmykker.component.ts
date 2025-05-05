import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { HandJewelrySession } from '../../models/api/HandJewelrySession';
import { HandsmykkeSesjonService } from '../../services/data/handsmykke-sesjon.service';
import { faCircle, faAngleLeft, faClock, faClipboard, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { Urls } from '../../konstanter/urls';
import { FourIndicationsObservation } from '../../models/api/FourIndicationsObservation';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import { HandJewelryType } from 'src/app/models/api/HandJewelryType';
import { HandsmykkeMapper } from 'src/app/utils/handsmykke-mapper';
import { HandsmykkeTypeService } from '../../services/data/handsmykketype.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-handsmykker',
  templateUrl: './handsmykker.component.html'
})
export class HandsmykkerComponent implements OnInit {

  sesjon: HandJewelrySession;
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
  handJewelryTypes: HandJewelryType[] = [];

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
        const sessionId = params[Queryparameters.SesjonId] || 0;
        this.sesjon = this.sesjonService.hentSesjon(sessionId);
        if (!this.sesjon) this.router.navigate(['']);
      });
    this.handsmykkeTypeService.getHandsmykkeTyper().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
    });
  }
  
  sesjonSlettetEventHandler(id: string) {
    this.sesjonService.slettSesjon(id);
    this.router.navigate([Urls.IkkeSendteSesjonerUrl]);
  }

  navigerTilRegistreringssideForHandsmykker(sessionId: string) {
    this.router.navigate([Urls.RegistrereHandsmykkerUrl], { queryParams: { sessionId: sessionId } });
  }

  observasjonSlettetEventHandler($event: FourIndicationsObservation) {
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }

  visHandsmykker(handJewelry: HandJewelryType[]): string {
    return HandsmykkeMapper.getHandsmykkevalg(this.handJewelryTypes, handJewelry.map(x => x.code)).filter(h => h.isSelected == true).map(h => h.name).join(', ');
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
      () => {this.sesjonSendesTilServer = false; this.sesjonErSendtTilServer = true});
  };

  navigerTilSendtSesjon() {
    this.router.navigate(['/' + Urls.SendteHandsmykkeSesjonUrl], { queryParams: { sessionId: this.sesjon.id } })
  }
}
