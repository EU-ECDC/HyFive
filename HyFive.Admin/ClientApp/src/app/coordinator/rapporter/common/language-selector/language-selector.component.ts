import { Component } from '@angular/core';
import { LanguageService } from 'src/app/_common/services/language-service';

@Component({
  selector: 'app-language-selector',
  templateUrl: './language-selector.component.html',
  styleUrls: ['./language-selector.component.scss']
})
export class LanguageSelectorComponent {

  availableLanguages = [
    { code: 'en', label: 'English' },
    // { code: 'it', label: 'Italiano' },
    { code: 'el', label: 'Ελληνικά' }
  ];
  selectedLanguage;

  constructor(private readonly languageService: LanguageService) {
    let code = this.languageService.getCurrentLanguage();
    this.selectedLanguage = this.availableLanguages.find(lang => lang.code == code);
  }

  changeLanguage() {
    if (this.selectedLanguage)
    this.languageService.setLanguage(this.selectedLanguage.code);
  }
  
}