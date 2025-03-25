import { Component, OnInit } from '@angular/core';
import {HjelpetekstComponent} from "../../shared/hjelpetekst/hjelpetekst.component";

@Component({
  selector: 'app-hjelpetekst-innstillinger',
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
      if(localStorageKey.startsWith(HjelpetekstComponent.HjelpetekstPrefiks)){
        localStorage.removeItem(localStorageKey);
      }
    }
    this.hjelpeteksterBleNullstilt = true;
  }
  private setHarSettHjelpetekster() : void {
    for (let localStorageKey in localStorage) {
      if(localStorageKey.startsWith(HjelpetekstComponent.HjelpetekstPrefiks)){
        this.harSettHjelpetekster = true;
        return;
      }
    }
  }

}
