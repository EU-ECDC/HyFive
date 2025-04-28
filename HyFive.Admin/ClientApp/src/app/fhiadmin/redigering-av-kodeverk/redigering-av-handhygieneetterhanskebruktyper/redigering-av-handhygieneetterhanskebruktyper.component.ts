import { Component, OnInit, OnDestroy } from '@angular/core';
import { HandHygieneAfterGloveUseType } from '../../../models/api/HandHygieneAfterGloveUseType';
import { ToastrService } from 'ngx-toastr';
import { HandHygieneAfterGloveUseTypeService } from '../../../services/data/handHygieneAfterGloveUseType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-redigering-av-handhygieneetterhanskebruktyper',
  templateUrl: './redigering-av-handhygieneetterhanskebruktyper.component.html'
})
export class RedigeringAvHandhygieneetterhanskebruktyperComponent implements OnInit, OnDestroy {

  handHygieneAfterGloveUseTypes: HandHygieneAfterGloveUseType[] = [];
  handHygieneAfterGloveUseTypeWhichChanges: HandHygieneAfterGloveUseType = null;

  constructor(
    private handhygieneEtterHanskebrukTypeService: HandHygieneAfterGloveUseTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.lastHandhygieneEtterHanskebrukTyper();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  lastHandhygieneEtterHanskebrukTyper() {
    this.handhygieneEtterHanskebrukTypeService.getHandHygieneAfterGloveUseTypes().subscribe(
      (result) => this.handHygieneAfterGloveUseTypes = result,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av HandhygieneEtterHanskebrukTyper: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  valgtHandhygieneEtterHanskebrukType(handhygieneEtterHanskebrukType: HandHygieneAfterGloveUseType): void {
    if (this.handHygieneAfterGloveUseTypeWhichChanges?.id == handhygieneEtterHanskebrukType.id) return;
    this.handHygieneAfterGloveUseTypeWhichChanges = JSON.parse(JSON.stringify(handhygieneEtterHanskebrukType));
  }

  updateHandHygieneAfterGloveUseType(): void {
    this.handhygieneEtterHanskebrukTypeService.updateHandHygieneAfterGloveUseType(this.handHygieneAfterGloveUseTypeWhichChanges).subscribe(
      (updateHandHygieneAfterGloveUseType) => {
        this.toastrService.success("HandHygieneAfterGloveUseType oppdatert");
        this.lastHandhygieneEtterHanskebrukTyper();
      },
      error => this.toastrService.error('Det oppstod en feil under oppdatering av HandHygieneAfterGloveUseType: ' + error?.error, '', { disableTimeOut: true}),
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
