import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservationOverviewReport} from "../../../models/api/ObservationOverviewReport";
import {HandJewelryObservation} from "../../../models/api/HandJewelryObservation";
import {Department} from "../../../models/api/Department";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Role} from "../../../models/api/Role";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {HandJewelrySelection} from "../../../../../../../HyFive.Observasjon/ClientApp/src/app/models/registration/handJewelry-selection.model";
import {HandJewelryTypeService} from "../../../services/data/handJewelryType.service";
import {HandJewelryType} from "../../../models/api/HandJewelryType";

@Component({
  selector: 'app-edit-handjewelry-observations',
  templateUrl: './edit-handjewelry-observations.component.html'
})
export class EditHandjewelryObservationsComponent implements OnInit {

  @Input() observations: ObservationOverviewReport[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;

  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  handjewelryObservationAsChanged: HandJewelryObservation;
  handJewelrySelection: HandJewelrySelection[] = [];
  handJewelryTypes: HandJewelryType[] = [];

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private handJewelryTypeService: HandJewelryTypeService
    ) { }

  ngOnInit(): void {
    this.handJewelryTypeService.getHandJewelryTypes().subscribe((types) => {
      this.handJewelryTypes = types;
    })
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.handjewelryObservationAsChanged)
        this.handjewelryObservationAsChanged = null;
    });
  }

  selectObservation(observation: ObservationOverviewReport) {
    if(!this.canEdit){
      return;
    }

    this.handJewelrySelection = this.handJewelryTypes.map((t) => {
      return {
        type: t.code,
        disabled: false,
        isSelected: observation.handJewelryTypes.map(ht => ht.code).indexOf(t.code) !== -1,
        name: t.name
      };
    })

    this.handjewelryObservationAsChanged = {
      id: observation.id,
      sessionId:  this.sessionId,
      handJewelries: observation.handJewelryTypes,
      comment: observation.comment,
      role: observation.role,
      registrationTime: observation.registeredTime
    }
  }

  changeComment(comment: string) {
    this.handjewelryObservationAsChanged.comment = comment;
  }

  updateHandJewelryObservation() {
    var types = this.handJewelrySelection.filter(h => h.isSelected).map(hsv => hsv.type)
    this.handjewelryObservationAsChanged.handJewelries = this.handJewelryTypes.filter(h => types.indexOf(h.code) !== -1)
    this.observationService.updateHandJewelryObservation(this.handjewelryObservationAsChanged).subscribe(
      (isUpdated) => {
        this.handjewelryObservationAsChanged = null;
        this.toastrService.success('The observation was updated');
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Error updating observation: ', { disableTimeOut: true});
      }
    );

  }

  deleteHandJewelryObservation() {
    this.observationService.deleteHandJewelryObservation(this.handjewelryObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.handjewelryObservationAsChanged = null;
        this.toastrService.success('The observation was deleted');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Error deleting observation', { disableTimeOut: true});
      });
  }

  cancelEditObservation(event) {
    event.stopPropagation();
    this.handjewelryObservationAsChanged = null;
  }

  selectRole(role: Role) {
    this.handjewelryObservationAsChanged.role = role;
  }
}
