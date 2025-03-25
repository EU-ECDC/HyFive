import { Component, OnInit, OnDestroy } from '@angular/core';
import { BeskyttelsesutstyrsettingType } from 'src/app/models/api/BeskyttelsesutstyrsettingType';
import { BeskyttelsesutstyrsettingtyperService } from '../../../services/data/beskyttelsesutstyrsettingtyper.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-beskyttelsesutstyrsettingtyper',
  templateUrl: './redigering-av-beskyttelsesutstyrsettingtyper.component.html'
})
export class RedigeringAvBeskyttelsesutstyrsettingtyperComponent implements OnInit, OnDestroy {

  beskyttelsesutstyrsettingTyper: BeskyttelsesutstyrsettingType[];
  settingTypeSomEndres: BeskyttelsesutstyrsettingType = null;

  constructor(
    private beskyttelsesutstyrsettingtyperService: BeskyttelsesutstyrsettingtyperService,
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
    this.beskyttelsesutstyrsettingtyperService.hentBeskyttelsesutstyrsettingtyper().subscribe(
      (settingtyper) => this.beskyttelsesutstyrsettingTyper = settingtyper,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av BeskyttelsesutstyrsettingTyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtSettingType(settingType: BeskyttelsesutstyrsettingType): void {
    if (this.settingTypeSomEndres?.id == settingType.id) return;
    this.settingTypeSomEndres = JSON.parse(JSON.stringify(settingType));
  }

  oppdaterSettingType(settingType: BeskyttelsesutstyrsettingType): void {
    this.beskyttelsesutstyrsettingtyperService.oppdaterBeskyttelsesutstyrsettingType(settingType).subscribe(
      (oppdatertBeskyttelsesutstyrsettingType) => {
        this.toastrService.success("BeskyttelsesutstyrsettingType ble oppdatert");
        this.lastSettingtyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av BeskyttelsesutstyrsettingType: ' + error?.error, '', { disableTimeOut: true}),
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
