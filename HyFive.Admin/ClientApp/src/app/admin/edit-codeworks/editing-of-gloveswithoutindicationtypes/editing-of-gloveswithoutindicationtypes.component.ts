import { Component, OnInit, OnDestroy } from '@angular/core';
import { GloveWithoutIndicationType } from '../../../models/api/GloveWithoutIndicationType';
import { ToastrService } from 'ngx-toastr';
import { GloveWithoutIndicationTypeService } from '../../../services/data/gloveWithoutIndicationType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-gloveswithoutindicationtypes',
  templateUrl: './editing-of-gloveswithoutindicationtypes.component.html'
})
export class EditingGlovewithoutindicationtypesComponent implements OnInit, OnDestroy {

  gloveWithoutIndicationType: GloveWithoutIndicationType[] = [];
  gloveWithoutIndicationTypeAsChanged: GloveWithoutIndicationType = null;

  constructor(
    private gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadGloveWithoutIndicationType();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadGloveWithoutIndicationType() {
    this.gloveWithoutIndicationTypeService.getGloveWithoutIndicationTypes().subscribe(
      (result) => this.gloveWithoutIndicationType = result,
      (error) => this.toastrService.error('An error occurred while loading GloveWithoutIndicationType: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  selectedGloveWithoutIndicationType(gloveWithoutIndicationType: GloveWithoutIndicationType): void {
    if (this.gloveWithoutIndicationTypeAsChanged?.id == gloveWithoutIndicationType.id) return;
    this.gloveWithoutIndicationTypeAsChanged = JSON.parse(JSON.stringify(gloveWithoutIndicationType));
  }

  updateGloveWithoutIndicationType(): void {
    this.gloveWithoutIndicationTypeService.updateGloveWithoutIndicationType(this.gloveWithoutIndicationTypeAsChanged).subscribe(
      (updatedGloveWithoutIndicationType) => {
        this.toastrService.success("GloveWithoutIndicationType updated");
        this.loadGloveWithoutIndicationType();
      },
      error => this.toastrService.error('An error occurred while updating GloveWithoutIndicationType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.gloveWithoutIndicationTypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.gloveWithoutIndicationTypeAsChanged = null;
  }
}
