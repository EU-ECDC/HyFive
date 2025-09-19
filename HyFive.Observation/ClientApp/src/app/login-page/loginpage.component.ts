import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../services/data/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { Urls } from '../constants/urls';
import {FiveIndicationsSessionService} from '../services/data/five-indications-session.service';
import {GloveSessionService} from '../services/data/glove-session.service';
import {ProtectiveEquipmentSessionService} from '../services/data/protectiveEquipment-session.service';
import {HandJewelrySessionService} from '../services/data/hand-Jewelry-session.service';
import {ToastrService} from 'ngx-toastr';
import { Facility } from '../models/api/Facility';
import { RequestAboutUserAccessService } from '../services/data/requestAboutUserAccess.service';
import {ClipboardService} from 'ngx-clipboard';
import { CodeWorkCacheService } from '../services/data/codeWork-cache.service';
import { HelpTextComponent } from '../shared/help-text/help-text.component';
import { Localstoragepaths } from '../constants/localstoragepaths';

@Component({
  selector: 'app-loginpage',
  templateUrl: './loginpage.component.html'
})
export class LoginPageComponent implements OnInit, OnDestroy {

  isLoggedIn = false;
  isOnline = false;
  user: LoggedInUser;
  Urls = Urls;
  receivedUserStatusFromServer = false;
  facilities: Facility[];
  selectedFacility: Facility = null;
  showErrorMessage: boolean = false;
  showRequestIsRegistered = false;
  facility: Facility;
  showRequestAwaitingApproval = false;
  showRequestRegistration = true;
  isShowPseudonym = false;

  constructor(
    private authorizationService: AuthorizationService,
    private fiveIndicationsSessionService: FiveIndicationsSessionService,
    private gloveSessionService: GloveSessionService,
    private protectiveEquipmentSessionService: ProtectiveEquipmentSessionService,
    private handJewelrySessionService: HandJewelrySessionService,
    private requestAboutUserAccessService: RequestAboutUserAccessService,
    private toastrService: ToastrService,
    private clipboardService: ClipboardService,
    private codeWorkCacheService: CodeWorkCacheService
  ) { }

  ngOnInit(): void {
    this.authorizationService.isLoggedIn().subscribe((isLoggedIn) => {
      this.receivedUserStatusFromServer = true;
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn) {
        this.authorizationService.getUser().subscribe(user => {
          this.user = user;

          this.requestAboutUserAccessService.fetchRequestSentAlready().subscribe(
            (forsporsel) => {
              if(forsporsel != null)
              {
                this.requestAboutUserAccessService.getFacility(forsporsel.facilityId).subscribe(
                  (facility) => {
                    if(facility != null)
                    {
                      this.facility = facility;
                      this.showRequestAwaitingApproval = true;
                      this.showRequestRegistration = false;
                    }
                  });
              }
          });

          // this does an initial load of code, so that the cache is filled and you can work offline          
          // this.codeWorkCacheService.loadCodeworks();
        });

        this.requestAboutUserAccessService.getFacilities().subscribe(
          (facilities) => {
            this.facilities = facilities;
          }
        );
      }
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  logout() {
    this.unregisterSw().then(() => {
      window.location.href = "/account/logout";
    });
  }

  unregisterSw() : Promise<any> {
    return navigator.serviceWorker.getRegistrations().then(function(registrations) {
      for(let registration of registrations) {
        registration.unregister()
      }}).catch(function(err) {
    });

  }

  hasLocalSessionsLying(): boolean {
    var hasGloveSessions = this.gloveSessionService.numberOfSessions() > 0;
    var hasProtectiveEquipmentSessions = this.protectiveEquipmentSessionService.numberOfSessions() > 0;
    var hasFiveIndicationsSessions = this.fiveIndicationsSessionService.numberOfSessions() > 0;
    var hasHandJewelrySessions = this.handJewelrySessionService.numberOfSessions() > 0;
    return hasGloveSessions || hasProtectiveEquipmentSessions || hasFiveIndicationsSessions || hasHandJewelrySessions;
  }

  receivedInternetStatus(isOnline: boolean) {
    this.isOnline = isOnline;
  }

  sendRequest() {
    if(this.selectedFacility)
    {
      var newRequestAboutUserAccess = {
        facilityId: this.selectedFacility?.id,
        userFirstName: this.user.firstName,
        userLastName: this.user.lastName,
        hprNumber: this.user.hprNumber,
        identityPseudonym: this.user.identityPseudonym,
        email: this.user.email
      }
      this.requestAboutUserAccessService.sendRequestAboutUserAccess(newRequestAboutUserAccess).subscribe(
        (isUserCreated) => {
          if (isUserCreated)
          {
            this.showRequestIsRegistered = true;
            this.showRequestRegistration = false;
          }
          else
            this.showErrorMessage = true;
        },
        (error) =>{
          this.showErrorMessage = true;
        }
      );
    }
  }

  copyPseudonymClick() {
    this.clipboardService.copy(this.user?.identityPseudonym);
    this.toastrService.success('Pseudonym copied to clipboard and can be pasted elsewhere using Paste (CTRL+V)');
  }

  showPseudonym() : void {
    this.isShowPseudonym = true;
  }

  closeInfoModal($event: boolean) {
    this.isShowPseudonym = $event;
  }
}
