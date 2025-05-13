import { Component, HostListener, OnInit } from '@angular/core'; 
import { Router, NavigationEnd, Scroll } from '@angular/router';
import { ViewportScroller } from '@angular/common';
import { Subscription, fromEvent } from 'rxjs';
import { debounceTime, filter } from 'rxjs/operators';
import { BrowserViewportService } from './_common/services/browser-viewport.service';
import { UrlService } from './_common/services/url.service';
import { InstitutionService } from './services/data/institution.service';
import { AuthorizationService } from './_common/services/authorization.service';
import { LoggedInUser } from './models/api/LoggedinUser';
import { KeyEventService, KEY_CODE } from './services/events/key-event.service';
import { UrlPaths } from './_common/konstanter/url-paths';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit { 

  private subscription = new Subscription(); 

  projectName = 'ECDC - Administration';
  isMobile: boolean;
  profilRoute = UrlPaths.profile;


  constructor(
    private router: Router,
    private viewportScroller: ViewportScroller,
    private browserViewportService: BrowserViewportService,
    private urlService: UrlService,
    public authorizationService: AuthorizationService,
    private institutionService: InstitutionService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit() {
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
    fromEvent(window, 'resize')
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
        let selectedInstitutionId = this.institutionService.getSelectedInstitutionId();
        if (selectedInstitutionId === null) {
          this.institutionService.getInstitutions().subscribe((result) => {
            this.institutionService.updateSelectedInstitutionId(result[0].id);
            window.location.reload();
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
