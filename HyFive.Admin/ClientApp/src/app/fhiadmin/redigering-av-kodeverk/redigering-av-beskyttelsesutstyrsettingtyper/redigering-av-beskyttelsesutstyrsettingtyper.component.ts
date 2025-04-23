import { Component, OnInit, OnDestroy } from '@angular/core';
import { ProtectiveEquipmentType } from 'src/app/models/api/ProtectiveEquipmentType';
import { ProtectiveEquipmentSettingTypesService } from '../../../services/data/protectiveEquipmentSettingTypes.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-beskyttelsesutstyrsettingtyper',
  templateUrl: './redigering-av-beskyttelsesutstyrsettingtyper.component.html'
})
export class RedigeringAvBeskyttelsesutstyrsettingtyperComponent implements OnInit, OnDestroy {

  beskyttelsesutstyrsettingTyper: ProtectiveEquipmentType[];
  settingTypeSomEndres: ProtectiveEquipmentType = null;

  constructor(
    private protectiveEquipmentSettingTypesService: ProtectiveEquipmentSettingTypesService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.avbrytRedigering();
    });

    this.lastSettingtyper();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastSettingtyper() {
    this.protectiveEquipmentSettingTypesService.getProtectiveEquipmentTypes().subscribe(
      (settingtyper) => this.beskyttelsesutstyrsettingTyper = settingtyper,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av BeskyttelsesutstyrsettingTyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtSettingType(settingType: ProtectiveEquipmentType): void {
    if (this.settingTypeSomEndres?.id == settingType.id) return;
    this.settingTypeSomEndres = JSON.parse(JSON.stringify(settingType));
  }

  oppdaterSettingType(settingType: ProtectiveEquipmentType): void {
    this.protectiveEquipmentSettingTypesService.updateProtectiveEquipmentSettingType(settingType).subscribe(
      (oppdatertBeskyttelsesutstyrsettingType) => {
        this.toastrService.success("ProtectiveEquipmentType ble oppdatert");
        this.lastSettingtyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av ProtectiveEquipmentType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.settingTypeSomEndres = null
    );
  }

  avbrytRedigering($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.settingTypeSomEndres = null;
  }
}
