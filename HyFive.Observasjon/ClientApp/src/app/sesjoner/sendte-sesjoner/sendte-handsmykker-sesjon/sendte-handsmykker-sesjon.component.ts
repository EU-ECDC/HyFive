import { Component, OnInit, OnDestroy } from '@angular/core';
import { SendteSesjonerService } from '../../../services/data/sendte-sesjoner.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Queryparameters } from '../../../konstanter/queryparameters';
import { Urls } from '../../../konstanter/urls';
import { HandsmykkeMapper } from 'src/app/utils/handsmykke-mapper';
import { HandsmykkeType } from 'src/app/models/api/HandsmykkeType';
import { HandsmykkeSesjon } from '../../../models/api/HandsmykkeSesjon';
import { HandsmykkeTypeService } from '../../../services/data/handsmykketype.service';
import { faFileExcel } from '@fortawesome/free-regular-svg-icons';
import {ToastrService} from 'ngx-toastr';
import {Dialogtekster} from '../../../konstanter/dialogtekster';

@Component({
  selector: 'app-sendte-handsmykker-sesjon',
  templateUrl: './sendte-handsmykker-sesjon.component.html'
})
export class SendteHandsmykkerSesjonComponent implements OnInit, OnDestroy {

  sesjon: HandsmykkeSesjon;
  handsmykkeTyper: HandsmykkeType[] = [];
  erOnline: boolean = true;

  faFileExcel = faFileExcel;
  lasterNedSomExcel = false;
  Dialogtekster = Dialogtekster;

  constructor(private router: Router,
    private route: ActivatedRoute,
    private sesjonService: SendteSesjonerService,
    private handsmykkeTypeService: HandsmykkeTypeService,
    private toastrService: ToastrService) { }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const sesjonId = params[Queryparameters.SesjonId] || 0;
      if (sesjonId === 0) this.router.navigate(['']);
      this.sesjonService.hentHandsmykkerSesjon(sesjonId).subscribe(
        (sesjon) => {
          this.sesjon = sesjon;
          if (!sesjon) this.router.navigate(['']);
        }
      );
    });
    this.handsmykkeTypeService.getHandsmykkeTyper().subscribe((handsmykkeTyper) => {
      this.handsmykkeTyper = handsmykkeTyper;
    });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  visHandsmykker(handsmykker: HandsmykkeType[]): string {
    return HandsmykkeMapper.getHandsmykkevalg(this.handsmykkeTyper, handsmykker.map(x => x.kode)).filter(h => h.erValgt == true).map(h => h.navn).join(', ');
  }

  navigerTilSendteSesjoner() {
    this.router.navigate([Urls.SendteSesjonerUrl]);
  }

  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.lastNedHandsmykkeSesjonSomExcel(this.sesjon.institusjonId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, Dialogtekster.FeilUnderNedlastingSesjonExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
