import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {SessionOverviewReport} from "../../../models/api/SessionOverviewReport";
import {SessionService} from "../../../services/data/session.service";
import {UpdateSessionRequest} from "../../../models/api/UpdateSessionRequest";
import {ToastrService} from "ngx-toastr";
import {DatoHjelper} from "../../../utils/datohjelper";

@Component({
  selector: 'app-edit-sessionsdata',
  templateUrl: './edit-sessionsdata.component.html'
})
export class EditSessionDataComponent implements OnInit {

  @Input() session: SessionOverviewReport;


  @Input() institutionId: number;
  @Input() canEdit: boolean;

  hasChangedDate: boolean;
  hasChangedComment: boolean;
  editingMode: boolean;

  sessionCopy: SessionOverviewReport;

  constructor(private sessionService : SessionService, private toastrService: ToastrService) {
  }

  ngOnInit(): void {
    this.sessionCopy = JSON.parse(JSON.stringify(this.session));
    this.resetState();
  }

  save() {
    if(this.canSave()){
      var request: Partial<UpdateSessionRequest> = {
        sessionId: this.sessionCopy.id,
        institutionId:  this.institutionId,
      }

      if(this.hasChangedComment){
        request.comment = this.sessionCopy.comment;
      }

      console.log("Just before submitting Request:", request);

      this.sessionService.updateSession(request).subscribe((result) => {
        this.toastrService.success("Session data update was successful");
          if(this.hasChangedComment){
            this.session.comment = this.sessionCopy.comment;
          }
          this.resetState();
        },
        (error) => this.toastrService.error('An error occurred while updating session data: ' + error?.message, '', { disableTimeOut: true})
      );
    }

  }

  private resetState() {
    this.hasChangedComment = false;
    this.hasChangedDate = false;
    this.editingMode = false;
    this.sessionCopy = JSON.parse(JSON.stringify(this.session))
  }

  canSave() {
    return this.hasChangedDate || this.hasChangedComment;
  }
}
