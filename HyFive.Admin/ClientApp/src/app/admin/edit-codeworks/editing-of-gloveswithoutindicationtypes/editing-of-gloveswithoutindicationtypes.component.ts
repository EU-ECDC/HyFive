import { Component, OnInit, OnDestroy } from '@angular/core';
import { GloveWithoutIndicationType } from '../../../models/api/GloveWithoutIndicationType';
import { ToastrService } from 'ngx-toastr';
import { GloveWithoutIndicationTypeService } from '../../../services/data/gloveWithoutIndicationType.service';
import { KeyEventService } from '../../../services/events/key-event.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-editing-of-gloveswithoutindicationtypes',
  templateUrl: './editing-of-gloveswithoutindicationtypes.component.html'
})
export class EditingGlovewithoutindicationtypesComponent implements OnInit, OnDestroy {

  gloveWithoutIndicationType: GloveWithoutIndicationType[] = [];
  gloveWithoutIndicationTypeAsChanged: GloveWithoutIndicationType = null;

  constructor(
    private readonly gloveWithoutIndicationTypeService: GloveWithoutIndicationTypeService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService

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
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Glove Without Indication Type:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  selectedGloveWithoutIndicationType(gloveWithoutIndicationType: GloveWithoutIndicationType): void {
    if (this.gloveWithoutIndicationTypeAsChanged?.id == gloveWithoutIndicationType.id) return;
    this.gloveWithoutIndicationTypeAsChanged = structuredClone(gloveWithoutIndicationType);
  }

  updateGloveWithoutIndicationType(): void {
    this.gloveWithoutIndicationTypeService.updateGloveWithoutIndicationType(this.gloveWithoutIndicationTypeAsChanged).subscribe(
      (updatedGloveWithoutIndicationType) => {
        this.toastrService.success(this.translate.instant("Glove Without Indication Type updated"));
        this.loadGloveWithoutIndicationType();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating Glove Without Indication Type:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
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
