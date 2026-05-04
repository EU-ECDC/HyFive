import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../services/data/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { Urls } from '../constants/urls';
import {ToastrService} from 'ngx-toastr';

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
  hasFacilities: boolean = false;

  constructor(
    private readonly authorizationService: AuthorizationService,
    private readonly toastrService: ToastrService
  ) { }

  ngOnInit(): void {
    this.authorizationService.isLoggedIn().subscribe((isLoggedIn) => {
      this.receivedUserStatusFromServer = true;
      this.isLoggedIn = isLoggedIn;
      if (isLoggedIn) {
        this.authorizationService.getUser().subscribe(user => {
          this.user = user;
          if (user.organisationUnits?.length > 0) {
            this.hasFacilities = true;
          } else {
            this.hasFacilities = false;
          }
        });
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
    if ('serviceWorker' in navigator && navigator.serviceWorker.getRegistrations) {
      return navigator.serviceWorker.getRegistrations().then(function(registrations) {
        for(let registration of registrations) {
          registration.unregister();
        } 
      }).catch(function(_) {});
    }
    return Promise.resolve();
  }

  receivedInternetStatus(isOnline: boolean) {
    this.isOnline = isOnline;
  }
}
