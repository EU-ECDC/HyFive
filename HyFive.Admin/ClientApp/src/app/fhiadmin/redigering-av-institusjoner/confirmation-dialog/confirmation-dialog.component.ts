import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: 'confirmation-dialog.component.html',
})

export class ConfirmationDialogComponent {

  @Input() buttonText: string;
  @Input() dialogMessage: string;
  @Input() disabled = false;
  @Input() buttonClass = 'btn btn-sm fhi-btn-secondary';
  @Input() shouldUseIcon = false;
  @Output() confirmEvent = new EventEmitter();

  constructor() {}

  confirm() {
    this.confirmEvent.emit();
  }
}
