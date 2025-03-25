import { Component, HostListener, Input } from "@angular/core";
import { faBars, faClipboard, faHospital, faInbox, faUser, faTimes } from "@fortawesome/free-solid-svg-icons";

import { MainMenuItem } from "./main-menu-item.model";
import { Urls } from "../konstanter/urls";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { MainMenuEventService } from "../services/events/main-menu-event.service";
import { MenuParameters } from "../konstanter/menu-parameters";

@Component({
  selector: "app-main-menu",
  templateUrl: "./main-menu.component.html",
})
export class MainMenuComponent {
  @Input() prosjektnavn: string;

  faBars = faBars;
  faTimes = faTimes;
  faCircle = faCircle;
  mainMenuIsOpen = false;
  screenWidth: number = window.innerWidth;
  MenuParameters = MenuParameters;
  mainMenuItems: MainMenuItem[] = [];
  

  constructor(private mainMenuEventService: MainMenuEventService)
  {
    this.lagMenyValg();
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
    return this.isMobileMenu() == false;
  }

  private lagMenyValg(): void {
    this.mainMenuItems = [
      {
        name: "Start observasjon",
        routerLink: `/${Urls.StartsideForObservasjonUrl}`,
        exactRouteMatch: true,
        icon: faHospital
      },
      {
        name: "Ikke sendte sesjoner",
        routerLink: `/${Urls.IkkeSendteSesjonerUrl}`,
        exactRouteMatch: false,
        icon: faClipboard
      },
      {
        name: "Sendte sesjoner",
        routerLink: `/${Urls.SendteSesjonerUrl}`,
        exactRouteMatch: false,
        icon: faInbox
      },
      {
        name: "Profil",
        routerLink: `/${Urls.ProfilUrl}`,
        exactRouteMatch: true,
        icon: faUser
      },
    ];
  }
}
