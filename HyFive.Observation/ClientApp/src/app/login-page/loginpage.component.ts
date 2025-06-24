import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../services/data/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { Urls } from '../constants/urls';
import {FiveIndicationsSessionService} from '../services/data/five-indications-session.service';
import {GloveSessionService} from '../services/data/glove-session.service';
import {ProtectiveEquipmentSessionService} from '../services/data/protectiveEquipment-session.service';
import {HandJewelrySessionService} from '../services/data/hand-Jewelry-session.service';
import {ToastrService} from 'ngx-toastr';
import { Institution } from '../models/api/Institution';
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
  institutions: Institution[];
  selectedInstitution: Institution = null;
  showErrorMessage: boolean = false;
  showRequestIsRegistered = false;
  institution: Institution;
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
                this.requestAboutUserAccessService.getInstitution(forsporsel.institutionId).subscribe(
                  (institution) => {
                    if(institution != null)
                    {
                      this.institution = institution;
                      this.showRequestAwaitingApproval = true;
                      this.showRequestRegistration = false;
                    }
                  });
              }
          });

          // this does an initial load of code, so that the cache is filled and you can work offline          
          // this.codeWorkCacheService.loadCodeworks();
        });

        this.requestAboutUserAccessService.getInstitutions().subscribe(
          (institutions) => {
            this.institutions = institutions;
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
    var hasFourIndicationsSessions = this.fiveIndicationsSessionService.numberOfSessions() > 0;
    var hasHandJewelrySessions = this.handJewelrySessionService.numberOfSessions() > 0;
    return hasGloveSessions || hasProtectiveEquipmentSessions || hasFourIndicationsSessions || hasHandJewelrySessions;
  }

  receivedInternetStatus(isOnline: boolean) {
    this.isOnline = isOnline;
  }

  sendRequest() {
    if(this.selectedInstitution)
    {
      var newRequestAboutUserAccess = {
        institutionId: this.selectedInstitution?.id,
        userFirstName: this.user.firstName,
        userLastName: this.user.lastName,
        hprNumber: this.user.hprNumber,
        identityPseudonym: this.user.identityPseudonym
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
