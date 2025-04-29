import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../_felles/services/authorization.service';
import { LoggedinUser } from '../models/api/LoggedinUser';
import { ToastrService } from 'ngx-toastr';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { ClipboardService } from 'ngx-clipboard';

@Component({
  selector: 'app-front-page-for-administration',
  templateUrl: './front-page-for-administration.component.html'
})
export class HomePageForAdministrationComponent implements OnInit, OnDestroy {
  loading = true;
  user: LoggedinUser = null;
  faCopy = faCopy;

  constructor(
    public authorizationService: AuthorizationService,
    private toastrService: ToastrService,
    private clipboardService: ClipboardService) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    },
      (error) => (this.toastrService.error("An error occurred while loading user: " + error?.message ? error.message : error, '', {disableTimeOut: true})),
      () => this.loading = false
    );
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  copyPseudonymClick() {
    this.clipboardService.copy(this.user?.identityPseudonym);
    this.toastrService.success('Pseudonym copied to the clipboard and can be pasted elsewhere using Paste (CTRL+V)');
  }
}
