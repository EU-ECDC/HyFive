import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthorizationService } from '../_felles/services/authorization.service';
import { InnloggetBruker } from '../models/api/InnloggetBruker';
import { ToastrService } from 'ngx-toastr';
import { faCopy } from '@fortawesome/free-solid-svg-icons';
import { ClipboardService } from 'ngx-clipboard';

@Component({
  selector: 'app-forside-for-administrasjon',
  templateUrl: './forside-for-administrasjon.component.html'
})
export class ForsideForAdministrasjonComponent implements OnInit, OnDestroy {
  laster = true;
  bruker: InnloggetBruker = null;
  faCopy = faCopy;

  constructor(
    public authorizationService: AuthorizationService,
    private toastrService: ToastrService,
    private clipboardService: ClipboardService) { }

  ngOnInit(): void {
    this.authorizationService.getBruker().subscribe((bruker) => {
      this.bruker = bruker;
    },
      (error) => (this.toastrService.error("En feil skjedde under innlasting av bruker: " + error?.message ? error.message : error, '', {disableTimeOut: true})),
      () => this.laster = false
    );
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  kopierPseudonymKlikk() {
    this.clipboardService.copy(this.bruker?.identPseudonym);
    this.toastrService.success('Pseudonym kopiert til utklippstavle og kan limes inn andre steder ved bruk av Lim inn (CTRL+V)');
  }
}
