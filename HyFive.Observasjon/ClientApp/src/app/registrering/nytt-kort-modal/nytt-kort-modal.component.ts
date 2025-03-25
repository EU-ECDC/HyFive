import { Component, EventEmitter, Input, OnChanges, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Farger } from "../../utils/farger";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { faUserNurse, faCheck, faCircle, faPlus } from "@fortawesome/free-solid-svg-icons";
import { Rolle } from "../../models/api/Rolle";
import { Rollevalg } from "../../models/registrering/rollevalg.model";

export const NyttKortModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-nytt-kort-modal',
  templateUrl: './nytt-kort-modal.component.html',
})
export class NyttKortModalComponent implements OnChanges, OnInit {

  faCircle = faCircle;
  faUserNurse = faUserNurse;
  faCheck = faCheck;
  faPlus = faPlus;
  farger = Farger;

  @ViewChild('content') modalContent: TemplateRef<any>;
  private modalRef: NgbModalRef;

  closeResult = '';

  @Input() roller: Rolle[] = [];
  rollevalg: Rollevalg[] = [];

  @Output() onClose = new EventEmitter();
  @Output() onDismiss = new EventEmitter();

  constructor(private modalService: NgbModal) {
  }

  ngOnChanges(): void {
    this.setRollevalg();
  }

  ngOnInit(): void {
    this.setRollevalg();
  }

  setRollevalg() {
    if (this.roller)
      this.rollevalg = this.roller.map((rolle) => {
        return { rolle: rolle, erValgt: false } as Rollevalg
      });
  }

  resetRollevalg() {
    if (this.rollevalg)
      this.rollevalg = this.rollevalg.map(x => { x.erValgt = false; return x; });
  }

  open() {
    this.modalRef = this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: NyttKortModalComponentConfig.windowClass
    });

    this.modalRef.result.then((result: Rolle[]) => {

      this.onClose.emit(result);
      this.resetRollevalg();

    }, (reason) => {

      this.onDismiss.emit(reason);
      this.resetRollevalg();

    });
  }

  close() {
    this.modalRef.close(this.rollevalg.filter(x => x.erValgt).map(x => { return x.rolle }));
    window.scrollTo(0,0);
  }

  dismiss() {
    this.modalRef.dismiss('lukk');
  }
}
