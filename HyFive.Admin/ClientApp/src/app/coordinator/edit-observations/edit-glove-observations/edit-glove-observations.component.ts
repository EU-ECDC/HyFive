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
import { DialogMessageService } from 'src/app/services/data/dialog-message.service';
import { TranslateService } from '@ngx-translate/core';

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
    private readonly observationService: ObservationService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly gloveWithIndicationService: GloveWithIndicationTypeService,
    private readonly gloveWithoutIndicationService: GloveWithoutIndicationTypeService,
    private readonly handhygieneAfterGloveuseService: HandHygieneAfterGloveUseTypeService,
    private readonly dialogMessageService: DialogMessageService,
    private readonly translate: TranslateService

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
      glovesUsed: observation.gloveObservation.glovesUsed,
      id: observation.id,
      sessionId:  this.sessionId,
      comment: observation.comment,
      role: observation.role,
      registeredTime: observation.registeredTime,
      postGloveHandHygieneType: observation.gloveObservation.postGloveHandHygieneType,
    }
    this.gloveWithIndicationTypes.map((h) =>{
      h.isSelected = observation.gloveObservation.gloveWithIndicationTypes.map(x => x.code).indexOf(h.code) !== -1;
      return h;
    });

    this.gloveWithoutIndicationTypes.map((h) =>{
      h.isSelected = observation.gloveObservation.gloveWithIndicationTypes.map(x => x.code).indexOf(h.code) !== -1;
      return h;
    });

    this.selectedHygieneAfterGloveuseCode = this.gloveObservationAsChanged.postGloveHandHygieneType?.code;
    this.gloveWithIndicationsSelected = this.gloveWithIndicationTypes?.filter(h => h.isSelected).length > 0;
    if(this.gloveWithIndicationsSelected){
      this.gloveObservationAsChanged.gloveWithIndicationTypes = [];
    }
    else{
      // Impliserer at gloveWithoutIndicationTypes er valgt. Da skal hanske benyttes
      this.gloveObservationAsChanged.gloveWithIndicationTypes = [];
      this.gloveObservationAsChanged.glovesUsed = true;
    }
  }

  changeComment(comment: string) {
    this.gloveObservationAsChanged.comment = comment;
  }

  updateGloveObservation() {
    if(this.gloveObservationAsChanged.glovesUsed){
      this.gloveObservationAsChanged.postGloveHandHygieneType = this.handHygieneAfterGloveUseTypes.find(x => x.code === this.selectedHygieneAfterGloveuseCode);
    }
    else {
      this.gloveObservationAsChanged.postGloveHandHygieneType = null;
      this.selectedHygieneAfterGloveuseCode = null;
    }

    if(this.gloveWithIndicationsSelected){
      this.gloveObservationAsChanged.gloveWithIndicationTypes = this.gloveWithIndicationTypes.filter(h => h.isSelected);
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = [];
    }
    else {
      this.gloveObservationAsChanged.gloveWithoutIndicationTypes = this.gloveWithoutIndicationTypes.filter(h => h.isSelected);
      this.gloveObservationAsChanged.gloveWithIndicationTypes = [];
    }

    this.observationService.updateGloveObservation(this.gloveObservationAsChanged).subscribe(
      (erOppdatert) => {
        this.gloveObservationAsChanged = null;
        this.toastrService.success(this.translate.instant("The observation was updated"));
        this.observationUpdatedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when updating the observation"), { disableTimeOut: true});
      }
    );
  }

  deleteGloveObservation() {
    this.observationService.deleteGloveObservation(this.gloveObservationAsChanged.id, this.sessionId).subscribe(
      () => {
        this.gloveObservationAsChanged = null;
        this.toastrService.success(this.translate.instant("The observation was deleted"));
        this.observationDeletedEvent.emit();
      },
      (error) => {
        this.toastrService.error(error?.error.message ? error.error.message : error, this.translate.instant("Error when deleting observation"), { disableTimeOut: true});
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
    if(gloveWithIndicationsSelected === false){
      this.gloveObservationAsChanged.glovesUsed = true;
    }
  }
}
