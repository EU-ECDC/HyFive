import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import {IndicationTypeChoice} from "../../../../models/five-indications/IndicationTypeChoice.model";
import {IndicationType} from "../../../../models/api/IndicationType";
import {IndicationTypesService} from "../../../../services/data/indicationTypes.service";
import {IndicationTypeMapper} from "../../../../utils/indicationtype-mapper";


@Component({
  selector: 'app-hand-hygiene-selection',
  templateUrl: './hand-hygiene-selection.component.html'
})
export class HandHygieneSelectionComponent implements OnInit {

  handhygieneTypeChoice: IndicationTypeChoice[] = [];

  @Input() parentId: string;
  @Input() availableIndications: IndicationType[];
  @Input() selectedIndications: IndicationType[] = [];
  @Output() indicationOptionChangedEvent = new EventEmitter<IndicationType[]>();

  constructor(private readonly handhygieneTypesService: IndicationTypesService) { }

  ngOnInit(): void {
    this.handhygieneTypesService.getIndicationTypes().subscribe((indicationTypes) => {
      this.availableIndications = indicationTypes;
      this.handhygieneTypeChoice = IndicationTypeMapper.getIndicationTypeSelection(this.availableIndications, this.selectedIndications);
    });
  }

  changed(indication: IndicationType): void {
    let selected = this.handhygieneTypeChoice.filter(x => x.isSelected);
    this.indicationOptionChangedEvent.emit(this.availableIndications.filter(x => selected.some(y => y.code === x.code)));
  }
}
