import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Aktivitet } from '../../models/api/Aktivitet';
import { ObservasjonEventService } from '../../services/events/observasjon-event.service';
import { faClock } from '@fortawesome/free-solid-svg-icons';
import { AktivitetType } from '../../models/api/AktivitetType';
import { AktivitetTypeKonstanter } from '../../models/api/AktivitetTypeKonstanter';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-registrer-aktivitet',
  templateUrl: './registrer-aktivitet.component.html'
})

export class RegistrerAktivitetComponent implements OnInit {

  timerErStartet: boolean = false;
  interval;
  tidtakingUtfores: boolean = false;
  faClock = faClock;
  visTekst: boolean = true;

  benyttetHansker?: boolean = null;

  @Input("parentId") parentId: string;
  @Input("tidtaking") tidtaking: boolean;
  @Input("deaktivert") deaktivert: boolean;
  @Input("aktivitetType") aktivitetType: AktivitetType;
  @Input("sekunder") sekunder: number;
  @Input("erRegistrert") erRegistrert: boolean;
  @Input("bekreftelseModalSkalVises") bekreftelseModalSkalVises: boolean;
  @Input("icon") icon: string;

  @Output() aktivitetRegistertEvent = new EventEmitter<Aktivitet>();

  constructor(private observasjonEventService: ObservasjonEventService, private modalService: NgbModal) {
  }

  ngOnInit(): void {
    if (this.sekunder && this.sekunder > 0) {
      this.visTekst = false;
    }
    this.observasjonEventService.observasjonNullstiltEvent.subscribe((parentId) => {
      if (parentId === this.parentId) {
        this.nullstillKomponent();
      }
    })
  }

  getAktivitetTekst() {
    if (this.aktivitetType?.kode === AktivitetTypeKonstanter.Desinfeksjon)
      return 'Sprit';
    if (this.aktivitetType?.kode === AktivitetTypeKonstanter.Handvask)
      return 'Vask';
    if(this.aktivitetType?.kode === AktivitetTypeKonstanter.IkkeUtfort)
      return 'Ikke utført';
  }

  registrerAktivitet(modalName) {

    if (this.bekreftelseModalSkalVises && !this.tidtakingUtfores) {
      this.modalService.open(modalName, { windowClass: 'hh-modal' });
    }
    else {
      if (this.tidtaking) {
        this.startEllerStoppTimer();
        this.visTekst = false;
      }
      else {
        this.aktivitetRegistertEvent.emit({ aktivitetType: this.aktivitetType, tidtakingBleUtfort: false });
      }
    }
  }

  bekrefteRegistrering(bleBekreftet: boolean) {

    this.benyttetHansker = bleBekreftet;

    if (this.tidtaking) {
      this.startEllerStoppTimer();
      this.visTekst = false;
    }
    else {
      this.aktivitetRegistertEvent.emit({ aktivitetType: this.aktivitetType, tidtakingBleUtfort: false, benyttetHanske: bleBekreftet});
    }
    
  }

  startEllerStoppTimer() {
    this.observasjonEventService.registreringAvAktivitetHarBegynt.emit({ parentId: this.parentId, aktivitetType: this.aktivitetType })
    if (this.timerErStartet === false) {
      this.startTimer();
      this.tidtakingUtfores = true;
    }
    else if (this.sekunder > 0) {
      this.stoppTimer();
      this.tidtakingUtfores = false;
      this.aktivitetRegistertEvent.emit({ aktivitetType: this.aktivitetType, sekunderBrukt: this.sekunder, tidtakingBleUtfort: true, benyttetHanske: this.benyttetHansker})
    }
  }

  private nullstillKomponent() {
    this.stoppTimer();
    this.sekunder = 0;
    this.visTekst = true;
    this.tidtakingUtfores = false;
  }

  private stoppTimer() {
    this.timerErStartet = false;
    clearInterval(this.interval);
    this.interval = 0;
  }

  private startTimer() {
    this.timerErStartet = true;
    this.sekunder = 0;
    this.interval = setInterval(() => {
      this.sekunder++;
    }, 1000);
  }
}
