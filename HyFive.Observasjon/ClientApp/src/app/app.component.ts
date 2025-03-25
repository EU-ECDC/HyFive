import { Component, HostListener, OnInit } from "@angular/core";
import { NavigationEnd, Router, Scroll } from "@angular/router";
import { Urls } from "./konstanter/urls";
import { UrlService } from "./services/events/url.service";
import { debounceTime, filter } from "rxjs/operators";
import { ViewportScroller } from "@angular/common";
import { fromEvent, Subscription } from "rxjs";
import { BrowserViewportService } from "./services/events/browser-viewport.service";
import { AutoriseringService } from "./services/data/autorisering.service";
import { AuthorizedRole } from "./models/autorisering/authorized-role";
import { InnloggetBruker } from "./models/api/InnloggetBruker";
import { AuthenticationEventService } from "./services/events/authentication-event.service";
import { Localstoragepaths } from "./konstanter/localstoragepaths";

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
})
export class AppComponent implements OnInit {
  private subscription = new Subscription();
  isMobile: boolean;
  erLoggetInn = false;
  bruker: InnloggetBruker;

  siderMedInverterteFarger = [
    Urls.IkkeSendteSesjonerUrl,
    Urls.SendteSesjonerUrl,
  ];

  constructor(
    private router: Router,
    private viewportScroller: ViewportScroller,
    private browserViewportService: BrowserViewportService,
    private urlService: UrlService,
    private autoriseringService: AutoriseringService
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

    this.autoriseringService.erLoggetInn().subscribe((erLoggetInn) => {
      this.erLoggetInn = erLoggetInn;
      if (this.erLoggetInn) {
        this.autoriseringService.getBruker().subscribe((bruker) => {
          this.bruker = bruker;
          if (
            this.bruker.erObservator == false &&
            window.location.pathname !== Urls.LoginsideUrl
          ) {
            this.router.navigate([Urls.LoginsideUrl]);
          }
        });
      } else {
        this.router.navigate([Urls.LoginsideUrl]);
      }
    });
  }

  hovedmenySkalVises(): boolean {
    // Vis hovedmeny hvis vi er offline og bruker har innlogget-id + valgt institusjon
    if (
      navigator.onLine == false &&
      localStorage.getItem(Localstoragepaths.InnloggetBrukerId) != null &&
      localStorage.getItem(Localstoragepaths.ValgtInstitusjon) != null
    ) {
      return true;
    }

    // Ellers vis hovedmeny kun hvis bruker er en observatør og vi ikke står på forsiden.
    return this.bruker?.erObservator;
  }
  
  skalViseAppBrand() {
    var erRotside       = window.location.pathname === "/";
    var erLoginSide     = window.location.pathname === "/"+Urls.LoginsideUrl;
    var erStartside     = window.location.pathname === "/"+Urls.StartsideForObservasjonUrl;
    return erLoginSide || erRotside || erStartside;
  }
}
