import { Component, HostListener, OnInit } from '@angular/core'; 
import { Router, NavigationEnd, Scroll } from '@angular/router';
import { ViewportScroller } from '@angular/common';
import { Subscription, fromEvent } from 'rxjs';
import { debounceTime, filter } from 'rxjs/operators';
import { BrowserViewportService } from './_felles/services/browser-viewport.service';
import { UrlService } from './_felles/services/url.service';
import { InstitusjonService } from './services/data/institusjon.service';
import { AuthorizationService } from './_felles/services/authorization.service';
import { InnloggetBruker } from './models/api/InnloggetBruker';
import { KeyEventService, KEY_CODE } from './services/events/key-event.service';
import { UrlPaths } from './_felles/konstanter/url-paths';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit { 

  private subscription = new Subscription(); 

  prosjektnavn = 'NOST - Administrasjon';
  isMobile: boolean;
  profilRoute = UrlPaths.profil;


  constructor(
    private router: Router,
    private viewportScroller: ViewportScroller,
    private browserViewportService: BrowserViewportService,
    private urlService: UrlService,
    public authorizationService: AuthorizationService,
    private institusjonService: InstitusjonService,
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

    this.authorizationService.getBruker().subscribe((bruker: InnloggetBruker) => {
      if (bruker.erKoordinator) {
        let valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId();
        if (valgtInstitusjonId === null) {
          this.institusjonService.hentInstitusjoner().subscribe((resultat) => {
            this.institusjonService.oppdaterValgtInstitusjonId(resultat[0].id);
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
