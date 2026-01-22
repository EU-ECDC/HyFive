import { Component, OnInit, OnDestroy } from '@angular/core';
import { HandHygieneAfterGloveUseType } from '../../../models/api/HandHygieneAfterGloveUseType';
import { ToastrService } from 'ngx-toastr';
import { HandHygieneAfterGloveUseTypeService } from '../../../services/data/handHygieneAfterGloveUseType.service';
import { KeyEventService } from '../../../services/events/key-event.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-editing-of-hand-hygiene-after-glove-usetypes',
  templateUrl: './editing-of-hand-hygiene-after-glove-usetypes.component.html'
})
export class EditingHandHygieneAfterGloveUseTypesComponent implements OnInit, OnDestroy {

  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];
  handHygieneAfterGloveUseTypeWhichChanges: HandHygieneAfterGloveUseType = null;

  constructor(
    private readonly HandhygieneAfterGloveUseTypeService: HandHygieneAfterGloveUseTypeService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService

  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadHandhygieneAfterGloveUseTypes();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadHandhygieneAfterGloveUseTypes() {
    this.HandhygieneAfterGloveUseTypeService.getHandHygieneAfterGloveUseTypes().subscribe(
      (result) => this.handHygieneAfterGloveUseTypes = result,
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading Hand Hygiene After Glove Use Types:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
    );
  }

  selectedHandHygieneAfterGloveUseType(postGloveHandHygieneType: HandHygieneAfterGloveUseType): void {
    if (this.handHygieneAfterGloveUseTypeWhichChanges?.id == postGloveHandHygieneType.id) return;
    this.handHygieneAfterGloveUseTypeWhichChanges = structuredClone(postGloveHandHygieneType);
  }

  updateHandHygieneAfterGloveUseType(): void {
    this.HandhygieneAfterGloveUseTypeService.updateHandHygieneAfterGloveUseType(this.handHygieneAfterGloveUseTypeWhichChanges).subscribe(
      (updateHandHygieneAfterGloveUseType) => {
        this.toastrService.success(this.translate.instant("Hand Hygiene After Glove Use Type updated"));
        this.loadHandhygieneAfterGloveUseTypes();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating Hand Hygiene After Glove Use Type:') + ' ' + error?.error.message, '', { disableTimeOut: true}),
      () => this.handHygieneAfterGloveUseTypeWhichChanges = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.handHygieneAfterGloveUseTypeWhichChanges = null;
  }
}
