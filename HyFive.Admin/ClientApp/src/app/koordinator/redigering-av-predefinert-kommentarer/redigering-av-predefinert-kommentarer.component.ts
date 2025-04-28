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

  nyPredefinertKommentar: OpprettPredefinertKommentarRequest = this.emptyRequest();
  predefinertKommentarer: PredefinertKommentar[] = [];
  predefinertKommentarSomEndres: PredefinertKommentar = null;
  loading: boolean = false;

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
    this.loading = true;
    this.predefinertKommentarerService.hentPredefinertKommentarer().subscribe(
      (predefinertKommentarer) => {
        this.loading = false;
        this.predefinertKommentarer = predefinertKommentarer
      },
      (error) => this.toastrService.error('Det oppstod en feil under innlasting av predefinerte kommentarer: ' + error?.message , '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): OpprettPredefinertKommentarRequest {
    return {
      kommentar: null
    }
  }

  opprettPredefinertKommentar() {
    this.predefinertKommentarerService.opprettPredefinertKommentar(this.nyPredefinertKommentar).subscribe(
      (opprettetPredefinertKommentar) => this.toastrService.success('Predefiner kommentar opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av predefinert kommentar: ' + error?.message , '', { disableTimeOut: true}),
      () => { this.nyPredefinertKommentar = this.emptyRequest(); this.lastPredefinertKommentarer(); }
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
      error => this.toastrService.error('An error occurred while updating predefinert kommentar: ' + error?.error , '', { disableTimeOut: true}),
      () => this.predefinertKommentarSomEndres = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.predefinertKommentarSomEndres = null;
  }
}
