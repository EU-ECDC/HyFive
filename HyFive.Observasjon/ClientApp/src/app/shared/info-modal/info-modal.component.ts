import { Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild, TemplateRef, Output, EventEmitter } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-info-modal',
  templateUrl: './info-modal.component.html'
})
export class InfoModalComponent implements OnInit {

  @Input() visInfoModal: boolean;
  @Input() modalTekst: string;
  @Output() lukkInfoModalEvent = new EventEmitter<boolean>();

  @ViewChild("modal") modal: TemplateRef<any>;

  ngOnChanges(changes: SimpleChanges){
    if(changes.visInfoModal?.previousValue === false && changes.visInfoModal?.currentValue === true){
      this.modalService.open(this.modal, { windowClass: 'hh-modal' });
    }
  }

  constructor(private modalService: NgbModal) { }

  ngOnInit(): void { }

  lukk(): void {
    this.visInfoModal = false;
    this.lukkInfoModalEvent.emit(this.visInfoModal);
  }
}
