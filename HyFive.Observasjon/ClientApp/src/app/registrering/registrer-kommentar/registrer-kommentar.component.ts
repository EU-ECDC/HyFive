import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { SessionType } from '../../models/api/SessionType';
import { PredefinertKommentarerService } from '../../services/data/predefinert-kommentarer.service';


@Component({
  selector: 'app-registrer-kommentar',
  templateUrl: './registrer-kommentar.component.html'
})

export class RegistrerKommentarComponent implements OnInit, OnChanges {

  predefinerteKommentarer: string[];
  comment: string = "";
  labelTekst: string = "Kommentar";

  faCommentLines = faCommentDots;

  @Input("kommentarinput") kommentarinput;
  @Input('deaktivert') deaktivert = false;
  @Input('institusjonid') institusjonid;
  @Input("sesjontype") sesjontype: SessionType;
  @Output() kommentarRegistertEvent = new EventEmitter<string>();

  constructor(
    private modalService: NgbModal,
    private predefinertKommentarerService: PredefinertKommentarerService) {
  }

  ngOnInit(): void {
    if (this.institusjonid && this.sesjontype)
      this.hentPredefinertKommentarer();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.kommentarinput?.currentValue !== changes.kommentarinput?.previousValue) {
      this.comment = this.kommentarinput;
    }
  }

  visKommentarModal(modalName) {
    this.comment = this.kommentarinput;
    this.modalService.open(modalName, { windowClass: 'hh-modal' });
  }

  hentPredefinertKommentarer() {
    this.predefinertKommentarerService.hentPredefinertKommentarer(this.institusjonid, this.sesjontype).subscribe(result => {
      this.predefinerteKommentarer = result;
    });
  }

  predfinertKommentarValgt(comment: string) {
    this.comment = comment;
  }

  registrerKommentar() {
    this.kommentarRegistertEvent.emit(this.comment);
  }
}
