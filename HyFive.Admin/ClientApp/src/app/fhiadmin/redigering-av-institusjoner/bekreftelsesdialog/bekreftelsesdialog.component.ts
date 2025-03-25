import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-bekreftelsesdialog',
  templateUrl: 'bekreftelsesdialog.component.html',
})

export class BekrefelsesdialogComponent {

  @Input() knappTekst: string;
  @Input() dialogmelding: string;
  @Input() disabled = false;
  @Input() buttonClass = 'btn btn-sm fhi-btn-secondary';
  @Input() skalBrukeIkon = false;
  @Output() bekreftetEvent = new EventEmitter();

  constructor() {}

  bekreft() {
    this.bekreftetEvent.emit();
  }
}
