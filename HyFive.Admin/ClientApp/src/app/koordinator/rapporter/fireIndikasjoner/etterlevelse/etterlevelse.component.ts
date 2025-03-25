import { Component, OnInit, OnDestroy } from '@angular/core';
import { FhiDiagramOptions } from '@folkehelseinstituttet/angular-highcharts';
import { ToastrService } from 'ngx-toastr';
import { Avdeling } from '../../../../models/api/Avdeling';
import { Rolle } from '../../../../models/api/Rolle';
import { InstitusjonService } from '../../../../services/data/institusjon.service';
import { RapportService } from '../../../../services/data/rapport.service';
import { RolleService } from '../../../../services/data/rolle.service';

@Component({
  selector: 'app-etterlevelse',
  templateUrl: './etterlevelse.component.html'
})
export class EtterlevelseComponent implements OnInit, OnDestroy {

  fraAr: number = 2024;
  tilAr: number = 2024;
  fraManed: number = 1;
  tilManed: number = 1;
  rolle: Rolle = null;
  avdeling: Avdeling = null;
  intervall: string = 'maned';
  maneder: any [];

  visGraf = false;
  roller: Rolle[];
  avdelinger: Avdeling[];

  prosentDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };

  antallDiagramOptions: FhiDiagramOptions = {
    title: 'Diagram title',
    series: [],
    diagramTypeId: 'line'
  };
  constructor(
    private grafService: RapportService,
    private institusjonService: InstitusjonService,
    private rolleService: RolleService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {

    this.maneder = this.initManeder();
    this.lastRoller();
    this.lastAvdelinger();
  }

  ngOnDestroy(): void {
  }

  lastRoller() {
    this.rolleService.hentRoller().subscribe(
      (roller) => this.roller = roller,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av roller: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  lastAvdelinger() {
    var institusjonId = this.institusjonService.hentValgtInstitusjonId();
    this.institusjonService.hentAvdelinger(institusjonId).subscribe(
      (avdelinger) => this.avdelinger = avdelinger,
      (error) => this.toastrService.error('Det oppstod en feil under lasting av roller: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  hentEtterlevelseForFireindikasjoner() {
    var institusjonId = this.institusjonService.hentValgtInstitusjonId();
    this.grafService.hentEtterlevelseForFireindikasjoner(institusjonId, this.intervall, this.fraManed, this.fraAr, this.tilManed, this.tilAr, this.rolle?.id, this.avdeling?.id).subscribe(
      (grafer) => {

        let prosentGraf = grafer[0];
        this.lagProsentDiagramOptions(prosentGraf);
        let antallGraf = grafer[1];
        this.lagAntallDiagramOptions(antallGraf);
        this.visGraf = true;
      },
      (error) => this.toastrService.error('Feil i generering av grafdata: ' + error?.message, '', { disableTimeOut: true })
    );
  }

  lagProsentDiagramOptions(graf: any) {
    this.prosentDiagramOptions = {
      title: graf.tittel,
      diagramTypeId: 'line',
      series: graf.grafDataListe,
      openSource: false,
      units: [{
        id: 'prosent',
        decimals: 1,
        label: 'Etterlevelse (%)',
        symbol: '%',
        position: 'end'
      }]
    };
  }

  lagAntallDiagramOptions(graf: any) {
    this.antallDiagramOptions = {
      title: graf.tittel,
      diagramTypeId: 'line',
      series: graf.grafDataListe,
      openSource: false,
      units: [{
        id: 'antall',
        label: 'Antall',
        position: 'end'
      }]
    };
  }

  initManeder() {
    return [
      { verdi: 1, beskrivelse: "Januar" },
      { verdi: 2, beskrivelse: "Februar" },
      { verdi: 3, beskrivelse: "Mars" },
      { verdi: 4, beskrivelse: "April" },
      { verdi: 5, beskrivelse: "Mai" },
      { verdi: 6, beskrivelse: "Juni" },
      { verdi: 7, beskrivelse: "Juli" },
      { verdi: 8, beskrivelse: "August" },
      { verdi: 9, beskrivelse: "September" },
      { verdi: 10, beskrivelse: "Oktober" },
      { verdi: 11, beskrivelse: "November" },
      { verdi: 12, beskrivelse: "Desember" }];
  }
}
