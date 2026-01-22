import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { faCommentDots } from '@fortawesome/free-regular-svg-icons';
import { SessionType } from '../../models/api/SessionType';
import { PredefinedCommentsService } from '../../services/data/predefined-comments.service';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-register-comment',
  templateUrl: './register-comment.component.html'
})

export class RegisterCommentComponent implements OnInit, OnChanges {

  predefinedComments: string[];
  comment: string = "";
  labelText = { name: "Comment", value: "Comment" };

  faCommentLines = faCommentDots;

  @Input() commentInput;
  @Input() disabled = false;
  @Input() facilityid;
  @Input() sessiontype: SessionType;
  @Output() commentRegisteredEvent = new EventEmitter<string>();

  constructor(
    private readonly modalService: NgbModal,
    private readonly predefinedCommentsService: PredefinedCommentsService,
    private readonly translate: TranslateService) {
  }

  ngOnInit(): void {
    if (this.facilityid && this.sessiontype)
      this.getPredefinedComments();

    this.translate.get(this.labelText.name).pipe(take(1)).subscribe(_res => {
      this.labelText.value = this.translate.instant(this.labelText.name);
    });
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

  // BLOCK ALL EMOJI
  private readonly emojiRegex = /[\p{Emoji}\p{Extended_Pictographic}]/gu;

  stripEmoji() {
    if (this.comment) {
      this.comment = this.comment.replace(this.emojiRegex, '');
    }
  }

  // BLOCK IMAGE PASTE
  blockImagePaste(event: ClipboardEvent) {
    if (!event.clipboardData) return;

    const hasImage = Array.from(event.clipboardData.items)
      .some(item => item.type.startsWith('image/'));

    if (hasImage) {
      event.preventDefault();
      alert(this.translate.instant('Images are not allowed.'));
    }
  }
}
