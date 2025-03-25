import { Component, OnInit } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';

@Component({
  selector: 'app-redigering-av-koordinatorer',
  templateUrl: './redigering-av-koordinatorer.component.html'
})
export class RedigeringAvKoordinatorerComponent implements OnInit {

  institusjonRapport: InstitusjonRapport = null;
  constructor(private institusjonService: InstitusjonService) { }

  ngOnInit(): void {
    let valgtInstitusjonsId = this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentInstitusjon(valgtInstitusjonsId).subscribe((result) => {
      this.institusjonRapport = {
        id: result.id,
        herId: result.herId,
        forkortelse: result.forkortelse,
        institusjontype: result.institusjontype,
        navn: result.navn,
        region: result.region,
        helseforetak: result.helseforetak
      } as InstitusjonRapport;
    });
  }
}
