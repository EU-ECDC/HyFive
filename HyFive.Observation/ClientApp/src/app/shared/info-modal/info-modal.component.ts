import { Component, Input, OnChanges, SimpleChanges, ViewChild, TemplateRef, Output, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-info-modal',
  templateUrl: './info-modal.component.html'
})
export class InfoModalComponent implements OnChanges {

  @Input() showInfoModal: boolean;
  @Input() modalText: string;
  @Output() closeInfoModalEvent = new EventEmitter<boolean>();

  @ViewChild("modal") modal: TemplateRef<any>;

  ngOnChanges(changes: SimpleChanges){
    if(changes.showInfoModal?.previousValue === false && changes.showInfoModal?.currentValue === true){
      this.modalService.open(this.modal, { windowClass: 'hh-modal' });
    }
  }

  constructor(private readonly modalService: NgbModal) { }

  close(): void {
    this.showInfoModal = false;
    this.closeInfoModalEvent.emit(this.showInfoModal);
  }
}
