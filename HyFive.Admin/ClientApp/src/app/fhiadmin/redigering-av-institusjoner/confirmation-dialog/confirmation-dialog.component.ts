import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: 'confirmation-dialog.component.html',
   styleUrls: ['confirmation-dialog.component.scss']
})

export class ConfirmationDialogComponent {

  @Input() buttonText: string;
  @Input() dialogMessage: string;
  @Input() disabled = false;
  @Input() buttonClass = 'btn btn-sm fhi-btn-secondary';
  @Input() shouldUseIcon = false;
  @Output() confirmEvent = new EventEmitter();

    showModalDialog: boolean = false;


  constructor() {}

  confirm() {
    this.confirmEvent.emit();
  }

  showModal() {
    this.showModalDialog = !this.showModalDialog;
  }
}
