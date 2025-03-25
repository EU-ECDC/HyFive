import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AktivitetType } from '../../../models/api/AktivitetType';
import { AktivitettypeService } from '../../../services/data/aktivitettype.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-aktivitettype',
  templateUrl: './redigering-av-aktivitettype.component.html'
})
export class RedigeringAvAktivitettypeComponent implements OnInit, OnDestroy {

  aktivitettyper: AktivitetType[] = [];
  aktivitettypeSomEndres: AktivitetType = null;

  constructor(
    private aktivitettypeService: AktivitettypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastAktivitettype();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastAktivitettype() {
    this.aktivitettypeService.hentAktivitettyper().subscribe(
      (resultat) => this.aktivitettyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Aktivitettyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtAktivitettype(aktivitettype: AktivitetType): void {
    if (this.aktivitettypeSomEndres?.id == aktivitettype.id) return;
    this.aktivitettypeSomEndres = JSON.parse(JSON.stringify(aktivitettype));
  }

  lagreAktivitettype(aktivitettype: AktivitetType): void {
    this.aktivitettypeService.oppdaterAktivitettype(aktivitettype).subscribe(
      (oppdatertAktivitettype) => {
        this.toastrService.success("Aktivitettype oppdatert");
        this.lastAktivitettype();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Aktivitettype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.aktivitettypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.aktivitettypeSomEndres = null;
  }
}
