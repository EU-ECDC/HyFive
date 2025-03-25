import { Component, OnInit, OnDestroy } from '@angular/core';
import { HandhygieneEtterHanskebrukType } from '../../../models/api/HandhygieneEtterHanskebrukType';
import { ToastrService } from 'ngx-toastr';
import { HandhygieneEtterHanskebrukTypeService } from '../../../services/data/handhygieneetterhanskebruktype.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-handhygieneetterhanskebruktyper',
  templateUrl: './redigering-av-handhygieneetterhanskebruktyper.component.html'
})
export class RedigeringAvHandhygieneetterhanskebruktyperComponent implements OnInit, OnDestroy {

  handhygieneEtterHanskebrukTyper: HandhygieneEtterHanskebrukType[] = [];
  handhygieneEtterHanskebrukTypeSomEndres: HandhygieneEtterHanskebrukType = null;

  constructor(
    private handhygieneEtterHanskebrukTypeService: HandhygieneEtterHanskebrukTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastHandhygieneEtterHanskebrukTyper();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastHandhygieneEtterHanskebrukTyper() {
    this.handhygieneEtterHanskebrukTypeService.hentHandhygieneEtterHanskebrukTyper().subscribe(
      (resultat) => this.handhygieneEtterHanskebrukTyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av HandhygieneEtterHanskebrukTyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHandhygieneEtterHanskebrukType(handhygieneEtterHanskebrukType: HandhygieneEtterHanskebrukType): void {
    if (this.handhygieneEtterHanskebrukTypeSomEndres?.id == handhygieneEtterHanskebrukType.id) return;
    this.handhygieneEtterHanskebrukTypeSomEndres = JSON.parse(JSON.stringify(handhygieneEtterHanskebrukType));
  }

  oppdaterHandhygieneEtterHanskebrukType(): void {
    this.handhygieneEtterHanskebrukTypeService.oppdaterHandhygieneEtterHanskebrukType(this.handhygieneEtterHanskebrukTypeSomEndres).subscribe(
      (oppdaterHandhygieneEtterHanskebrukType) => {
        this.toastrService.success("HandhygieneEtterHanskebrukType oppdatert");
        this.lastHandhygieneEtterHanskebrukTyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av HandhygieneEtterHanskebrukType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.handhygieneEtterHanskebrukTypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.handhygieneEtterHanskebrukTypeSomEndres = null;
  }
}
