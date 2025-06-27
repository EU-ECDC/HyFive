import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ObservationOverviewReport} from "../../../models/api/ObservationOverviewReport";
import {Department} from "../../../models/api/Department";
import {ObservationService} from "../../../services/data/observation.service";
import {ToastrService} from "ngx-toastr";
import {KeyEventService} from "../../../services/events/key-event.service";
import {GloveWithIndicationTypeService
} from "../../../services/data/gloveWithIndicationType.service";
import {GloveWithoutIndicationTypeService} from "../../../services/data/gloveWithoutIndicationType.service";
import {HandHygieneAfterGloveUseTypeService} from "../../../services/data/handHygieneAfterGloveUseType.service";
import {GloveWithIndicationType} from "../../../models/api/GloveWithIndicationType";
import {GloveWithoutIndicationType} from "../../../models/api/GloveWithoutIndicationType";
import {Role} from "../../../models/api/Role";
import {GloveObservation} from "../../../models/api/GloveObservation";
import {HandHygieneAfterGloveUseType} from "../../../models/api/HandHygieneAfterGloveUseType";

@Component({
  selector: 'app-edit-glove-observations',
  templateUrl: './edit-glove-observations.component.html'
})
export class EditGloveObservationsComponent implements OnInit{

  @Input() observations: ObservationOverviewReport[]
  @Input() sessionId: string;
  @Input() department: Department;
  @Input() canEdit = false;

  @Output() observationUpdatedEvent = new EventEmitter();
  @Output() observationDeletedEvent = new EventEmitter();

  gloveObservationAsChanged: GloveObservation;
  selectedHygieneAfterGloveuseCode: any;

  gloveWithIndicationsSelected: boolean;

  gloveWithIndicationTypes: GloveWithIndicationType[];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[];

  constructor(
    private observationService: ObservationService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService,
    private gloveWithIndicationService: GloveWithIndicationTypeService,
    private gloveWithoutIndicationService: GloveWithoutIndicationTypeService,
    private handhygieneAfterGloveuseService: HandHygieneAfterGloveUseTypeService

  ) { }

  ngOnInit(): void {
    this.handhygieneAfterGloveuseService.getHandHygieneAfterGloveUseTypes().subscribe((types) => {
      this.handHygieneAfterGloveUseTypes = types;
    })

    this.gloveWithIndicationService.getGloveWithIndicationTypes().subscribe((types)=> {
      this.gloveWithIndicationTypes = types;
    })

    this.gloveWithoutIndicationService.getGloveWithoutIndicationTypes().subscribe((types)=> {
      this.gloveWithoutIndicationTypes = types;
    })

    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      if (this.gloveObservationAsChanged)
        this.gloveObservationAsChanged = null;
    });
  }

  selectObservation(observation: ObservationOverviewReport) {
    if(!this.canEdit){
      return;
    }

    this.gloveObservationAsChanged = {
      gloveUsed: observation.gloveObservation.gloveUsed,
      id: observation.id,
      sessionId:  this.sessionId,
      comment: observation.comment,
      role: observation.role,
      registeredTime: observation.registeredTime,
      postGloveHandHygieneType: observation.gloveObservation.postGloveHandHygieneType,
    }
    this.gloveWithIndicationTypes.map((h) =>{
      h.isSelected = observation.gloveObservation.indicatedGloveTypes.map(x => x.code).indexOf(h.code) !== -1;
      return h;
    });

    this.gloveWithoutIndicationTypes.map((h) =>{
      h.isSelected = observation.gloveObservation.indicatedGloveTypes.map(x => x.code).indexOf(h.code) !== -1;
      return h;
    });

    this.selectedHygieneAfterGloveuseCode = this.gloveObservationAsChanged.postGloveHandHygieneType?.code;
    this.gloveWithIndicationsSelected = this.gloveWithIndicationTypes?.filter(h => h.isSelected).length > 0;
    if(this.gloveWithIndicationsSelected){
      this.gloveObservationAsChanged.indicatedGloveTypes = [];
    }
    else{
      // Impliserer at gloveWithoutIndicationTypes er valgt. Da skal hanske benyttes
      this.gloveObservationAsChanged.indicatedGloveTypes = [];
      this.gloveObservationAsChanged.gloveUsed = true;
    }
  }

  changeComment(comment: string) {
    this.gloveObservationAsChanged.comment = comment;
  }

  updateGloveObservation() {
    if(this.gloveObservationAsChanged.gloveUsed){
      this.gloveObservationAsChanged.postGloveHandHygieneType = this.handHygieneAfterGloveUseTypes.find(x => x.code === this.selectedHygieneAfterGloveuseCode);
    }
    else {
      this.gloveObservationAsChanged.postGloveHandHygieneType = null;
      this.selectedHygieneAfterGloveuseCode = null;
    }

    if(this.gloveWithIndicationsSelected){
      this.gloveObservationAsChanged.indicatedGloveTypes = this.gloveWithIndicationTypes.filter(h => h.isSelected);
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = [];
    }
    else {
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = this.gloveWithoutIndicationTypes.filter(h => h.isSelected);
      this.gloveObservationAsChanged.indicatedGloveTypes = [];
    }

    this.observationService.updateGloveObservation(this.gloveObservationAsChanged).subscribe(
      (erOppdatert) => {
        this.gloveObservationAsChanged = null;
        this.toastrService.success('Observation was updated');
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Error updating observation: ', { disableTimeOut: true});
      }
    );
  }

  deleteGloveObservation() {
    this.observationService.deleteGloveObservation(this.gloveObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.gloveObservationAsChanged = null;
        this.toastrService.success('Observation was deleted');
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error ? error.error : error, 'Error deleting observation', { disableTimeOut: true});
      });
  }

  cancelEditObservation(event) {
    event.stopPropagation();
    this.gloveObservationAsChanged = null;
    this.selectedHygieneAfterGloveuseCode = null;
  }

  selectRole(role: Role) {
    this.gloveObservationAsChanged.role = role;
  }

  setUsedGloveIfCurrent(gloveWithIndicationsSelected: boolean) {
    if(gloveWithIndicationsSelected == false){
      this.gloveObservationAsChanged.gloveUsed = true;
    }
  }
}
