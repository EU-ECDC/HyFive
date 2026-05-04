import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservationOverviewReport} from "../../../models/api/ObservationOverviewReport";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Role} from "../../../models/api/Role";
import {ProtectiveEquipmentObservation} from "../../../models/api/ProtectiveEquipmentObservation";
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-edit-protective-equipment-observations',
  templateUrl: './edit-protective-equipment-observations.component.html'
})
export class EditProtectiveEquipmentObservationsComponent implements OnInit {

  @Input() observations: ObservationOverviewReport[]
  @Input() sessionId: string;
  @Input() departmentId: number;
  @Input() facilityId: number;
  @Input() canEdit = false;

  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  protectiveEquipmentObservationAsChanged: ProtectiveEquipmentObservation;
  canUpdateProtectiveEquipmentObservation = false;

  registeredDate: Date = null;


  constructor(
    private readonly observationService: ObservationService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly dialogMessageService: DialogMessageService,
    private readonly translate: TranslateService
  ) { }

  ngOnInit(): void {

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.protectiveEquipmentObservationAsChanged)
        this.cancelEditObservation(null)
    });
  }
  
  selectObservation(observation: ObservationOverviewReport) {

    if(!this.canEdit){
      return;
    }

    this.cancelEditObservation(null)
    this.protectiveEquipmentObservationAsChanged = observation.protectiveEquipmentObservation
  }

  changeComment(comment: string) {
    this.protectiveEquipmentObservationAsChanged.comment = comment;
  }

  updateRegistrationTime(dateString: string) {
    if (dateString) {
      this.registeredDate = new Date(dateString);
      this.protectiveEquipmentObservationAsChanged.registeredTime = this.registeredDate;
    }
  }

  updateProtectiveEquipmentObservation() {

    this.observationService.updateProtectiveEquipmentObservation(this.protectiveEquipmentObservationAsChanged).subscribe(
      (isUpdated) => {
        this.protectiveEquipmentObservationAsChanged = null;
        this.toastrService.success(this.translate.instant("The observation was updated"));
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when updating the observation"), { disableTimeOut: true});
      }
    );
  }

  deleteProtectiveEquipmentObservation() {
    this.observationService.deleteProtectiveEquipmentObservation(this.protectiveEquipmentObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.protectiveEquipmentObservationAsChanged = null;
        this.toastrService.success(this.translate.instant("The observation was deleted"));
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when deleting observation"), { disableTimeOut: true});
      });
  }

  cancelEditObservation(event: Event) {
    if(event) {
      event.stopPropagation();
    }

    this.protectiveEquipmentObservationAsChanged = null;
    this.canUpdateProtectiveEquipmentObservation = false;
  }

  selectRole(role: Role) {
    this.protectiveEquipmentObservationAsChanged.role = role;
  }

  updateObservationWithChangesFromMap($event: ProtectiveEquipmentObservation) {
    this.protectiveEquipmentObservationAsChanged = $event;
    this.canUpdateProtectiveEquipmentObservation = true;
  }
}
