import { Component, OnInit, OnDestroy } from '@angular/core';
import { PredefinedComment } from '../../models/api/PredefinedComment';
import { PredefinedCommentsService } from '../../services/data/predefinedComments.service';
import { ToastrService } from 'ngx-toastr';
import { CreatePredefinedCommentRequest } from '../../models/api/CreatePredefinedCommentRequest';
import { KeyEventService } from '../../services/events/key-event.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-edit-predefined-comments',
  templateUrl: './edit-predefined-comments.component.html'
})
export class EditingPredefinedCommentsComponent implements OnInit, OnDestroy {

  newPredefinedComment: CreatePredefinedCommentRequest = this.emptyRequest();
  predefinedComments: PredefinedComment[] = [];
  predefinedCommentAsChanged: PredefinedComment = null;
  loading: boolean = false;

  constructor(
    private readonly predefinedCommentsService: PredefinedCommentsService,
    private readonly toastrService: ToastrService,
    private readonly keyEventService: KeyEventService,
    private readonly translate: TranslateService
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
      (error) => this.toastrService.error(this.translate.instant('An error occurred while loading predefined comments: ') + error?.error.message , '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): CreatePredefinedCommentRequest {
    return {
      comment: null
    }
  }

  createPredefinedComment() {
    this.predefinedCommentsService.createPredefinedComment(this.newPredefinedComment).subscribe(
      (updatedPredefinedtComment) => this.toastrService.success(this.translate.instant('Predefined Comment created')),
      error => this.toastrService.error(this.translate.instant('An error occurred while creating predefined comment: ') + error?.error.message , '', { disableTimeOut: true}),
      () => { this.newPredefinedComment = this.emptyRequest(); this.loadPredefinedComments(); }
    );
  }

  selectedPredefinedComment(predefinedComment: PredefinedComment): void {
    if (this.predefinedCommentAsChanged?.id == predefinedComment.id) return;
    this.predefinedCommentAsChanged = structuredClone(predefinedComment);
  }

  updatePredefinedComment(predefinedComment: PredefinedComment): void {
    this.predefinedCommentsService.updatePredefinedComment(predefinedComment).subscribe(
      (updatedComment) => {
        this.toastrService.success(this.translate.instant("Predefined comment updated"));
        this.loadPredefinedComments();
      },
      error => this.toastrService.error(this.translate.instant('An error occurred while updating predefined comment: ') + error?.error.message , '', { disableTimeOut: true}),
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
