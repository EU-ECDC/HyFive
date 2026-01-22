import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  constructor(private readonly translate: TranslateService) {}

  initLanguage() {
    const savedLang = localStorage.getItem('lang');
    const defaultLang = savedLang || 'en';
    this.translate.setDefaultLang('en');
    this.translate.use(defaultLang);
  }

  setLanguage(lang: string) {
    this.translate.use(lang);
    localStorage.setItem('lang', lang);
    globalThis.location.reload();
  }

  getCurrentLanguage(): string {
    return this.translate.currentLang || this.translate.getDefaultLang() || 'en';
  }

  getLocale(): string {
  return localStorage.getItem('lang') || 'en';
}
}