import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ActivityType } from '../../../models/api/ActivityType';
import { ActivityTypeService } from '../../../services/data/activity-type.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-aktivitettype',
  templateUrl: './redigering-av-aktivitettype.component.html'
})
export class RedigeringAvAktivitettypeComponent implements OnInit, OnDestroy {

  activityTypes: ActivityType[] = [];
  activitytypeAsChanged: ActivityType = null;

  constructor(
    private activityTypeService: ActivityTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.lastAktivitettype();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastAktivitettype() {
    this.activityTypeService.getActivityTypes().subscribe(
      (result) => this.activityTypes = result,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av Aktivitettyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtAktivitettype(aktivitettype: ActivityType): void {
    if (this.activitytypeAsChanged?.id == aktivitettype.id) return;
    this.activitytypeAsChanged = JSON.parse(JSON.stringify(aktivitettype));
  }

  lagreAktivitettype(aktivitettype: ActivityType): void {
    this.activityTypeService.updateActivityType(aktivitettype).subscribe(
      (oppdatertAktivitettype) => {
        this.toastrService.success("Aktivitettype oppdatert");
        this.lastAktivitettype();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av Aktivitettype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.activitytypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.activitytypeAsChanged = null;
  }
}
