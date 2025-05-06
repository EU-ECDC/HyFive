import { Component, OnInit } from '@angular/core';
import {HelpTextComponent} from "../help-text/help-text.component";

@Component({
  selector: 'app-help-text-innstillinger',
  templateUrl: './hjelpetekst-innstillinger.component.html'
})
export class HjelpetekstInnstillingerComponent implements OnInit {

  harSettHjelpetekster: boolean;
  hjelpeteksterBleNullstilt: boolean;

  constructor() { }

  ngOnInit(): void {
    this.setHarSettHjelpetekster();
  }

  markerHjelpeteksterSomUsett(){
    for (let localStorageKey in localStorage) {
      if(localStorageKey.startsWith(HelpTextComponent.HelpTextPrefix)){
        localStorage.removeItem(localStorageKey);
      }
    }
    this.hjelpeteksterBleNullstilt = true;
  }
  private setHarSettHjelpetekster() : void {
    for (let localStorageKey in localStorage) {
      if(localStorageKey.startsWith(HelpTextComponent.HelpTextPrefix)){
        this.harSettHjelpetekster = true;
        return;
      }
    }
  }

}
