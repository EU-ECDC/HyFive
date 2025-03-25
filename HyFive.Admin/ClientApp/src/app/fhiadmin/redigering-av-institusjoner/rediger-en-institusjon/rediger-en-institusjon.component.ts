import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { InstitusjonService } from '../../../services/data/institusjon.service';
import { Institusjon } from '../../../models/api/Institusjon';
import { InstitusjonType } from '../../../models/api/InstitusjonType';
import { ToastrService } from 'ngx-toastr';
import { UrlPaths } from '../../../_felles/konstanter/url-paths';
import { Helseforetak } from 'src/app/models/api/Helseforetak';
import { HelseforetakService } from 'src/app/services/data/helseforetak.service';
import { InstitusjonstypeKonstanter } from 'src/app/models/api/InstitusjonstypeKonstanter';
import { KommuneService } from 'src/app/services/data/kommune.service';
import { Kommune } from 'src/app/models/api/Kommune';

@Component({
  selector: 'app-rediger-en-institusjon',
  templateUrl: './rediger-en-institusjon.component.html'
})
export class RedigerEnInstitusjonComponent implements OnInit {

  constructor(private institusjonService: InstitusjonService,
              private toastrService: ToastrService,
              private kommuneService: KommuneService,
              private helseforetakService: HelseforetakService) { }

  institusjon: Institusjon = null;
  institusjontyper: InstitusjonType[] = [];
  institusjontypeId = 0;
  listAvHelseforetak: Helseforetak[] = [];

  kommune: Kommune = null;
  kommuner: Kommune[];
  kommuneId = 0;
  UrlPaths = UrlPaths;
  helseforetakId = 0;
  visKommune = false;
  visHelseforetak = false;


  @Input() institusjonId: number;
  @Output() institusjonSlettetEvent: EventEmitter<number> = new EventEmitter<number>();
  @Output() institusjonOppdatertEvent: EventEmitter<Institusjon> = new EventEmitter<Institusjon>();

  ngOnInit(): void {
    if (this.institusjonId === 0) {
      this.institusjon = null;
      return;
    }
    this.institusjonService.hentInstitusjon(this.institusjonId).subscribe((institusjon) => {
      this.institusjon = institusjon;
      this.institusjontypeId = institusjon.institusjontype.id;
      this.kommuneId = institusjon.kommune?.id;
      this.helseforetakId = institusjon.helseforetak?.id;
      
      this.institusjonService.hentInstitusjontyper().subscribe((typer) => {
        this.institusjontyper = typer;
        this.visKommune = this.institusjontyper.length > 0 && this.institusjon.institusjontype.kode == InstitusjonstypeKonstanter.Sykehjem;
        this.visHelseforetak = this.institusjontyper.length > 0 && this.institusjon.institusjontype.kode == InstitusjonstypeKonstanter.Sykehus;
      });
    });

    this.kommuneService.hentKommuner().subscribe(
      (kommuner) => {
        this.kommuner = kommuner;
    });

    this.helseforetakService.hentAlleHelseforetak().subscribe(
      (alleHelseforetak) => {
        this.listAvHelseforetak = alleHelseforetak;
    });
  }

  slettInstitusjon() {
      if (this.institusjonId > 0) {
        this.institusjonService.slettInstitusjon(this.institusjonId).subscribe(() => {
          this.institusjon = null;
          this.institusjonSlettetEvent.emit(this.institusjonId);
        },
          (error =>
            this.toastrService.error(`En feil oppstod: ${error?.error} / ${error?.message}`, 'Feil under sletting av institusjon', { disableTimeOut: true})));
      }
  }

  institusjonTypeEndret() {
    this.institusjon.institusjontype = this.institusjontyper.find(i => i.id === this.institusjontypeId);
    this.visKommune = this.institusjontyper.length > 0 && this.institusjon.institusjontype.kode == InstitusjonstypeKonstanter.Sykehjem;
     this.visHelseforetak = this.institusjontyper.length > 0 && this.institusjon.institusjontype.kode == InstitusjonstypeKonstanter.Sykehus;
  }

  kommuneEndret() {
    if (this.kommuneId) {
      this.institusjon.kommune = this.kommuner.find(r => r.id === this.kommuneId);
    }
  }

  lagreInstitusjon() {
    this.institusjonService.oppdaterInstitusjon(this.institusjon).subscribe(
      (institusjon) => {
        this.toastrService.success('Institusjonen ble oppdatert');
        this.institusjonOppdatertEvent.emit(institusjon);
      },
      (error) => this.toastrService.error(`En feil oppstod: ${error?.error} / ${error?.message}`, 'Feil under oppdatering', { disableTimeOut: true}));
  }

  helseforetakEndret() {
    if (this.helseforetakId) {
      this.institusjon.helseforetak = this.listAvHelseforetak.find(r => r.id === this.helseforetakId);
    }
  }

  kanIkkeLagreInstitusjon(): boolean {
    if (this.institusjon.institusjontype.id > 0 && this.institusjon.navn?.length > 0)
      return false;
    else
      return true;
  }
}

