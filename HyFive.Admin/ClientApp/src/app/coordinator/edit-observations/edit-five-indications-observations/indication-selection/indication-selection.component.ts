import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import {IndicationTypeChoice} from "../../../../models/five-indications/IndicationTypeChoice.model";
import {IndicationType} from "../../../../models/api/IndicationType";
import {IndicationTypesService} from "../../../../services/data/indicationTypes.service";
import {IndicationTypeMapper} from "../../../../utils/indicationtype-mapper";


@Component({
  selector: 'app-indication-selection',
  templateUrl: './indication-selection.component.html'
})
export class IndicationSelectionComponent implements OnInit {

  indicationTypeChoice: IndicationTypeChoice[] = [];

  @Input() parentId: string;
  @Input() availableIndications: IndicationType[];
  @Input() selectedIndications: IndicationType[] = [];
  @Output() indicationOptionChangedEvent = new EventEmitter<IndicationType[]>();

  constructor(private readonly indicationTypesService: IndicationTypesService) { }

  ngOnInit(): void {
    this.indicationTypesService.getIndicationTypes().subscribe((indicationTypes) => {
      this.availableIndications = indicationTypes;
      this.indicationTypeChoice = IndicationTypeMapper.getIndicationTypeSelection(this.availableIndications, this.selectedIndications);
    });
  }

  changed(indication: IndicationType): void {
    let valg = this.indicationTypeChoice.filter(x => x.isSelected);
    this.indicationOptionChangedEvent.emit(this.availableIndications.filter(x => valg.some(y => y.code === x.code)));
  }
}
