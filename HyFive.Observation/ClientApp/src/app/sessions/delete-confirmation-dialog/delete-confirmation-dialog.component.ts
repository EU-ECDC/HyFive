import { Component, Input, Output, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-delete-confirmation-dialog',
  templateUrl: 'delete-confirmation-dialog.component.html',
})

export class DeleteConfirmationDialogComponent {

  @Input() dialogMessage: string;
  @Input() isBlackColor: boolean = true;
  @Output() deletedEvent = new EventEmitter();

  constructor(private readonly modalService: NgbModal) {}

  delete(){
    this.deletedEvent.emit();
  }

  showDeleteModal(modalName) {
    this.modalService.open(modalName, { windowClass: 'hh-modal' });
  }
}
