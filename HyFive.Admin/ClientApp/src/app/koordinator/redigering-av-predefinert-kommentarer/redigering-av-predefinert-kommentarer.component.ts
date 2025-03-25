import { Component, OnInit, OnDestroy } from '@angular/core';
import { PredefinertKommentar } from '../../models/api/PredefinertKommentar';
import { PredefinertKommentarerService } from '../../services/data/predefinertkommentarer.service';
import { ToastrService } from 'ngx-toastr';
import { OpprettPredefinertKommentarRequest } from '../../models/api/OpprettPredefinertKommentarRequest';
import { KeyEventService } from '../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-predefinert-kommentarer',
  templateUrl: './redigering-av-predefinert-kommentarer.component.html'
})
export class RedigeringAvPredefinertKommentarerComponent implements OnInit, OnDestroy {

  nyPredefinertKommentar: OpprettPredefinertKommentarRequest = this.tomRequest();
  predefinertKommentarer: PredefinertKommentar[] = [];
  predefinertKommentarSomEndres: PredefinertKommentar = null;
  laster: boolean = false;

  constructor(
    private predefinertKommentarerService: PredefinertKommentarerService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.predefinertKommentarSomEndres = null;
    });

    this.lastPredefinertKommentarer();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastPredefinertKommentarer() {
    this.laster = true;
    this.predefinertKommentarerService.hentPredefinertKommentarer().subscribe(
      (predefinertKommentarer) => {
        this.laster = false;
        this.predefinertKommentarer = predefinertKommentarer
      },
      (error) => this.toastrService.error('Det oppstod en feil under innlasting av predefinerte kommentarer: ' + error?.message , '', { disableTimeOut: true}),
    );
  }

  tomRequest(): OpprettPredefinertKommentarRequest {
    return {
      kommentar: null
    }
  }

  opprettPredefinertKommentar() {
    this.predefinertKommentarerService.opprettPredefinertKommentar(this.nyPredefinertKommentar).subscribe(
      (opprettetPredefinertKommentar) => this.toastrService.success('Predefiner kommentar opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av predefinert kommentar: ' + error?.message , '', { disableTimeOut: true}),
      () => { this.nyPredefinertKommentar = this.tomRequest(); this.lastPredefinertKommentarer(); }
    );
  }

  valgtPredefinertKommentar(predefinertKommentar: PredefinertKommentar): void {
    if (this.predefinertKommentarSomEndres?.id == predefinertKommentar.id) return;
    this.predefinertKommentarSomEndres = JSON.parse(JSON.stringify(predefinertKommentar));
  }

  oppdaterPredefinertKommentar(predefinertKommentar: PredefinertKommentar): void {
    this.predefinertKommentarerService.oppdaterPredefinertKommentar(predefinertKommentar).subscribe(
      (oppdatertRolle) => {
        this.toastrService.success("Predefinert kommentar oppdatert");
        this.lastPredefinertKommentarer();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av predefinert kommentar: ' + error?.error , '', { disableTimeOut: true}),
      () => this.predefinertKommentarSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.predefinertKommentarSomEndres = null;
  }
}
