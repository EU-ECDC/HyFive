import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../_common/services/authorization.service';
import { LoggedInUser } from '../models/api/LoggedInUser';
import { ToastrService } from 'ngx-toastr';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { ClipboardService } from 'ngx-clipboard';
import { AuthorizedRole } from '../_common/authorization/authorized-role';

@Component({
  selector: 'app-home-page-for-administration',
  templateUrl: './home-page-for-administration.component.html'
})
export class HomePageForAdministrationComponent implements OnInit, OnDestroy {
  loading = true;
  user: LoggedInUser = null;
  faCopy = faCopy;
  selectedRole: AuthorizedRole;

  constructor(
    public authorizationService: AuthorizationService,
    private readonly toastrService: ToastrService,
    private readonly clipboardService: ClipboardService) { }

  ngOnInit(): void {
    this.authorizationService.getUser().subscribe((user) => {
      this.user = user;
    },
      (error) => (this.toastrService.error($localize`:@@LoadingUserError:An error occurred while loading user: ` + error?.error.message ? error.error.message : error, '', {disableTimeOut: true})),
      () => this.loading = false
    );
    this.selectedRole = this.authorizationService.getSelectedRole();
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  copyPseudonymClick() {
    this.clipboardService.copy(this.user?.identityPseudonym);
    this.toastrService.success($localize`:@@PseudonymClipborad:Pseudonym copied to the clipboard and can be pasted elsewhere using Paste (CTRL+V)`);
  }

  logout() {
    localStorage.clear();
    globalThis.location.href = '/account/logout';
  }
}
