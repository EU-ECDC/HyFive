import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservationOverviewReport} from "../../../models/api/ObservationOverviewReport";
import {Department} from "../../../models/api/Department";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Role} from "../../../models/api/Role";
import {ProtectiveEquipmentObservation} from "../../../models/api/ProtectiveEquipmentObservation";

@Component({
  selector: 'app-edit-protective-equipment-observations',
  templateUrl: './edit-protective-equipment-observations.component.html'
})
export class EditProtectiveEquipmentObservationsComponent implements OnInit {

  @Input() observations: ObservationOverviewReport[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;

  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  protectiveEquipmentObservationAsChanged: ProtectiveEquipmentObservation;
  canUpdateProtectiveEquipmentObservation = false;

  registeredDate: Date = null;


  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
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
      this.protectiveEquipmentObservationAsChanged.registrationTime = this.registeredDate;
    }
  }

  updateProtectiveEquipmentObservation() {

    this.observationService.updateProtectiveEquipmentObservation(this.protectiveEquipmentObservationAsChanged).subscribe(
      (isUpdated) => {
        this.protectiveEquipmentObservationAsChanged = null;
        this.toastrService.success('Observation was updated');
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Error updating observation: ', { disableTimeOut: true});
      }
    );
  }

  deleteProtectiveEquipmentObservation() {
    this.observationService.deleteProtectiveEquipmentObservation(this.protectiveEquipmentObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.protectiveEquipmentObservationAsChanged = null;
        this.toastrService.success('Observation was deleted');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Error deleting observation', { disableTimeOut: true});
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
