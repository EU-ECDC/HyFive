import { Component, HostListener, Input } from "@angular/core";
import { faBars, faClipboard, faHospital, faInbox, faUser, faTimes } from "@fortawesome/free-solid-svg-icons";

import { MainMenuItem } from "./main-menu-item.model";
import { Urls } from "../constants/urls";
import { faCircle } from "@fortawesome/free-solid-svg-icons";
import { MainMenuEventService } from "../services/events/main-menu-event.service";
import { MenuParameters } from "../constants/menu-parameters";

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
  

  constructor(private mainMenuEventService: MainMenuEventService){
    this.loadMenuOptions();
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
