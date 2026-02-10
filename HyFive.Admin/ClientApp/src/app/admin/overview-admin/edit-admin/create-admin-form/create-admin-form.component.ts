import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { CreateAdminRequest } from "src/app/models/api/CreateAdminRequest";
import { User } from "src/app/models/api/User";
import { MailValidatorHelper } from "src/app/utils/mail-validator-helper";

@Component({
  selector: 'app-create-admin-form',
  templateUrl: './create-admin-form.component.html'
})
export class CreateAdminFormComponent implements OnInit {

    @Input() users: User[];
    @Output() CreateAdminEvent: EventEmitter<CreateAdminRequest> = new EventEmitter<CreateAdminRequest>();
    @Output() cancelCreateEvent = new EventEmitter();

    newAdmin: CreateAdminRequest = null;
    mailValidatorHelper;

    constructor() {
        this.mailValidatorHelper = MailValidatorHelper;
    }

    ngOnInit(): void {
       this.createToAdmin(); 
    }

    createToAdmin() {
        this.newAdmin = {
        lastName: '',
        firstName: '',
        email: '',
        identityPseudonym: null,
        } as CreateAdminRequest;
    }

    createAdmin() {
        this.CreateAdminEvent.emit(this.newAdmin)
    }

    cancelEdit() {
        this.cancelCreateEvent.emit();
    }

    canCreate() {
        return this.newAdmin.firstName?.length > 0
          && this.newAdmin.lastName?.length > 0
          && !this.users.some(fc => fc.email == this.newAdmin?.email)
          && this.newAdmin.email?.length > 0
          && this.mailValidatorHelper.validateMail(this.newAdmin?.email)
      }

    omitSpecialChar(event) {   
        let k;  
        k = event.charCode;  //         k = event.keyCode;  (Both can be used)
        return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
    }
}