import { Component, OnInit, OnDestroy } from '@angular/core';
import { PredefinedComment } from '../../models/api/PredefinedComment';
import { PredefinedCommentsService } from '../../services/data/predefinedComments.service';
import { ToastrService } from 'ngx-toastr';
import { OpprettPredefinertKommentarRequest } from '../../models/api/OpprettPredefinertKommentarRequest';
import { KeyEventService } from '../../services/events/key-event.service';

@Component({
  selector: 'app-edit-predefined-comments',
  templateUrl: './edit-predefined-comments.component.html'
})
export class EditingPredefinedCommentsComponent implements OnInit, OnDestroy {

  newPredefinedComment: OpprettPredefinertKommentarRequest = this.emptyRequest();
  predefinedComments: PredefinedComment[] = [];
  predefinedCommentAsChanged: PredefinedComment = null;
  loading: boolean = false;

  constructor(
    private predefinedCommentsService: PredefinedCommentsService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.predefinedCommentAsChanged = null;
    });

    this.loadPredefinedComments();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  loadPredefinedComments() {
    this.loading = true;
    this.predefinedCommentsService.getPredefinedComments().subscribe(
      (predefinedComments) => {
        this.loading = false;
        this.predefinedComments = predefinedComments
      },
      (error) => this.toastrService.error('Det oppstod en feil under innlasting av predefinerte kommentarer: ' + error?.message , '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): OpprettPredefinertKommentarRequest {
    return {
      comment: null
    }
  }

  createPredefinedComment() {
    this.predefinedCommentsService.createPredefinedComment(this.newPredefinedComment).subscribe(
      (updatedPredefinedtComment) => this.toastrService.success('Predefined Comment created'),
      error => this.toastrService.error('An error occurred while creating predefined comment: ' + error?.message , '', { disableTimeOut: true}),
      () => { this.newPredefinedComment = this.emptyRequest(); this.loadPredefinedComments(); }
    );
  }

  selectedPredefinedComment(predefinedComment: PredefinedComment): void {
    if (this.predefinedCommentAsChanged?.id == predefinedComment.id) return;
    this.predefinedCommentAsChanged = JSON.parse(JSON.stringify(predefinedComment));
  }

  updatePredefinedComment(predefinedComment: PredefinedComment): void {
    this.predefinedCommentsService.updatePredefinedComment(predefinedComment).subscribe(
      (updatedComment) => {
        this.toastrService.success("Predefined comment updated");
        this.loadPredefinedComments();
      },
      error => this.toastrService.error('An error occurred while updating predefined comment: ' + error?.error , '', { disableTimeOut: true}),
      () => this.predefinedCommentAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.predefinedCommentAsChanged = null;
  }
}
