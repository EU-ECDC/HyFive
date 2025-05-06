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

@Component({
  selector: 'app-rediger-hanske-observasjon',
  templateUrl: './rediger-hanske-observasjon.component.html'
})
export class RedigerHanskeObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  Colors = Colors;
  DialogueTexts = DialogueTexts;
  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithoutIndicationTypes: GloveWithoutIndicationType[] = [];
  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];

  activeTab = "with";
  hanskeBrukt = null;
  valgtHygieneEtterHanskebruk: string = null;

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
  @Output() observasjonSlettetEvent = new EventEmitter();
  showInfoModal = false;
  observationMissingText: any;


  ngOnInit(): void {

    this.uuid = Uuid.generateUUID();

    this.gloveWithIndicationTypeService.getGloveWithIndicationTypes().subscribe((gloveWithIndicationTypes) => {
      this.gloveWithIndicationTypes = gloveWithIndicationTypes;
      if (this.observation.gloveWithIndicationTypes.length) {
        this.activeTab = "with";
        this.gloveWithIndicationTypes.forEach(x => {
          if (this.observation.gloveWithIndicationTypes.some(y => y.code === x.code))
            x.isSelected = true;
        });
      }
    });
    this.gloveWithoutIndicationTypeService.getHanskeUtenIndikasjonTyper().subscribe((gloveWithoutIndicationTypes) => {
      this.gloveWithoutIndicationTypes = gloveWithoutIndicationTypes;
      if (this.observation.gloveWithoutIndicationTypes.length) {
        this.activeTab = "uten";
        this.gloveWithoutIndicationTypes.forEach(x => {
          if (this.observation.gloveWithoutIndicationTypes.some(y => y.code === x.code))
            x.isSelected = true;
        });
      }
    });
    this.handHygieneAfterGloveUseTypeService.getHandhygieneAfterGloveUseTypes().subscribe((handHygieneAfterGloveUseTypes) => {
      this.handHygieneAfterGloveUseTypes = handHygieneAfterGloveUseTypes;
    });

    this.hanskeBrukt = this.observation.gloveUsed;
    this.valgtHygieneEtterHanskebruk = this.observation.handHygieneAfterGloveUseType?.code;
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

  lagreObservasjon() {


    this.observation.gloveWithIndicationTypes = this.gloveWithIndicationTypes.filter(x => x.isSelected);
    this.observation.gloveWithoutIndicationTypes = this.gloveWithoutIndicationTypes.filter(x => x.isSelected);
    this.observation.gloveUsed = this.hanskeBrukt;

    if(this.observation.gloveUsed){
      this.observation.handHygieneAfterGloveUseType = this.handHygieneAfterGloveUseTypes.find(x => x.code === this.valgtHygieneEtterHanskebruk);
    }
    else {
      this.observation.handHygieneAfterGloveUseType = null;
      this.valgtHygieneEtterHanskebruk = null;
    }


    this.sessionService.changeObservation(this.observation);
    this.erRedigeringsmodus = false;
  }

  deleteObservation() {
    this.sessionService.deleteObservation(this.observation);
    this.observasjonSlettetEvent.emit();
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
