import { Component, Input } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

export const DialogModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-dialog-modal',
  templateUrl: './dialog-modal.component.html',
})
export class DialogModalComponent {

  @Input() message: string;

  constructor(private readonly activeModal: NgbActiveModal) {
  }


  close(result: boolean) {
    this.activeModal.close(result);
  }

  dismiss() {
    this.activeModal.dismiss(false);
  }
}
