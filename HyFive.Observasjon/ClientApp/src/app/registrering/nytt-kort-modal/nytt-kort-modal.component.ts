import { Component, EventEmitter, Input, OnChanges, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Farger } from "../../utils/farger";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { faUserNurse, faCheck, faCircle, faPlus } from "@fortawesome/free-solid-svg-icons";
import { Role } from "../../models/api/Role";
import { RoleSelected } from "../../models/registration/roleSelected.model";

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

  @Input() roles: Role[] = [];
  roleSelected: RoleSelected[] = [];

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
    if (this.roles)
      this.roleSelected = this.roles.map((role) => {
        return { role: role, isSelected: false } as RoleSelected
      });
  }

  resetRollevalg() {
    if (this.roleSelected)
      this.roleSelected = this.roleSelected.map(x => { x.isSelected = false; return x; });
  }

  open() {
    this.modalRef = this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: NyttKortModalComponentConfig.windowClass
    });

    this.modalRef.result.then((result: Role[]) => {

      this.onClose.emit(result);
      this.resetRollevalg();

    }, (reason) => {

      this.onDismiss.emit(reason);
      this.resetRollevalg();

    });
  }

  close() {
    this.modalRef.close(this.roleSelected.filter(x => x.isSelected).map(x => { return x.role }));
    window.scrollTo(0,0);
  }

  dismiss() {
    this.modalRef.dismiss('lukk');
  }
}
