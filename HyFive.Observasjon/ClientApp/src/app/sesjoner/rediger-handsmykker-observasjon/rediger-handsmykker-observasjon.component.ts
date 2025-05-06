import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Department } from '../../models/api/Department';
import { HandJewelryObservation } from '../../models/api/HandJewelryObservation';
import { HandJewelryType } from '../../models/api/HandJewelryType';
import { faCheck, faCircle, faTrashAlt, faSave } from '@fortawesome/free-solid-svg-icons';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { HandJewelrySessionService } from '../../services/data/hand-Jewelry-session.service';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { HandJewelrySelection } from '../../models/registration/handJewelry-selection.model';
import { HandJewelryMapper } from '../../utils/handJewelry-mapper';
import { DialogueTexts } from 'src/app/constants/dialogueTexts';
import { Colors } from '../../utils/colors';
import { Role } from '../../models/api/Role';
import { HandJewelryTypeConstants } from '../../models/api/HandJewelryTypeConstants';
import { HandJewelryTypeService } from '../../services/data/hand-jewelry-type.service';

@Component({
  selector: 'app-rediger-handsmykker-observasjon',
  templateUrl: './rediger-handsmykker-observasjon.component.html'
})
export class RedigerHandsmykkerObservasjonComponent implements OnInit {

  erRedigeringsmodus: boolean = false;
  handJewelrySelection = [] as HandJewelrySelection[];
  handJewelryTypes: HandJewelryType[] = [];
  iconTypeMap: Map<HandJewelryTypeConstants, IconProp> = HandJewelryMapper.getIconTypeMap();
  Colors = Colors;
  DialogueTexts = DialogueTexts;

  faCircle = faCircle;
  faCheck = faCheck;
  faCommentLines = faCommentDots;
  faSave = faSave;
  faTrashAlt = faTrashAlt;

  constructor(
    private sessionService: HandJewelrySessionService,
    private handJewelryTypeService: HandJewelryTypeService
  ) { }

  @Input() isReadonly: boolean = false;
  @Input() observation: HandJewelryObservation;
  @Input() department: Department;
  @Output() observasjonSlettetEvent = new EventEmitter();


  ngOnInit(): void {
    this.handJewelryTypeService.getHandJewelryTypes().subscribe((handJewelryTypes) => {
      this.handJewelryTypes = handJewelryTypes;
      this.handJewelrySelection = HandJewelryMapper.getHandjewelrySelection(this.handJewelryTypes, this.observation.handJewelry.map(x => x?.code));
      this.handJewelrySelection.forEach(x => this.changed(x));
    });
  }

  antallValgteHandsmykker() {
    return this.handJewelrySelection.reduce((acc, curr) => { if (curr.isSelected) return acc + 1; return acc; }, 0);
  }

  changed(valg: HandJewelrySelection) {
    if (valg.isSelected && valg.type == HandJewelryTypeConstants.AllClear)
      this.handJewelrySelection = this.handJewelrySelection.map(x => { if (x.type !== HandJewelryTypeConstants.AllClear) { x.disabled = true; x.isSelected = false; } return x; }) // disable all
    else if (valg.isSelected && valg.type != HandJewelryTypeConstants.AllClear)
      this.handJewelrySelection = this.handJewelrySelection.map(x => { if (x.type === HandJewelryTypeConstants.AllClear) { x.disabled = true; x.isSelected = false; } return x; }) // disable altok
    else if (this.antallValgteHandsmykker() < 1)
      this.handJewelrySelection = this.handJewelrySelection.map(x => { x.disabled = false; return x; }) // enable all
  }

  lagreObservasjon() {
    this.observation.handJewelry = this.handJewelrySelection.reduce((acc, item) => {
      if (item.isSelected) acc.push(this.handJewelryTypes.find(x => x.code === item.type));
      return acc;
    }, [] as HandJewelryType[]) as HandJewelryType[];
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

  rolleValgt(role: Role) {
    this.observation.role = role;
  }
}
