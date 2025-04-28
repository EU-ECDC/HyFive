import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserService } from '../../services/data/user.service';

@Component({
  selector: 'app-pseudonym-dialog',
  templateUrl: 'pseudonym-dialog.component.html',
})

export class PseudonymDialogComponent {

  @Input() identityPseudonym: string;
  @Input() text: string;
  @Input() editable: boolean = false;
  @Output() identityPseudonymChanged = new EventEmitter<string>();


  constructor(private userService: UserService) { }

  ok() {
    if (this.userService.isValidPseudonym(this.identityPseudonym)) {
      this.identityPseudonymChanged.emit(this.identityPseudonym);
    }
  }

  isOk(): boolean {
    return this.userService.isValidPseudonym(this.identityPseudonym);
}
}
