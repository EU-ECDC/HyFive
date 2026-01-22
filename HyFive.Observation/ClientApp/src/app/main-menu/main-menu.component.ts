import { Component, HostListener, Input } from "@angular/core";
import { faBars, faCircle, faClipboard, faHospital, faInbox, faUser, faTimes } from "@fortawesome/free-solid-svg-icons";

import { MainMenuItem } from "./main-menu-item.model";
import { Urls } from "../constants/urls";
import { MainMenuEventService } from "../services/events/main-menu-event.service";
import { MenuParameters } from "../constants/menu-parameters";
import { TranslateService } from "@ngx-translate/core";
import { take } from "rxjs/operators";

@Component({
  selector: "app-main-menu",
  templateUrl: "./main-menu.component.html",
})
export class MainMenuComponent {
  @Input() projectName: string;

  faBars = faBars;
  faTimes = faTimes;
  faCircle = faCircle;
  mainMenuIsOpen = false;
  screenWidth: number = window.innerWidth;
  MenuParameters = MenuParameters;
  mainMenuItems: MainMenuItem[] = [];
  menu = { name: "menu", value: "Menu" };
  close = { name: "close", value: "Close" };
  

  constructor(private readonly mainMenuEventService: MainMenuEventService,
              private readonly translate: TranslateService
  ){

    this.translate.get([...this.menu.name, ...this.mainMenuItems.map(it => it.name)]).pipe(take(1)).subscribe(res => {
    this.menu.value = translate.instant(this.menu.name);
    this.close.value = translate.instant(this.close.name);
        this.loadMenuOptions();
        this.mainMenuItems = this.mainMenuItems.map(mOpt => ({
          ...mOpt,
          name: this.translate.instant(mOpt.name)
        }));
    });
  }



  mainMenuClose(): void {
    this.mainMenuIsOpen = false;
  }

  mainMenuToggle(): void {
    this.mainMenuIsOpen = !this.mainMenuIsOpen;
  }

  @HostListener("window:resize", ["$event"])
  getScreenWidth(event?) {
    this.screenWidth = window.innerWidth;
  }

  isMobileMenu(): boolean {
    return this.screenWidth < MenuParameters.mobileMenuWidth;
  }
  isDesktopMenu() {
    return this.isMobileMenu() === false;
  }

  private loadMenuOptions(): void {
      this.mainMenuItems = [
        {
          name: "Start Observation",
          routerLink: `/${Urls.HomePageForObservationUrl}`,
          exactRouteMatch: true,
          icon: faHospital
        },
        {
          name: "Unsent Sessions",
          routerLink: `/${Urls.NotSentSessionsUrl}`,
          exactRouteMatch: false,
          icon: faClipboard
        },
        {
          name: "Sent Sessions",
          routerLink: `/${Urls.SentSessionsUrl}`,
          exactRouteMatch: false,
          icon: faInbox
        },
        {
          name: "Profile",
          routerLink: `/${Urls.ProfileUrl}`,
          exactRouteMatch: true,
          icon: faUser
        },
      ];
  }
}
