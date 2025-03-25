import { Component, OnInit, OnDestroy } from '@angular/core';
import { SendteSesjonerService } from '../../../services/data/sendte-sesjoner.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../../konstanter/queryparameters';
import { Urls } from '../../../konstanter/urls';
import { HandsmykkeType } from 'src/app/models/api/HandsmykkeType';
import { HanskeSesjon } from '../../../models/api/HanskeSesjon';
import { HanskeObservasjon } from '../../../models/api/HanskeObservasjon';
import {ToastrService} from 'ngx-toastr';
import { faFileExcel } from '@fortawesome/free-regular-svg-icons';
import {Dialogtekster} from '../../../konstanter/dialogtekster';

@Component({
  selector: 'app-sendte-hanske-sesjon',
  templateUrl: './sendte-hanske-sesjon.component.html'
})
export class SendteHanskeSesjonComponent implements OnInit, OnDestroy {

  sesjon: HanskeSesjon;
  handsmykkeTyper: HandsmykkeType[] = [];
  erOnline: boolean = true;
  faFileExcel = faFileExcel
  Dialogtekster = Dialogtekster;

  private lasterNedSomExcel: boolean;

  constructor(private router: Router,
              private route: ActivatedRoute,
              private sesjonService: SendteSesjonerService,
              private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const sesjonId = params[Queryparameters.SesjonId] || 0;
      if (sesjonId === 0) this.router.navigate(['']);
      this.sesjonService.hentHanskeSesjon(sesjonId).subscribe(
        (sesjon) => {
          this.sesjon = sesjon;
          if (!sesjon) this.router.navigate(['']);
        }
      )
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  visIndikasjoner(item: HanskeObservasjon): string { // TODO Velge mellom visning av indikasjoner/typer, eller vise om observasjon var med eller uten indikasjoner
    if (item.hanskeMedIndikasjonTyper.length) return item.hanskeMedIndikasjonTyper.map(x => x.navn).join(', ');;
    return item.hanskeUtenIndikasjonTyper.map(x => x.navn).join(', ');
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }
  
  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.lastNedHanskeSesjonSomExcel(this.sesjon.institusjonId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, Dialogtekster.FeilUnderNedlastingSesjonExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
