import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../_felles/services/authorization.service';
import { LoggedinUser } from '../models/api/LoggedinUser';
import { ToastrService } from 'ngx-toastr';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { ClipboardService } from 'ngx-clipboard';

@Component({
  selector: 'app-forside-for-administrasjon',
  templateUrl: './forside-for-administrasjon.component.html'
})
export class ForsideForAdministrasjonComponent implements OnInit, OnDestroy {
  laster = true;
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
      (error) => (this.toastrService.error("En feil skjedde under innlasting av user: " + error?.message ? error.message : error, '', {disableTimeOut: true})),
      () => this.laster = false
    );
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  kopierPseudonymKlikk() {
    this.clipboardService.copy(this.user?.identityPseudonym);
    this.toastrService.success('Pseudonym kopiert til utklippstavle og kan limes inn andre steder ved bruk av Lim inn (CTRL+V)');
  }
}
