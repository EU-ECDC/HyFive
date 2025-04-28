import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeNb from '@angular/common/locales/nb';
import localeNbExtra from '@angular/common/locales/extra/nb';
registerLocaleData(localeNb, 'nb', localeNbExtra);

import { NgbDatepickerI18n, NgbDateParserFormatter, NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { DatepickerI18nService } from 'src/app/_felles/services/datepicker-i18n.service';
import { DatepickerParserFormatterService } from 'src/app/_felles/services/datepicker-parser-formatter.service';
import { DatepickerDateAdapterService } from 'src/app/_felles/services/datepicker-date-adapter.service';

import 'what-input';

import { SharedModule } from '../shared/shared.module';

import { MainMenuComponent } from '../core/main-menu/main-menu.component';
import { ChangeInstitutionComponent } from '../koordinator/change-institution/change-institution.component';

@NgModule({
  declarations: [
    MainMenuComponent,
    ChangeInstitutionComponent
  ],
  imports: [
    SharedModule,
    HttpClientModule
  ],
  exports: [
    MainMenuComponent,
    ChangeInstitutionComponent
  ],
  providers: [
    { provide: LOCALE_ID, useValue: 'nb' },
    { provide: NgbDatepickerI18n, useClass: DatepickerI18nService },
    { provide: NgbDateParserFormatter, useClass: DatepickerParserFormatterService },
    { provide: NgbDateAdapter, useClass: DatepickerDateAdapterService }
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in AppModule only!');
    }
  }
}
