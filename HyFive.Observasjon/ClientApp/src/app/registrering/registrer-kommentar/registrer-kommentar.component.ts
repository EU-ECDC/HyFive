import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { SesjonType } from '../../models/api/SesjonType';
import { PredefinertKommentarerService } from '../../services/data/predefinert-kommentarer.service';


@Component({
  selector: 'app-registrer-kommentar',
  templateUrl: './registrer-kommentar.component.html'
})

export class RegistrerKommentarComponent implements OnInit, OnChanges {

  predefinerteKommentarer: string[];
  kommentar: string = "";
  labelTekst: string = "Kommentar";

  faCommentLines = faCommentDots;

  @Input("kommentarinput") kommentarinput;
  @Input('deaktivert') deaktivert = false;
  @Input('institusjonid') institusjonid;
  @Input("sesjontype") sesjontype: SesjonType;
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
      this.kommentar = this.kommentarinput;
    }
  }

  visKommentarModal(modalName) {
    this.kommentar = this.kommentarinput;
    this.modalService.open(modalName, { windowClass: 'hh-modal' });
  }

  hentPredefinertKommentarer() {
    this.predefinertKommentarerService.hentPredefinertKommentarer(this.institusjonid, this.sesjontype).subscribe(result => {
      this.predefinerteKommentarer = result;
    });
  }

  predfinertKommentarValgt(kommentar: string) {
    this.kommentar = kommentar;
  }

  registrerKommentar() {
    this.kommentarRegistertEvent.emit(this.kommentar);
  }
}
