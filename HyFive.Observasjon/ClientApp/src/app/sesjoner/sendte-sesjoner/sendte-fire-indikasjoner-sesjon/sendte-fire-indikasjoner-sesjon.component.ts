import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faTrashAlt, faLongArrowAltLeft } from '@fortawesome/free-solid-svg-icons';
import { Dialogtekster } from '../../../konstanter/dialogtekster';
import { Queryparameters } from '../../../konstanter/queryparameters';
import { Urls } from '../../../konstanter/urls';
import { FireIndikasjonerObservasjon } from '../../../models/api/FireIndikasjonerObservasjon';
import { AktivitetType } from '../../../models/api/AktivitetType';
import { AktivitetService } from '../../../services/data/aktivitet.service';
import { AktivitetTypeKonstanter } from 'src/app/models/api/AktivitetTypeKonstanter';
import { SendteSesjonerService } from '../../../services/data/sendte-sesjoner.service';
import {FireIndikasjonerSesjon} from '../../../models/api/FireIndikasjonerSesjon';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-sendte-fire-indikasjoner',
  templateUrl: './sendte-fire-indikasjoner-sesjon.component.html',
})
export class SendteFireIndikasjonerSesjonComponent implements OnInit, OnDestroy {

  sesjon: FireIndikasjonerSesjon;
  sesjonErSendtTilServer = false;
  aktivitetTyper: AktivitetType[];
  erOnline: boolean = true;
  lasterNedSomExcel = false;

  faArrowLeft = faLongArrowAltLeft;
  faTrashAlt = faTrashAlt;
  Dialogtekster = Dialogtekster;
  Urls = Urls;

  constructor(
    private sesjonService: SendteSesjonerService,
    private router: Router,
    private route: ActivatedRoute,
    private aktivitetService: AktivitetService,
    private toastrService: ToastrService) {

  }

  ngOnInit(): void {
    this.route
      .queryParams
      .subscribe(params => {
        const sesjonId = params[Queryparameters.SesjonId] || 0;
        if(sesjonId === 0) this.router.navigate(['']);
        this.sesjonService.hentFireIndikasjonerSesjon(sesjonId).subscribe(
          (sesjon) => {
            this.sesjon = sesjon;
            if (!this.sesjon) this.router.navigate(['']);
          }
        );
      });

      this.aktivitetService.getAktivitetTyper().subscribe((aktivitetTyper) => {
        this.aktivitetTyper = aktivitetTyper;
      });
  }
  
  ngOnDestroy(): void {
    this.toastrService.clear();
  }

  navigerTilSendteSesjoner(){
    this.router.navigate([Urls.SendteSesjonerUrl])
  }

  beregnAnledningerEtterlevd(sesjon: FireIndikasjonerSesjon) : number{
    if(sesjon?.observasjoner?.length == 0)
      return 0;
    return sesjon?.observasjoner?.filter(f => f.aktivitet.aktivitetType?.kode != AktivitetTypeKonstanter.IkkeUtfort).length
  }

  beregnAnledningerEtterlevdProsent(sesjon: FireIndikasjonerSesjon): number {
    if (sesjon?.observasjoner?.length == 0)
      return 0;
    return (this.beregnAnledningerEtterlevd(sesjon) / sesjon?.observasjoner?.length) * 100;
  }

  beregnAnledningerUtelatt(sesjon: FireIndikasjonerSesjon) : number{
    if(sesjon?.observasjoner?.length == 0)
      return 0;
    return sesjon?.observasjoner?.filter(f => f.aktivitet.aktivitetType?.kode == AktivitetTypeKonstanter.IkkeUtfort).length
  }

  beregnAnledningerUtelattProsent(sesjon: FireIndikasjonerSesjon) : number{
    if(sesjon?.observasjoner?.length == 0)
      return 0;
    return (this.beregnAnledningerUtelatt(sesjon) / sesjon?.observasjoner?.length)*100
  }

  hentIngress(observasjon: FireIndikasjonerObservasjon) {
    return this.aktivitetTyper?.find(x => x.kode === observasjon.aktivitet.aktivitetType?.kode)?.navn + ' - ' + observasjon.indikasjonstyper.map(i => i.navn).join(', ');
  }

  mottattInternetStatus(harInternett: boolean){
    this.erOnline = harInternett;
    if(this.erOnline) {

    }
  }

  lastNedSomExcel() {
    this.lasterNedSomExcel = true;
    this.sesjonService.lastNedFireIndikasjonerSesjonSomExcel(this.sesjon.institusjonId, this.sesjon.id).subscribe(
      () => {},
      error => this.toastrService.error(error?.message ? error.message : error, Dialogtekster.FeilUnderNedlastingSesjonExcel, {disableTimeOut: true}),
      () => this.lasterNedSomExcel = false)
  }
}
