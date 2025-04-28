import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { HandJewelryType } from '../../../models/api/HandJewelryType';
import { HandJewelryTypeService } from '../../../services/data/handJewelryType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-handsmykketype',
  templateUrl: './redigering-av-handsmykketype.component.html'
})
export class RedigeringAvHandsmykketypeComponent implements OnInit, OnDestroy {

  handJewelryTypes: HandJewelryType[] = [];
  handJewelryTypeAsChanged: HandJewelryType = null;

  constructor(
    private handJewelryTypeService: HandJewelryTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.lastHandsmykketyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }


  lastHandsmykketyper() {
    this.handJewelryTypeService.getHandJewelryTypes().subscribe(
      (result) => this.handJewelryTypes = result,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Håndsmykketyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHandsmykketype(handsmykketype: HandJewelryType): void {
    if (this.handJewelryTypeAsChanged?.id == handsmykketype.id) return;
    this.handJewelryTypeAsChanged = JSON.parse(JSON.stringify(handsmykketype));
  }

  updateHandJewelryType(handsmykketype: HandJewelryType): void {
    this.handJewelryTypeService.updateHandJewelryType(handsmykketype).subscribe(
      (oppdatertHandsmykketype) => {
        this.toastrService.success("Håndsmykketype oppdatert");
        this.lastHandsmykketyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Håndsmykketype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.handJewelryTypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.handJewelryTypeAsChanged = null;
  }
}
