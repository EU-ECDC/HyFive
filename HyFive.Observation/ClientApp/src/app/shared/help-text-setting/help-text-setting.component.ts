import { Component, OnInit } from '@angular/core';
import {HelpTextComponent} from "../help-text/help-text.component";

@Component({
  selector: 'app-help-text-settings',
  templateUrl: './help-text-setting.component.html'
})
export class HelpTextSettingsComponent implements OnInit {

  hasSetHelpTexts: boolean;
  HelpTextsWereReset: boolean;

  constructor() { }

  ngOnInit(): void {
    this.setHasSetHelpTexts();
  }

  markHelpTextAsUnseen(){
    for (let localStorageKey in localStorage) {
      if(localStorageKey.startsWith(HelpTextComponent.HelpTextPrefix)){
        localStorage.removeItem(localStorageKey);
      }
    }
    this.HelpTextsWereReset = true;
  }
  private setHasSetHelpTexts() : void {
    for (let localStorageKey in localStorage) {
      if(localStorageKey.startsWith(HelpTextComponent.HelpTextPrefix)){
        this.hasSetHelpTexts = true;
        return;
      }
    }
  }

}
