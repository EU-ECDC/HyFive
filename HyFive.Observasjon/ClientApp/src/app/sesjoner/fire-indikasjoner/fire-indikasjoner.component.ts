import { Component, OnInit } from '@angular/core';
import { FireIndikasjonerSesjonService } from '../../services/data/fire-indikasjoner-sesjon.service';
import { FireIndikasjonerSesjon } from '../../models/api/FireIndikasjonerSesjon';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { ActivatedRoute, Router } from '@angular/router';
import { Urls } from '../../konstanter/urls';
import { Queryparameters } from '../../konstanter/queryparameters';
import { faArrowLeft, faTrashAlt, faCircle } from '@fortawesome/free-solid-svg-icons';
import { Dialogtekster } from '../../konstanter/dialogtekster';
import { ToastrService } from 'ngx-toastr';
import { AktivitetService } from '../../services/data/aktivitet.service';
import { AktivitetType } from '../../models/api/AktivitetType';
import { FireIndikasjonerSesjonsvisning } from 'src/app/models/registrering/fire-indikasjoner-sesjonsvisning.model';

@Component({
  selector: 'app-fire-indikasjoner',
  templateUrl: './fire-indikasjoner.component.html',
})
export class FireIndikasjonerComponent implements OnInit {

  sesjon: FireIndikasjonerSesjon;
  sesjonvisning: FireIndikasjonerSesjonsvisning;
  sesjonErSendtTilServer = false;
  sesjonSendesTilServer = false;
  aktivitetTyper: AktivitetType[];
  erOnline: boolean;

  faArrowLeft = faArrowLeft;
  faTrashAlt = faTrashAlt;
  Dialogtekster = Dialogtekster;
  Urls = Urls;

  constructor(
    private sesjonService: FireIndikasjonerSesjonService,
    private aktivitetService: AktivitetService,
    private router: Router,
    private route: ActivatedRoute,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.erOnline=true;
    this.route
      .queryParams
      .subscribe(params => {
        const sesjonId = params[Queryparameters.SesjonId] || 0;
        this.sesjon = this.sesjonService.hentSesjon(sesjonId);
        this.sesjonvisning = this.sesjonService.hentSesjonsvisningForSesjon(sesjonId);
        if (!this.sesjon) this.router.navigate(['']);
      });
    this.aktivitetService.getAktivitetTyper().subscribe((aktivitetTyper) => {
      this.aktivitetTyper = aktivitetTyper;
    });
  }
  

  sesjonSlettetEventHandler(id: string) {
    this.sesjonService.slettSesjon(id);
    this.router.navigate([Urls.IkkeSendteSesjonerUrl]);
  }

  navigerTilRegistreringssideForFireIndikasjoner(sesjonId: string){
    this.router.navigate([Urls.RegistrereFireIndikasjonerUrl], {queryParams: { sesjonId: sesjonId}})
  }

  observasjonSlettetEventHandler($event: FireIndikasjonerObservasjon) {
    // Mulig TODO: pop observasjonen rett fra lista istedet for å laste på nytt fra LocalStorage
    this.sesjon = this.sesjonService.hentSesjon(this.sesjon.id);
  }

  hentIngress(observasjon: FireIndikasjonerObservasjon) {
    return this.aktivitetTyper?.find(x => x.kode === observasjon.aktivitet.aktivitetType?.kode)?.navn + ' - ' + observasjon.indikasjonstyper.map(i => i.navn).join(', ');
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
    this.router.navigate(['/'+Urls.SendteFireIndikasjonerSesjonUrl], { queryParams: {sesjonId: this.sesjon.id}})
  }
}
