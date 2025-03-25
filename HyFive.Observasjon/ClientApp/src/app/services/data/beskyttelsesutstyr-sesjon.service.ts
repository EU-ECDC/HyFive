import {EventEmitter, Injectable} from "@angular/core";
import { BaseSesjonService } from './base-sesjon.service';
import { BeskyttelsesutstyrSesjonsvisning } from '../../models/registrering/beskyttelsesutstyr-sesjonsvisning.model';
import { BeskyttelsesutstyrSesjon } from "src/app/models/api/BeskyttelsesutstyrSesjon";
import { BeskyttelsesutstyrObservasjon } from "src/app/models/api/BeskyttelsesutstyrObservasjon";
import { Rolle } from "src/app/models/api/Rolle";
import { Avdeling } from "src/app/models/api/Avdeling";
import { Uuid } from "src/app/utils/uuid";
import { BeskyttelsesutstyrsettingType } from '../../models/api/BeskyttelsesutstyrsettingType';
import { Kort } from '../../models/registrering/kort.model';
import { BeskyttelsesutstyrKort } from "src/app/models/registrering/beskyttelsesutstyr-kort.model";
import { Localstoragepaths } from '../../konstanter/localstoragepaths';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { InstitusjonService } from "./institusjon.service";
import {BeskyttelsesutstyrType} from "../../models/api/BeskyttelsesutstyrType";
import {Beskyttelsesutstyr} from "../../models/api/Beskyttelsesutstyr";

@Injectable({
  providedIn: 'root'
})
export class BeskyttelsesutstyrSesjonService extends BaseSesjonService<BeskyttelsesutstyrSesjonsvisning, BeskyttelsesutstyrSesjon, BeskyttelsesutstyrObservasjon>{
  sesjonsvisningLocalStoragePath: string = Localstoragepaths.BeskyttelsesutstyrSesjonsvisninger;
  sesjonLocalStoragePath: string = Localstoragepaths.BeskyttelsesutstyrSesjoner;



  constructor(
    public institusjonService: InstitusjonService,
    private httpClient: HttpClient) {
    super(institusjonService);
  }

  public sendTilServer(sesjonId: string): Observable<string> {
    var sesjoner = this.hentSesjoner();
    var sesjonIndeks = sesjoner.map(s => s.id).indexOf(sesjonId);
    var sesjonSomSkalSendes = sesjoner[sesjonIndeks];
    return this.httpClient.post<string>(`${environment.apiBaseUrl}/v1/beskyttelsesutstyr`, sesjonSomSkalSendes)
  }

  beskyttelsesutstyrOppdatert: EventEmitter<BeskyttelsesutstyrType[]> = new EventEmitter<BeskyttelsesutstyrType[]>()

  lagSesjonsvisning(
    rollerSomObserveres: Rolle[],
    avdeling: Avdeling,
    setting: BeskyttelsesutstyrsettingType): string {
    let id = Uuid.generateUUID();

    let sesjonsvisning: BeskyttelsesutstyrSesjonsvisning = {
      sesjonId: id,
      avdeling: avdeling,
      kort: this.genererKort(rollerSomObserveres, setting),
      setting: setting
    };

    var sesjonsvisninger = this.hentSesjonsvisninger();
    sesjonsvisninger.push(sesjonsvisning);
    this.lagreSesjonsvisninger(sesjonsvisninger);

    return id;
  }

  oppdaterSesjonUtstyrstyper(sesjonId: string, utstyrstyper: BeskyttelsesutstyrType[]){
    var sesjonsvisninger = this.hentSesjonsvisninger();
    var sesjonsvisningSomSkalOppdateres = sesjonsvisninger.find(s => s.sesjonId == sesjonId);
    sesjonsvisningSomSkalOppdateres.setting.utstyrstyper = utstyrstyper;
    this.lagreSesjonsvisninger(sesjonsvisninger);
    this.beskyttelsesutstyrOppdatert.emit(utstyrstyper);
  }

  antallKvalifisertUtstyr(beskyttelsesutstyrListe: Beskyttelsesutstyr[]) : number{
    return beskyttelsesutstyrListe.reduce((kvalifiserteUtstyr, curr) => {
        if (curr.erIndikert || curr.bleBenyttet){
          return kvalifiserteUtstyr + 1;
        }
        else {
          return kvalifiserteUtstyr;
        }
      },
      0);
  }

  private genererKort(roller: Rolle[], setting: BeskyttelsesutstyrsettingType): BeskyttelsesutstyrKort[] {
    return roller.map((r, i) => {
      return { id: Uuid.generateUUID(), rolle: r, utstyr: setting.utstyrstyper, erAktivt: i == 0 } as BeskyttelsesutstyrKort
    });
  }
}
