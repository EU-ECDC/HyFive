import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-delete-confirmation-dialog',
  templateUrl: 'delete-confirmation-dialog.component.html',
})

export class DeleteConfirmationDialogComponent {

  @Input("dialogMessage") dialogMessage: string;
  @Input("isBlackColor") isBlackColor: boolean = true;
  @Output("deletedEvent") deletedEvent = new EventEmitter();

  constructor(private modalService: NgbModal) {}

  delete(){
    this.deletedEvent.emit();
  }

  showDeleteModal(modalName) {
    this.modalService.open(modalName, { windowClass: 'hh-modal' });
  }
}
