import { Component, EventEmitter, Input, OnChanges, OnInit, Output, TemplateRef, ViewChild } from "@angular/core";
import { Colors } from "../../utils/colors";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { faUserNurse, faCheck, faCircle, faPlus } from "@fortawesome/free-solid-svg-icons";
import { Role } from "../../models/api/Role";
import { RoleSelected } from "../../models/registration/roleSelected.model";

export const NewCardModalComponentConfig = {
  windowClass: 'hh-modal'
};

@Component({
  selector: 'app-new-card-modal',
  templateUrl: './new-card-modal.component.html',
})
export class NewCardModalComponent implements OnChanges, OnInit {

  faCircle = faCircle;
  faUserNurse = faUserNurse;
  faCheck = faCheck;
  faPlus = faPlus;
  colors = Colors;

  @ViewChild('content') modalContent: TemplateRef<any>;
  private modalRef: NgbModalRef;

  closeResult = '';

  @Input() roles: Role[] = [];
  roleSelected: RoleSelected[] = [];

  @Output() onClose = new EventEmitter();
  @Output() onDismiss = new EventEmitter();

  constructor(private readonly modalService: NgbModal) {
  }

  ngOnChanges(): void {
    this.setRoleSelection();
  }

  ngOnInit(): void {
    this.setRoleSelection();
  }

  setRoleSelection() {
    if (this.roles)
      this.roleSelected = this.roles.map((role) => {
        return { role: role, isSelected: false } as RoleSelected
      });
  }

  resetRoleSelection() {
    if (this.roleSelected)
      this.roleSelected = this.roleSelected.map(x => { x.isSelected = false; return x; });
  }

  open() {
    this.modalRef = this.modalService.open(this.modalContent, {
      ariaLabelledBy: 'modal-basic-title',
      windowClass: NewCardModalComponentConfig.windowClass
    });

    this.modalRef.result.then((result: Role[]) => {

      this.onClose.emit(result);
      this.resetRoleSelection();

    }, (error) => {

      this.onDismiss.emit(error);
      this.resetRoleSelection();

    });
  }

  close() {
    this.modalRef.close(this.roleSelected.filter(x => x.isSelected).map(x => { return x.role }));
    window.scrollTo(0,0);
  }

  dismiss() {
    this.modalRef.dismiss('close');
  }
}
