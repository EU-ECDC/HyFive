import { Component, OnInit, OnDestroy } from '@angular/core';
import { IndicationTypesService } from '../../../services/data/indicationTypes.service';
import { IndicationType } from '../../../models/api/IndicationType';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-indication-types',
  templateUrl: './editing-indication-types.component.html'
})
export class EditingIndicationTypesComponent implements OnInit, OnDestroy {

  indicationtypes: IndicationType[] = [];
  indicationtypeAsChanged: IndicationType = null;

  constructor(
    private indicationTypesService: IndicationTypesService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadIndicationtypes();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  loadIndicationtypes() {
    this.indicationTypesService.getIndicationTypes().subscribe(
      (result) => this.indicationtypes = result,
      (error) => this.toastrService.error('An error occurred while loading Indication Types: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  selectedIndicationtype(indicationtype: IndicationType): void {
    if (this.indicationtypeAsChanged?.id == indicationtype.id) return;
    this.indicationtypeAsChanged = JSON.parse(JSON.stringify(indicationtype));
  }

  updateIndicationtype(indicationtype: IndicationType): void {
    indicationtype.number = indicationtype.number.toString();
    this.indicationTypesService.updateIndicationTypes(indicationtype).subscribe(
      (updatedIndicationtype) => {
        this.toastrService.success("Indicationstype updated");
        this.loadIndicationtypes();
      },
      error => this.toastrService.error('An error occurred while updating Indicationstype: ' + error?.error, '', { disableTimeOut: true}),
      () => this.indicationtypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.indicationtypeAsChanged = null;
  }
}
