import { Component, HostListener, OnInit } from "@angular/core";
import { NavigationEnd, Router, Scroll } from "@angular/router";
import { Urls } from "./constants/urls";
import { UrlService } from "./services/events/url.service";
import { debounceTime, filter } from "rxjs/operators";
import { ViewportScroller } from "@angular/common";
import { fromEvent, Subscription } from "rxjs";
import { BrowserViewportService } from "./services/events/browser-viewport.service";
import { AuthorizationService } from "./services/data/authorization.service";
import { AuthorizedRole } from "./models/autorisering/authorized-role";
import { LoggedInUser } from "./models/api/LoggedInUser";
import { AuthenticationEventService } from "./services/events/authentication-event.service";
import { Localstoragepaths } from "./constants/localstoragepaths";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
})
export class AppComponent implements OnInit {
  private subscription = new Subscription();
  isMobile: boolean;
  isLoggedIn = false;
  user: LoggedInUser;

  siderMedInverterteFarger = [
    Urls.NotSentSessionsUrl,
    Urls.SentSessionsUrl,
  ];

  constructor(
    private router: Router,
    private viewportScroller: ViewportScroller,
    private browserViewportService: BrowserViewportService,
    private urlService: UrlService,
    private authorizationService: AuthorizationService
  ) {}

  ngOnInit(): void {
    this.router.events
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.urlService.updateAfterNavigationEnd(
          this.router.parseUrl(event.urlAfterRedirects)
        );
      });

    this.router.events
      .pipe(filter((event) => event instanceof Scroll))
      .subscribe((event: Scroll) => {
        if (event.anchor) {
          this.viewportScroller.scrollToAnchor(event.anchor);
        }
      });

    this.browserViewportService.updateResponsiveProperties();
    fromEvent(window, "resize")
      .pipe(debounceTime(200))
      .subscribe(() => {
        this.browserViewportService.updateResponsiveProperties();
      });

    this.subscription.add(
      this.browserViewportService.isMobile$.subscribe((isMobile) => {
        this.isMobile = isMobile;
      })
    );

    this.authorizationService.isLoggedIn().subscribe((isLoggedIn) => {
      this.isLoggedIn = isLoggedIn;
      if (this.isLoggedIn) {
        this.authorizationService.getUser().subscribe((user) => {
          this.user = user;
          if (
            this.user.isObserver == false &&
            window.location.pathname !== Urls.LoginPageUrl
          ) {
            this.router.navigate([Urls.LoginPageUrl]);
          }
        });
      } else {
        this.router.navigate([Urls.LoginPageUrl]);
      }
    });
  }

  hovedmenySkalVises(): boolean {
    // Vis hovedmeny hvis vi er offline og user har innlogget-id + valgt institusjon
    if (
      navigator.onLine == false &&
      localStorage.getItem(Localstoragepaths.LoggedInUserId) != null &&
      localStorage.getItem(Localstoragepaths.SelectedInstitution) != null
    ) {
      return true;
    }

    // Ellers vis hovedmeny kun hvis user er en observatør og vi ikke står på forsiden.
    return this.user?.isObserver;
  }
  
  skalViseAppBrand() {
    var erRotside       = window.location.pathname === "/";
    var erLoginSide     = window.location.pathname === "/"+Urls.LoginPageUrl;
    var erStartside     = window.location.pathname === "/"+Urls.HomePageForObservationUrl;
    return erLoginSide || erRotside || erStartside;
  }
}
