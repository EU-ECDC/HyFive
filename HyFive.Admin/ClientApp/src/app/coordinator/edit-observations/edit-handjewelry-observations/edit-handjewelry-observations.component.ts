import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservationOverviewReport} from "../../../models/api/ObservationOverviewReport";
import {HandJewelryObservation} from "../../../models/api/HandJewelryObservation";
import {KeyEventService} from "../../../services/events/key-event.service";
import {Role} from "../../../models/api/Role";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {HandJewelrySelection} from "../../../../../../../HyFive.Observation/ClientApp/src/app/models/registration/handJewelry-selection.model";
import {HandJewelryTypeService} from "../../../services/data/handJewelryType.service";
import {HandJewelryType} from "../../../models/api/HandJewelryType";
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-edit-handjewelry-observations',
  templateUrl: './edit-handjewelry-observations.component.html'
})
export class EditHandjewelryObservationsComponent implements OnInit {

  @Input() observations: ObservationOverviewReport[]
  @Input() sessionId: string;
  @Input() departmentId: number;
  @Input() canEdit = false;

  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  handjewelryObservationAsChanged: HandJewelryObservation;
  handJewelrySelection: HandJewelrySelection[] = [];
  handJewelryTypes: HandJewelryType[] = [];

  constructor(
    private readonly observationService: ObservationService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly handJewelryTypeService: HandJewelryTypeService,
    private readonly dialogMessageService: DialogMessageService,
    private readonly translate: TranslateService
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
      registeredTime: observation.registeredTime
    }
  }

  changeComment(comment: string) {
    this.handjewelryObservationAsChanged.comment = comment;
  }

  updateHandJewelryObservation() {
    let types = this.handJewelrySelection.filter(h => h.isSelected).map(hsv => hsv.type)
    this.handjewelryObservationAsChanged.handJewelries = this.handJewelryTypes.filter(h => types.indexOf(h.code) !== -1)
    this.observationService.updateHandJewelryObservation(this.handjewelryObservationAsChanged).subscribe(
      (isUpdated) => {
        this.handjewelryObservationAsChanged = null;
        this.toastrService.success(this.translate.instant("The observation was updated"));
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when updating the observation"), { disableTimeOut: true});
      }
    );

  }

  deleteHandJewelryObservation() {
    this.observationService.deleteHandJewelryObservation(this.handjewelryObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.handjewelryObservationAsChanged = null;
        this.toastrService.success(this.translate.instant("The observation was deleted"));
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when deleting observation"), { disableTimeOut: true});
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
