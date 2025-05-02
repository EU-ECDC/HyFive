import { Component, OnInit, OnDestroy } from '@angular/core';
import { SendteSesjonerService } from '../../../services/data/sendte-sessions.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../../konstanter/queryparameters';
import { Urls } from '../../../konstanter/urls';
import { HandJewelryType } from 'src/app/models/api/HandJewelryType';
import { GloveSession } from '../../../models/api/GloveSession';
import { HanskeObservasjon } from '../../../models/api/HanskeObservasjon';
import {ToastrService} from 'ngx-toastr';
import { faFileExcel } from '@fortawesome/free-regular-svg-icons';
import {Dialogtekster} from '../../../konstanter/dialogtekster';

@Component({
  selector: 'app-sendte-hanske-sesjon',
  templateUrl: './sendte-hanske-sesjon.component.html'
})
export class SendteHanskeSesjonComponent implements OnInit, OnDestroy {

  sesjon: GloveSession;
  handJewelryTypes: HandJewelryType[] = [];
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
      const sessionId = params[Queryparameters.SesjonId] || 0;
      if (sessionId === 0) this.router.navigate(['']);
      this.sesjonService.hentHanskeSesjon(sessionId).subscribe(
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
    if (item.hanskeMedIndikasjonTyper.length) return item.hanskeMedIndikasjonTyper.map(x => x.name).join(', ');;
    return item.hanskeUtenIndikasjonTyper.map(x => x.name).join(', ');
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }
  
  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.lastNedHanskeSesjonSomExcel(this.sesjon.institutionId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, Dialogtekster.FeilUnderNedlastingSesjonExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
