import { Component, HostListener, OnInit } from '@angular/core'; 
import { Router, NavigationEnd, Scroll } from '@angular/router';
import { ViewportScroller } from '@angular/common';
import { Subscription, fromEvent } from 'rxjs';
import { debounceTime, filter } from 'rxjs/operators';
import { BrowserViewportService } from './_common/services/browser-viewport.service';
import { UrlService } from './_common/services/url.service';
import { AuthorizationService } from './_common/services/authorization.service';
import { LoggedInUser } from './models/api/LoggedinUser';
import { KeyEventService, KEY_CODE } from './services/events/key-event.service';
import { UrlPaths } from './_common/constants/url-paths';
import { FacilityService } from './services/data/facility.service';
import { DateAdapter } from '@angular/material/core';
import { LanguageService } from './_common/services/language-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit { 

  private readonly subscription = new Subscription(); 

  projectName = "ECDC - Administration";
  isMobile: boolean;
  profilRoute = UrlPaths.profile;


  constructor(
    private readonly router: Router,
    private readonly viewportScroller: ViewportScroller,
    private readonly browserViewportService: BrowserViewportService,
    private readonly urlService: UrlService,
    public authorizationService: AuthorizationService,
    private readonly facilityService: FacilityService,
    private readonly keyEventService: KeyEventService,
    private readonly dateAdapter: DateAdapter<any>,
    private readonly languageService: LanguageService
  ) { }

  ngOnInit() {
    this.dateAdapter.setLocale(this.languageService.getCurrentLanguage());
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.urlService.updateAfterNavigationEnd(this.router.parseUrl(event.urlAfterRedirects));
      }); 

    this.router.events
      .pipe(filter(event => event instanceof Scroll))
      .subscribe((event: Scroll) => {
        if (event.anchor) {
          this.viewportScroller.scrollToAnchor(event.anchor);
        }
      });

    this.browserViewportService.updateResponsiveProperties();
    fromEvent(globalThis, 'resize')
      .pipe(debounceTime(200))
      .subscribe(() => {
        this.browserViewportService.updateResponsiveProperties();
      });

    this.subscription.add(this.browserViewportService.isMobile$
      .subscribe(isMobile => {
        this.isMobile = isMobile;
      }));

    this.authorizationService.getUser().subscribe((user: LoggedInUser) => {
      if (user.isCoordinator) {
        let selectedFacilityId = this.facilityService.getSelectedFacilityId();
        if (selectedFacilityId === null) {
          this.facilityService.getFacilities().subscribe((result) => {
            this.facilityService.updateSelectedFacilityId(result[0].id);
            globalThis.location.reload();
          });
        }
      }
    });
  }

  @HostListener('window:keyup', ['$event'])
  keyEvent(event: KeyboardEvent) {
    if (event.keyCode == KEY_CODE.ESC) {
      this.keyEventService.escapeKeyEvent.emit(event);
    }
  }
}
