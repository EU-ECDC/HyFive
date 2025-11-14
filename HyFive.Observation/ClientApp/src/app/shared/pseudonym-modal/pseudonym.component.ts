import {AfterViewInit, Component, EventEmitter, Output, TemplateRef, ViewChild} from '@angular/core';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-pseudonym',
  templateUrl: './pseudonym.component.html'
})
export class PseudonymComponent implements AfterViewInit{

  @Output() closeInfoModalEvent = new EventEmitter<boolean>();
  @ViewChild('content') modalContent: TemplateRef<any>;

  showInfoModal = true;

  constructor(private readonly modalService: NgbModal) {
  }

  ngAfterViewInit(): void {
    const modalRef = this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title'
    });
  }

  close() {
    this.showInfoModal = false;
    this.closeInfoModalEvent.emit(this.showInfoModal);
  }
}
