import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { ObservationEventService } from '../../services/events/observation-event.service';
import { IndicationType } from '../../models/api/IndicationType';
import { IndicationService } from '../../services/data/indication.service';
import { faCircle, faPlus, faCheck } from '@fortawesome/free-solid-svg-icons';
import { IndicationTypeSelection } from '../../models/registration/indicationType-selection.model';
import { IndicationTypeMapper } from '../../utils/indicationtype-mapper';
import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { IndicationTypeConstants } from '../../models/api/IndicationTypeConstants';

@Component({
  selector: 'app-indication-selection',
  templateUrl: './indication-selection.component.html',
  styleUrls: ['./indication-selection.component.scss']
})
export class IndicationSelectionComponent implements OnInit {

  faCircle = faCircle;
  faCheck = faCheck;
  faPlus = faPlus;
  iconTypeMap: Map<IndicationTypeConstants, IconProp> = IndicationTypeMapper.getIconTypeMap();

  indicationTypeSelection: IndicationTypeSelection[] = [];

  @Input("isActive") isActive: boolean;
  @Input("parentId") parentId: string;
  @Input("availableIndications") availableIndications: IndicationType[];
  @Input("selectedIndications") selectedIndications: IndicationType[] = [];
  @Input("isReadonly") isReadonly: boolean;
  @Output() indicationSelectionChangedEvent = new EventEmitter<IndicationType[]>();

  constructor(private observationEventService: ObservationEventService,
    private indicationService: IndicationService) { }

  ngOnInit(): void {
    this.indicationService.getIndicationTypes().subscribe((indicationTypes) => {
      this.availableIndications = indicationTypes;
      this.indicationTypeSelection = IndicationTypeMapper.getIndicationTypeOption(this.availableIndications, this.selectedIndications);
    });

    this.observationEventService.observationResetEvent.subscribe((parentId) => {
      if (parentId == this.parentId) {
        this.selectedIndications = [];
        this.indicationTypeSelection = IndicationTypeMapper.getIndicationTypeOption(this.availableIndications, []);
      }
    })
  }

  changed(indication: IndicationType): void {
    let selection = this.indicationTypeSelection.filter(x => x.isSelected);
    this.indicationSelectionChangedEvent.emit(this.availableIndications.filter(x => selection.some(y => y.code === x.code)));
  }
}
