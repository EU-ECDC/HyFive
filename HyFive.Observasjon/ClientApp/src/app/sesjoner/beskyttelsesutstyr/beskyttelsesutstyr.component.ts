import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../konstanter/queryparameters';
import { BeskyttelsesutstyrSesjon } from '../../models/api/BeskyttelsesutstyrSesjon';
import { BeskyttelsesutstyrSesjonService } from '../../services/data/beskyttelsesutstyr-sesjon.service';
import { Urls } from '../../konstanter/urls';
import { faCircle, faAngleUp, faClipboard, faClock } from '@fortawesome/free-solid-svg-icons';
import { faCalendar } from '@fortawesome/free-regular-svg-icons';
import {Dialogtekster} from '../../konstanter/dialogtekster';
import { ProtectiveEquipment } from '../../models/api/ProtectiveEquipment';
import { ProtectiveEquipmentObservation } from '../../models/api/ProtectiveEquipmentObservation';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-beskyttelsesutstyr',
  templateUrl: './beskyttelsesutstyr.component.html'
})
export class BeskyttelsesutstyrComponent implements OnInit {

  sesjon: BeskyttelsesutstyrSesjon;
  sesjonErSendtTilServer = false;
  sesjonSendesTilServer = false;
  erOnline: boolean = true;
  institusjonid: number;

  Dialogtekster = Dialogtekster;
  Urls = Urls;

  faCircle = faCircle;
  faClipboard = faClipboard;
  faClock = faClock;
  faCalendar = faCalendar;
  faAngleUp = faAngleUp;


  constructor(
    private sesjonService: BeskyttelsesutstyrSesjonService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(
      params => {
        const sesjonId = params[Queryparameters.SesjonId] || 0;
        this.sesjon = this.sesjonService.hentSesjon(sesjonId);
        this.institusjonid = this.sesjon.department.institusjonId;
        if(!this.sesjon) this.router.navigate(['']);
      }
    );
  }

  navigerTilRegistreringssideForBeskyttelsesutstyr(sesjonId: string){
    this.router.navigate([Urls.RegistrereBeskyttelsesutstyrUrl], {queryParams: {sesjonId: sesjonId}});
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }

  sesjonSlettetEventHandler(sesjonId: string) {
    this.sesjonService.slettSesjon(sesjonId);
    this.router.navigate([Urls.IkkeSendteSesjonerUrl]);
  }

  visUtstyr(beskyttelsesutstyr: ProtectiveEquipment[]): string{
    if(beskyttelsesutstyr?.length > 0){
      return beskyttelsesutstyr.filter(b => b.wasUsed).map(b => b.equipmentType.name).join(', ');
    }
    return "";
  }

  observasjonSlettetEventHandler($event: ProtectiveEquipmentObservation) {
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  sendTilKoordinator() {
    this.sesjonSendesTilServer = true;
    this.sesjonService.sendTilServer(this.sesjon.id).subscribe(res => {
        this.toastrService.success("Sesjon ble sendt til koordinator");
        this.sesjonService.slettSesjon(this.sesjon.id);
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
    this.router.navigate(['/'+Urls.SendteBeskyttelsesutstyrSesjonUrl], { queryParams: {sesjonId: this.sesjon.id}})
  }
}
