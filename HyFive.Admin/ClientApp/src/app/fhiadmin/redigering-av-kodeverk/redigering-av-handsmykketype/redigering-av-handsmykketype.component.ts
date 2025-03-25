import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { HandsmykkeType } from '../../../models/api/HandsmykkeType';
import { HandsmykketypeService } from '../../../services/data/handsmykketype.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-handsmykketype',
  templateUrl: './redigering-av-handsmykketype.component.html'
})
export class RedigeringAvHandsmykketypeComponent implements OnInit, OnDestroy {

  handsmykketyper: HandsmykkeType[] = [];
  handsymkketypeSomEndres: HandsmykkeType = null;

  constructor(
    private handsmykketypeService: HandsmykketypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastHandsmykketyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }


  lastHandsmykketyper() {
    this.handsmykketypeService.hentHandsmykketyper().subscribe(
      (resultat) => this.handsmykketyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Håndsmykketyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHandsmykketype(handsmykketype: HandsmykkeType): void {
    if (this.handsymkketypeSomEndres?.id == handsmykketype.id) return;
    this.handsymkketypeSomEndres = JSON.parse(JSON.stringify(handsmykketype));
  }

  oppdaterHandsmykketype(handsmykketype: HandsmykkeType): void {
    this.handsmykketypeService.oppdaterHandsmykketype(handsmykketype).subscribe(
      (oppdatertHandsmykketype) => {
        this.toastrService.success("Håndsmykketype oppdatert");
        this.lastHandsmykketyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Håndsmykketype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.handsymkketypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.handsymkketypeSomEndres = null;
  }
}
