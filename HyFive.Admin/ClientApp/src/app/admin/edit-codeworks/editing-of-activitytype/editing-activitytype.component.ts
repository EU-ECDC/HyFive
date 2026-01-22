import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ActivityType } from '../../../models/api/ActivityType';
import { ActivityTypeService } from '../../../services/data/activity-type.service';
import { KeyEventService } from '../../../services/events/key-event.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-editing-activitytype',
  templateUrl: './editing-activitytype.component.html'
})
export class EditingActivityTypeComponent implements OnInit, OnDestroy {

  activityTypes: ActivityType[] = [];
  activitytypeAsChanged: ActivityType = null;

  constructor(
    private readonly activityTypeService: ActivityTypeService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService

  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadActivitytype();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadActivitytype() {
    this.activityTypeService.getActivityTypes().subscribe(
      (result) => this.activityTypes = result,
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Activity Types:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  selectedActivitytype(activitytype: ActivityType): void {
    if (this.activitytypeAsChanged?.id == activitytype.id) return;
    this.activitytypeAsChanged = structuredClone(activitytype);
  }

  saveActivitytype(activitytype: ActivityType): void {
    this.activityTypeService.updateActivityType(activitytype).subscribe(
      (oppdatertAktivitettype) => {
        this.toastrService.success(this.translate.instant("Activity Type updated"));
        this.loadActivitytype();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating Activitytype:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
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
