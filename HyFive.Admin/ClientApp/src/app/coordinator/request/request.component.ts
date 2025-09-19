import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { FacilityService } from 'src/app/services/data/facility.service';
import { RequestAboutUserAccess } from "../../models/api/RequestAboutUserAccess";
import { RequestAboutUserAccessService } from "../../services/data/requestAboutUserAccess.service";
import { RequestStatus } from 'src/app/models/api/RequestStatus';

@Component({
  selector: 'app-request',
  templateUrl: './request.component.html'
})
export class RequestComponent implements OnInit, OnDestroy {

  requests: RequestAboutUserAccess[];
  showAllRequest: boolean = false;
  requestStatus = RequestStatus;

  constructor(
    private requestAboutUserAccessService: RequestAboutUserAccessService,
    private facilityService: FacilityService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadRequestsAwaitingApproval();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  loadAllRequests() {
    this.showAllRequest = true;
    var facilityId = this.facilityService.getSelectedFacilityId();
    this.requestAboutUserAccessService.getAllRequests(facilityId).subscribe(
      (requests) => this.requests = requests,
      (error) => this.toastrService.error('An error occurred while loading user access requests: ' + error?.message, '', { disableTimeOut: true})
    );
  }

  loadRequestsAwaitingApproval() {
    this.showAllRequest = false;
    var facilityId = this.facilityService.getSelectedFacilityId();
    this.requestAboutUserAccessService.getRequestsAwaitingApproval(facilityId).subscribe(
      (requests) => this.requests = requests,
      (error) => this.toastrService.error('An error occurred while loading user access requests: ' + error?.message, '', { disableTimeOut: true})
    );
  }

  approveRequest(requestId: number) {
    this.requestAboutUserAccessService.approveRequest(requestId).subscribe(
      () => {
        if(this.showAllRequest)
          this.loadAllRequests();
        else
          this.loadRequestsAwaitingApproval();
        this.toastrService.success("Request approved");
      },
      (error) => this.toastrService.error('An error occurred while approving the user access request: ' + error?.message, '', { disableTimeOut: true})
    );
  }

  rejectRequest(requestId: number) {
    this.requestAboutUserAccessService.rejectRequest(requestId).subscribe(
      () => {
        if(this.showAllRequest)
          this.loadAllRequests();
        else
          this.loadRequestsAwaitingApproval();
        this.toastrService.success("Request has been rejected");
      },
      (error) => this.toastrService.error('An error occurred while rejecting the user access request: ' + error?.message, '', { disableTimeOut: true})
    );
  }

}
