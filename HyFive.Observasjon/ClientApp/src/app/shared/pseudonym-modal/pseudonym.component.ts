import {AfterViewInit, Component, EventEmitter, Input, Output, TemplateRef, ViewChild} from '@angular/core';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pseudonym',
  templateUrl: './pseudonym.component.html'
})
export class PseudonymComponent implements AfterViewInit{

  @Output() lukkInfoModalEvent = new EventEmitter<boolean>();
  @ViewChild('content') modalContent: TemplateRef<any>;

  visInfoModal = true;

  constructor(private modalService: NgbModal) {
  }

  ngAfterViewInit(): void {
    const modalRef = this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title'
    });
  }

  lukk() {
    this.visInfoModal = false;
    this.lukkInfoModalEvent.emit(this.visInfoModal);
  }
}
