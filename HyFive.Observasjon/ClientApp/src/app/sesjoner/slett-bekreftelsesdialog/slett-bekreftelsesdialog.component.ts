import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-slett-bekreftelsesdialog',
  templateUrl: 'slett-bekreftelsesdialog.component.html',
})

export class SlettBekrefelsesdialogComponent {

  @Input("dialogmelding") dialogmelding: string;
  @Input("erSvartFarge") erSvartFarge: boolean = true;
  @Output("slettetEvent") slettetEvent = new EventEmitter();

  constructor(private modalService: NgbModal) {}

  slett(){
    this.slettetEvent.emit();
  }

  visSlettModal(modalName) {
    this.modalService.open(modalName, { windowClass: 'hh-modal' });
  }
}
