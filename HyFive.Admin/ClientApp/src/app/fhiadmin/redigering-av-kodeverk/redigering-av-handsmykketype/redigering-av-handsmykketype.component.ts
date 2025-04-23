import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BraceletType } from '../../../models/api/BraceletType';
import { HandJewelryTypeService } from '../../../services/data/handJewelryType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-handsmykketype',
  templateUrl: './redigering-av-handsmykketype.component.html'
})
export class RedigeringAvHandsmykketypeComponent implements OnInit, OnDestroy {

  handsmykketyper: BraceletType[] = [];
  handsymkketypeSomEndres: BraceletType = null;

  constructor(
    private handsmykketypeService: HandJewelryTypeService,
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
    this.handsmykketypeService.getHandJewelryTypes().subscribe(
      (resultat) => this.handsmykketyper = resultat,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Håndsmykketyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHandsmykketype(handsmykketype: BraceletType): void {
    if (this.handsymkketypeSomEndres?.id == handsmykketype.id) return;
    this.handsymkketypeSomEndres = JSON.parse(JSON.stringify(handsmykketype));
  }

  updateHandJewelryType(handsmykketype: BraceletType): void {
    this.handsmykketypeService.updateHandJewelryType(handsmykketype).subscribe(
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
