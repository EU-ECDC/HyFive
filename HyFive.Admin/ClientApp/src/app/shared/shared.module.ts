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
import { NavigationLinkComponent } from './navigation-link/navigation-bar.component';
import {ToastrConfig} from '../_common/constants/toastr-config';
import { NgSelectModule } from '@ng-select/ng-select';


@NgModule({
  declarations: [
    SafePipe,
    SpinnerComponent,
    AccordionComponent,
    NavigationLinkComponent,
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
    NgSelectModule
  ],
    exports: [
        SafePipe,
        SpinnerComponent,
        AccordionComponent,
        NavigationLinkComponent,
        CommonModule,
        RouterModule,
        FormsModule,
        ReactiveFormsModule,
        FontAwesomeModule,
        NgbModule,
        ClipboardModule,
        NgSelectModule
  ],
})
export class SharedModule { }
