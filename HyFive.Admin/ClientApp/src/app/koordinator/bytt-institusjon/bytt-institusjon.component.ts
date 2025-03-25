import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../../_felles/services/authorization.service';
import { InnloggetBruker } from '../../models/api/InnloggetBruker';
import { faUser } from '@fortawesome/free-solid-svg-icons';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';
import { RolleEventService } from '../../services/events/rolle-event.service';
import { AuthorizedRole } from '../../_felles/authorization/authorized-role';
import { InstitusjonForKoordinatorEventService } from '../../services/events/institusjon-for-koordinator-event.service';
@Component({
  selector: 'app-bytt-institusjon',
  templateUrl: './bytt-institusjon.component.html'
})
export class ByttInstitusjonComponent implements OnInit {

  bruker: InnloggetBruker = null;
  roller: string;
  faUser = faUser;
  valgtInstitusjon: InstitusjonRapport = null;
  valgtInstitusjonTemp: InstitusjonRapport = null;
  visVelgInstitusjon = false;
  visByttInstitusjonBoks = false;
  
  visteInstitusjoner: InstitusjonRapport[] = [];
  institusjoner: InstitusjonRapport[] = [];

  constructor(
    private authorizationService: AuthorizationService,
    private institusjonService: InstitusjonService,
    private rolleEventService: RolleEventService,
    private institusjonForKoordinatorEventService: InstitusjonForKoordinatorEventService
  ) { }

  ngOnInit(): void {
    this.authorizationService.getBruker().subscribe((bruker: InnloggetBruker) => {
      this.bruker = bruker;
      let valgtRolle = this.authorizationService.hentValgtRolle();
      this.initialiser(valgtRolle);
    });

    this.rolleEventService.byttRolleEvent.subscribe(
      (valgtRolle) => {
        this.initialiser(valgtRolle);
      });

    this.institusjonForKoordinatorEventService.oppdaterInstitusjonsListe.subscribe(
      () => {
        let valgtRolle = this.authorizationService.hentValgtRolle();
        this.initialiser(valgtRolle);
      });
  }

  private initialiser(valgtRolle: AuthorizedRole) {
    if (valgtRolle === AuthorizedRole.Administrator) {
      this.visByttInstitusjonBoks = false;
    } else if (valgtRolle === AuthorizedRole.Koordinator) {
      this.institusjonService.hentInstitusjonerForKoordinator().subscribe((institusjoner) => {
        if (institusjoner.length > 0) {
          this.visByttInstitusjonBoks = true;
        }

        this.institusjoner = institusjoner;
        let valgtInstitusjonId = this.institusjonService.hentValgtInstitusjonId()
        this.valgtInstitusjon = this.institusjoner.find(x => x.id === valgtInstitusjonId);

        if (!this.valgtInstitusjon) {
          this.valgtInstitusjon = this.institusjoner[0];
          this.institusjonService.oppdaterValgtInstitusjonId(this.valgtInstitusjon.id);
        }

        this.visteInstitusjoner = this.institusjoner.filter(x => x.id !== this.valgtInstitusjon.id);
      });
    }
  }

  byttInstitusjon(institusjon: InstitusjonRapport) {  
    this.valgtInstitusjon = institusjon;
    this.institusjonService.oppdaterValgtInstitusjonId(this.valgtInstitusjon.id);
    window.location.reload();
  }
}
