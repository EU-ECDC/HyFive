import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { SessionType } from '../../models/api/SessionType';
import { PredefinedCommentsService } from '../../services/data/predefined-comments.service';


@Component({
  selector: 'app-register-comment',
  templateUrl: './register-comment.component.html'
})

export class RegisterCommentComponent implements OnInit, OnChanges {

  predefinedComments: string[];
  comment: string = "";
  labelText: string = "Comment";

  faCommentLines = faCommentDots;

  @Input("commentInput") commentInput;
  @Input('disabled') disabled = false;
  @Input('facilityid') facilityid;
  @Input("sessiontype") sessiontype: SessionType;
  @Output() commentRegisteredEvent = new EventEmitter<string>();

  constructor(
    private modalService: NgbModal,
    private predefinedCommentsService: PredefinedCommentsService) {
  }

  ngOnInit(): void {
    if (this.facilityid && this.sessiontype)
      this.getPredefinedComments();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.commentInput?.currentValue !== changes.commentInput?.previousValue) {
      this.comment = this.commentInput;
    }
  }

  showCommentModal(modalName) {
    this.comment = this.commentInput;
    this.modalService.open(modalName, { windowClass: 'hh-modal' });
  }

  getPredefinedComments() {
    this.predefinedCommentsService.getPredefinedComments(this.facilityid, this.sessiontype).subscribe(result => {
      this.predefinedComments = result;
    });
  }

  predefinedCommentSelected(comment: string) {
    this.comment = comment;
  }

  registerComment() {
    this.commentRegisteredEvent.emit(this.comment);
  }
}
