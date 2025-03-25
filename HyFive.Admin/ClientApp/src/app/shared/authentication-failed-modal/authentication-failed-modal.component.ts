import {Component, OnInit, ViewChild, ViewContainerRef} from '@angular/core';
import {AuthenticationEventService} from '../../services/events/authentication-event.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'app-authentication-failed-modal',
  templateUrl: './authentication-failed-modal.component.html'
})
export class AuthenticationFailedModalComponent implements OnInit {
  @ViewChild("content")
  content : ViewContainerRef;

  constructor(private authenticationEventService: AuthenticationEventService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.authenticationEventService.authenticationFailedEvent.subscribe(authenticationFailed => {
      if(authenticationFailed){
        this.modalService.open(this.content);
      }
    })
  }

  open(content) {
    this.modalService.open(content, {ariaLabelledBy: 'modal-basic-title'});
  }

  loggInn() {
    window.location.reload(); // Refresh vil trigge re-login
  }
}
