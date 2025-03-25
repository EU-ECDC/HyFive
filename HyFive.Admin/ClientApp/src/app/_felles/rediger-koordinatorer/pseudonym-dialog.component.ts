import { Component, Input, Output, EventEmitter } from '@angular/core';
import { BrukerService } from '../../services/data/bruker.service';

@Component({
  selector: 'app-pseudonym-dialog',
  templateUrl: 'pseudonym-dialog.component.html',
})

export class PseudonymDialogComponent {

  @Input() identPseudonym: string;
  @Input() tekst: string;
  @Input() redigerbar: boolean = false;
  @Output() identPseudonymEndret = new EventEmitter<string>();


  constructor(private brukerService: BrukerService) { }

  ok() {
    if (this.brukerService.erGyldigPseudonym(this.identPseudonym)) {
      this.identPseudonymEndret.emit(this.identPseudonym);
    }
  }

  erOk(): boolean {
    return this.brukerService.erGyldigPseudonym(this.identPseudonym);
}
}
