import { Component, OnInit } from '@angular/core';
import { InstitusjonService } from '../../services/data/institusjon.service';
import { QueryParameters } from '../../_felles/konstanter/queryparameters';
import { InstitusjonRapport } from '../../models/api/InstitusjonRapport';

@Component({
  selector: 'app-redigering-av-observatorer',
  templateUrl: './redigering-av-observatorer.component.html'
})
export class RedigeringAvObservatorerComponent implements OnInit {

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
        region: result.region
      } as InstitusjonRapport;
    });
  }

}
