import { Component, OnInit } from "@angular/core";
import { NavigationEnd, Router, Scroll } from "@angular/router";
import { Urls } from "./constants/urls";
import { UrlService } from "./services/events/url.service";
import { debounceTime, filter } from "rxjs/operators";
import { ViewportScroller } from "@angular/common";
import { fromEvent, Subscription } from "rxjs";
import { BrowserViewportService } from "./services/events/browser-viewport.service";
import { AuthorizationService } from "./services/data/authorization.service";
import { LoggedInUser } from "./models/api/LoggedInUser";
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
  hasFacilities: boolean = false;

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
          if (user.facilityIds?.length > 0) {
            this.hasFacilities = true;
          } else {
            this.hasFacilities = false;
          }
          if (
            !this.hasFacilities &&
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

  mainMenuToBeDisplayed(): boolean {
  // Show main menu if we are offline and user has logged in-id + selected healthcare facility
    if (
      navigator.onLine == false &&
      localStorage.getItem(Localstoragepaths.LoggedInUserId) != null &&
      localStorage.getItem(Localstoragepaths.SelectedFacility) != null
    ) {
      return true;
    }

  // Otherwise show main menu only if user is an Observer and we are not on the front page.
    return ((this.user?.isObserver || this.user?.isCoordinator || this.user?.isAdmin) && this.hasFacilities);
  }
  
  shouldShowAppBrand() {
    var isRootPage       = window.location.pathname === "/";
    var isLoginPage     = window.location.pathname === "/"+Urls.LoginPageUrl;
    var isHomePage     = window.location.pathname === "/"+Urls.HomePageForObservationUrl;
    return isLoginPage || isRootPage || isHomePage;
  }
}
