import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { User } from "src/app/models/api/User";
import { MailValidatorHelper } from "src/app/utils/mail-validator-helper";

@Component({
  selector: 'app-create-observer-form',
  templateUrl: './create-observer-form.component.html'
})
export class CreateObserverFormComponent implements OnInit, OnDestroy {

  @Input() facilityId: 0;
  @Input() observers: User[];
  @Output() createObserverEvent: EventEmitter<User> = new EventEmitter<User>();
  @Output() cancelCreateEvent = new EventEmitter();
  newObserver: User = null;
  mailValidatorHelper;

  constructor() {
    this.mailValidatorHelper = MailValidatorHelper;
  }

  ngOnInit(): void {
      this.createEmptyObserver(); 
  }

  ngOnDestroy(): void {
    this.newObserver = null;
  }

  createEmptyObserver() {
    this.newObserver = {
      id: 0,
      facilityId: this.facilityId,
      lastName: '',
      firstName: '',
      email: '',
      hprNumber: null,
      identityPseudonym: null,
      createdTime: new Date(),
      isDisabled: false
    };
  }

  canCreate() {
    return this.newObserver.firstName.length > 0
      && this.newObserver.lastName.length > 0
      && this.newObserver.email.length > 0
      && this.mailValidatorHelper.validateMail(this.newObserver.email)
      && !this.observers.some(obs => obs?.email == this.newObserver?.email)
      //&& this.filteredObservers.find(fo => fo.firstName == this.newObserver?.firstName && fo.lastName == this.newObserver?.lastName) == undefined
  }

  omitSpecialChar(event) {   
    let k;  
    k = event.charCode;  //         k = event.keyCode;  (Both can be used)
    return((k > 64 && k < 91) || (k > 96 && k < 123) || k == 8 || k == 32 || (k >= 48 && k <= 57)); 
  }

  cancelEdit() {
    this.cancelCreateEvent.emit();
  }

  createObserver() {
    this.createObserverEvent.emit(this.newObserver);
  }
}