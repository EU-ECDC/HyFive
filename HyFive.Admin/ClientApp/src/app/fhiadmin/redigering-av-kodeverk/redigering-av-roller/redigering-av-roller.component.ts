import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Rolle } from '../../../models/api/Rolle';
import { RolleService } from '../../../services/data/rolle.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-roller',
  templateUrl: './redigering-av-roller.component.html'
})
export class RedigeringAvRollerComponent implements OnInit, OnDestroy {

  roller: Rolle[];

  rolleSomEndres: Rolle = null;
  nyRolle: Rolle = this.opprettTomRolle();
  deaktiverOpprett: boolean = true;


  constructor(
    private rolleService: RolleService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastRoller();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastRoller() {
    this.rolleService.hentRoller().subscribe(
      (roller) => this.roller = roller,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av roller: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  opprettTomRolle() {
    return {
      id: 0,
      navn: '',
      beskrivelse: '',
      institusjonIder: []
    } as Rolle;
  }

  opprettRolle() {
    this.rolleService.opprettRolle(this.nyRolle).subscribe(
      (opprettetRolle) => this.toastrService.success('Rolle opprettet'),
      error => this.toastrService.error('Det oppstod en feil under opprettelse av rolle: ' + error?.message, '', { disableTimeOut: true}),
      () => { this.nyRolle = this.opprettTomRolle(); this.lastRoller(); }
    );
  }

  valgtRolle(rolle: Rolle): void {
    if (this.rolleSomEndres?.id == rolle.id) return;
    this.rolleSomEndres = JSON.parse(JSON.stringify(rolle));
  }

  oppdaterRolle(rolle: Rolle) {
    this.rolleService.oppdaterRolle(rolle).subscribe(
      (oppdatertRolle) => {
        this.toastrService.success("Rolle oppdatert");
        this.lastRoller();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av rolle: ' + error?.error, '', { disableTimeOut: true}),
      () => this.rolleSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.rolleSomEndres = null;
  }
}
