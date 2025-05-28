import { Component, OnInit, OnDestroy } from '@angular/core';
import { HandHygieneAfterGloveUseType } from '../../../models/api/HandHygieneAfterGloveUseType';
import { ToastrService } from 'ngx-toastr';
import { HandHygieneAfterGloveUseTypeService } from '../../../services/data/handHygieneAfterGloveUseType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-hand-hygiene-after-glove-usetypes',
  templateUrl: './editing-of-hand-hygiene-after-glove-usetypes.component.html'
})
export class EditingHandHygieneAfterGloveUseTypesComponent implements OnInit, OnDestroy {

  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];
  handHygieneAfterGloveUseTypeWhichChanges: HandHygieneAfterGloveUseType = null;

  constructor(
    private HandhygieneAfterGloveUseTypeService: HandHygieneAfterGloveUseTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
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
      (error) => this.toastrService.error('An error occurred while loading Hand Hygiene After Glove Use Types: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  selectedHandHygieneAfterGloveUseType(postGloveHandHygieneType: HandHygieneAfterGloveUseType): void {
    if (this.handHygieneAfterGloveUseTypeWhichChanges?.id == postGloveHandHygieneType.id) return;
    this.handHygieneAfterGloveUseTypeWhichChanges = JSON.parse(JSON.stringify(postGloveHandHygieneType));
  }

  updateHandHygieneAfterGloveUseType(): void {
    this.HandhygieneAfterGloveUseTypeService.updateHandHygieneAfterGloveUseType(this.handHygieneAfterGloveUseTypeWhichChanges).subscribe(
      (updateHandHygieneAfterGloveUseType) => {
        this.toastrService.success("HandHygieneAfterGloveUseType updated");
        this.loadHandhygieneAfterGloveUseTypes();
      },
      error => this.toastrService.error('An error occurred while updating HandHygieneAfterGloveUseType: ' + error?.error, '', { disableTimeOut: true}),
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
