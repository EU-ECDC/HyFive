import { Component, OnInit, OnDestroy } from '@angular/core';
import { GloveWithIndicationType } from '../../../models/api/GloveWithIndicationType';
import { GloveWithIndicationTypeService
 } from '../../../services/data/gloveWithIndicationType.service';
import { ToastrService } from 'ngx-toastr';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-glove-indicationtypes',
  templateUrl: './editing-gloves-with-indicationtypes.component.html'
})
export class EditingGlovesWithIndicationTypesComponent implements OnInit, OnDestroy {

  gloveWithIndicationTypes: GloveWithIndicationType[] = [];
  gloveWithIndicationTypeAsChanged: GloveWithIndicationType = null;

  constructor(
    private gloveWithIndicationTypeService: GloveWithIndicationTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadGloveByIndicationType();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadGloveByIndicationType() {
    this.gloveWithIndicationTypeService.getGloveWithIndicationTypes().subscribe(
      (result) => this.gloveWithIndicationTypes = result,
      (error) => this.toastrService.error('An error occurred while loading GloveByIndicationType: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  selectedGloveWithIndicationType(gloveWithIndicationType: GloveWithIndicationType): void {
    if (this.gloveWithIndicationTypeAsChanged?.id == gloveWithIndicationType.id) return;
    this.gloveWithIndicationTypeAsChanged = JSON.parse(JSON.stringify(gloveWithIndicationType));
  }

  updateGloveWithIndicationType(): void {
    this.gloveWithIndicationTypeService.updateGloveWithIndicationType(this.gloveWithIndicationTypeAsChanged).subscribe(
      (updatedGloveWithIndicationType) => {
        this.toastrService.success("GloveByIndicationType updated");
        this.loadGloveByIndicationType();
      },
      error => this.toastrService.error('An error occurred while updating GloveByIndicationType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.gloveWithIndicationTypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.gloveWithIndicationTypeAsChanged = null;
  }
}
