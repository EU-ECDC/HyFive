import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Department } from '../../models/api/Department';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { faCommentDots, faHandPaper } from '@fortawesome/free-regular-svg-icons';
import { DialogueTexts } from 'src/app/constants/dialogueTexts';
import { Colors } from '../../utils/colors';
import { Role } from '../../models/api/Role';
import { GloveSessionService } from '../../services/data/glove-session.service';
import { GloveObservation } from '../../models/api/GloveObservation';
import { GloveWithIndicationTypeService } from '../../services/data/glove-with-indication-type.service';
import { GloveWithoutIndicationTypeService } from '../../services/data/glove-without-indication-type.service';
import { HandHygieneAfterGloveUseTypeService } from '../../services/data/hand-hygiene-after-glove-useType-service';
import { GloveWithoutIndicationType } from '../../models/api/GloveWithoutIndicationType';
import { HandHygieneAfterGloveUseType } from '../../models/api/HandHygieneAfterGloveUseType';
import { GloveWithIndicationType } from '../../models/api/GloveWithIndicationType';
import { Uuid } from 'src/app/utils/uuid';
import { SessionType } from 'src/app/models/api/SessionType';

@Component({
  selector: 'app-edit-glove-observation',
  templateUrl: './edit-glove-observation.component.html'
})
export class EditGloveObservationComponent implements OnInit {

  isEditMode: boolean = false;
  Colors = Colors;
  DialogueTexts = DialogueTexts;
  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[] = [];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];

  activeTab = "with";
  glovesUsed = null;
  selectedHygieneAfterGloveUse: string = null;

  uuid: string;

  faCircle = faCircle;
  faCheck = faCheck;
  faCommentLines = faCommentDots;
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  faHandPaper = faHandPaper;

  constructor(
    private sessionService: GloveSessionService,
    private gloveWithIndicationTypeService: GloveWithIndicationTypeService,
    private gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,
    private handHygieneAfterGloveUseTypeService: HandHygieneAfterGloveUseTypeService,
  ) { }

  @Input() isReadonly: boolean = false;
  @Input() observation: GloveObservation;
  @Input() department: Department;
  @Input("institutionid") institutionid: number;
  @Output() observationDeletedEvent = new EventEmitter();
  showInfoModal = false;
  observationMissingText: any;
  gloveSessionType: SessionType = SessionType.Gloves;


  ngOnInit(): void {

    this.uuid = Uuid.generateUUID();

    this.gloveWithIndicationTypeService.getGloveWithIndicationTypes().subscribe((gloveWithIndicationTypes) => {
      this.gloveWithIndicationTypes = gloveWithIndicationTypes;
      if (this.observation.gloveWithIndicationTypes?.length) {
        this.activeTab = "with";
        this.gloveWithIndicationTypes.forEach(x => {
          if (this.observation.gloveWithIndicationTypes.some(y => y.code === x.code))
            x.isSelected = true;
        });
      }
    });
    this.gloveWithoutIndicationTypeService.getGloveWithoutIndicationTypes().subscribe((gloveWithoutIndicationTypes) => {
      this.gloveWithoutIndicationTypes = gloveWithoutIndicationTypes;
      if (this.observation.gloveWithoutIndicationTypes.length) {
        this.activeTab = "without";
        this.gloveWithoutIndicationTypes.forEach(x => {
          if (this.observation.gloveWithoutIndicationTypes.some(y => y.code === x.code))
            x.isSelected = true;
        });
      }
    });
    this.handHygieneAfterGloveUseTypeService.getHandhygieneAfterGloveUseTypes().subscribe((handHygieneAfterGloveUseTypes) => {
      this.handHygieneAfterGloveUseTypes = handHygieneAfterGloveUseTypes;
    });

    this.glovesUsed = this.observation.glovesUsed;
    this.selectedHygieneAfterGloveUse = this.observation.handHygieneAfterGloveUseType?.code;
  }

  gloveWithIndicationsChanged(code, event) {
    this.gloveWithIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  gloveWithoutIndicationsChanged(code, event) {
    this.gloveWithoutIndicationTypes.forEach(x => {
      if (x.code === code) x.isSelected = event.target.checked;
    });
  }

  saveObservation() {


    this.observation.gloveWithIndicationTypes = this.gloveWithIndicationTypes.filter(x => x.isSelected);
    this.observation.gloveWithoutIndicationTypes = this.gloveWithoutIndicationTypes.filter(x => x.isSelected);
    this.observation.glovesUsed = this.glovesUsed;

    if(this.observation.glovesUsed){
      this.observation.handHygieneAfterGloveUseType = this.handHygieneAfterGloveUseTypes.find(x => x.code === this.selectedHygieneAfterGloveUse);
    }
    else {
      this.observation.handHygieneAfterGloveUseType = null;
      this.selectedHygieneAfterGloveUse = null;
    }


    this.sessionService.changeObservation(this.observation);
    this.isEditMode = false;
  }

  deleteObservation() {
    this.sessionService.deleteObservation(this.observation);
    this.observationDeletedEvent.emit();
  }

  registerComment(comment: string) {
    this.observation.comment = comment;
  }

  roleSelected(role: Role) {
    this.observation.role = role;
  }

  closeInfoModal($event: boolean) {
    this.showInfoModal = $event;
    if(this.showInfoModal === false){
      this.observationMissingText = null;
    }
  }
}
