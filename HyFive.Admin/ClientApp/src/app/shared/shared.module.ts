import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ClipboardModule } from 'ngx-clipboard';

import { SafePipe } from './pipes/safe.pipe';

import { SpinnerComponent } from './spinner/spinner.component';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AccordionComponent } from './accordion/accordion.component';
import { NavigasjonslenkeComponent } from './navigasjonslenke/navigasjonslenke.component';
import {ToastrConfig} from '../_felles/konstanter/toastr-config';

@NgModule({
  declarations: [
    SafePipe,
    SpinnerComponent,
    AccordionComponent,
    NavigasjonslenkeComponent,
  ],
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    NgbModule,
    ClipboardModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(ToastrConfig.toastrConfig),
  ],
    exports: [
        SafePipe,
        SpinnerComponent,
        AccordionComponent,
        NavigasjonslenkeComponent,
        CommonModule,
        RouterModule,
        FormsModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        NgbModule,
        ClipboardModule,
  ],
})
export class SharedModule { }
