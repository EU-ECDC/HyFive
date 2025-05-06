import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { SessionType } from '../../models/api/SessionType';
import { PredefinedCommentsService } from '../../services/data/predefined-comments.service';


@Component({
  selector: 'app-registrer-kommentar',
  templateUrl: './registrer-kommentar.component.html'
})

export class RegistrerKommentarComponent implements OnInit, OnChanges {

  predefinerteKommentarer: string[];
  comment: string = "";
  labelTekst: string = "Comment";

  faCommentLines = faCommentDots;

  @Input("kommentarinput") kommentarinput;
  @Input('deaktivert') deaktivert = false;
  @Input('institutionid') institutionid;
  @Input("sessiontype") sessiontype: SessionType;
  @Output() kommentarRegistertEvent = new EventEmitter<string>();

  constructor(
    private modalService: NgbModal,
    private predefinedCommentsService: PredefinedCommentsService) {
  }

  ngOnInit(): void {
    if (this.institutionid && this.sessiontype)
      this.getPredefinedComments();
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

  getPredefinedComments() {
    this.predefinedCommentsService.getPredefinedComments(this.institutionid, this.sessiontype).subscribe(result => {
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
