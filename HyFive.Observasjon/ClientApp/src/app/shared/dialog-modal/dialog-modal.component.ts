import { Component, Input, OnInit } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";

export const DialogModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-dialog-modal',
  templateUrl: './dialog-modal.component.html',
})
export class DialogModalComponent implements OnInit {

  @Input() melding: string;

  constructor(private activeModal: NgbActiveModal) {
  }

  ngOnInit(): void {
  }

  close(result: boolean) {
    this.activeModal.close(result);
  }

  dismiss() {
    this.activeModal.dismiss(false);
  }
}
