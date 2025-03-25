import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { InstitusjonOversiktRapport } from '../../models/api/InstitusjonOversiktRapport';
import { SesjonOversiktRapport } from '../../models/api/SesjonOversiktRapport';
import { SesjonType } from '../../models/api/SesjonType';
import { FireIndikasjonerObservasjon } from '../../models/api/FireIndikasjonerObservasjon';
import { Bruker } from '../../models/api/Bruker';
import {HandsmykkeObservasjon} from "../../models/api/HandsmykkeObservasjon";
import {HanskeObservasjon} from "../../models/api/HanskeObservasjon";
import {BeskyttelsesutstyrObservasjon} from "../../models/api/BeskyttelsesutstyrObservasjon";
import { AuthorizedRole } from '../../_felles/authorization/authorized-role';

@Injectable({
  providedIn: 'root'
})
export class ObservasjonService {

  constructor(private readonly http: HttpClient) { }

  hentInstitusjonerMedSesjoner(institusjonId: string, sesjontype: SesjonType, fraDato: Date, tilDato: Date, valgtRolle: AuthorizedRole): Observable<InstitusjonOversiktRapport[]> {
    let url = `${environment.apiBaseUrl}/v1/observasjon/institusjonerMedSesjoner`;

    let params = new HttpParams();
    if (sesjontype && sesjontype.toString() !== 'null')
      params = params.append("sesjontype", sesjontype?.toString());

    if (fraDato !== null)
      params = params.append("fradato", fraDato?.toString());

    if (tilDato !== null)
      params = params.append("tildato", tilDato?.toString());

    if (institusjonId !== null)
      params = params.append("institusjonid", institusjonId);

    params = params.append("rolle", valgtRolle.toString());

    return this.http.get<InstitusjonOversiktRapport[]>(url, { params: params });
  }

  hentSesjonerForAvdeling(avdelingsid: number, sesjontype: SesjonType, fraDato: Date, tilDato: Date, valgtRolle: AuthorizedRole): Observable<SesjonOversiktRapport[]> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/avdeling`;
    let params = new HttpParams();

    if (avdelingsid !== null)
      params = params.append("avdelingsid", avdelingsid?.toString());

    if (sesjontype && sesjontype.toString() !== 'null')
      params = params.append("sesjontype", sesjontype?.toString());

    if (fraDato !== null)
      params = params.append("fradato", fraDato?.toString());

    if (tilDato !== null)
      params = params.append("tildato", tilDato?.toString());

    params = params.append("rolle", valgtRolle.toString());

    return this.http.get<SesjonOversiktRapport[]>(url, { params: params });
  }

  hentSesjonerForInstitusjon(institusjonId: number, observator?: Bruker, sesjontype?: SesjonType, fraDato?: Date, tilDato?: Date): Observable<SesjonOversiktRapport[]> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/institusjon`;
    let params = new HttpParams();

    params = params.append("institusjonid", institusjonId?.toString());

    if (observator)
      params = params.append("observatorid", observator?.toString());

    if (sesjontype && sesjontype.toString() !== 'null' )
      params = params.append("sesjontype", sesjontype?.toString());

    if (fraDato)
      params = params.append("fradato", fraDato?.toString());

    if (tilDato)
      params = params.append("tildato", tilDato?.toString());

    return this.http.get<SesjonOversiktRapport[]>(url, { params: params });
  }

  overforSesjonTilFHI(institusjonId: number, sesjonId: any) {
    const url = `${environment.apiBaseUrl}/v1/observasjon/overfor`;
    let params = new HttpParams();

    params = params.append("institusjonid", institusjonId?.toString());
    params = params.append("sesjonId", sesjonId.toString());

    return this.http.get<SesjonOversiktRapport>(url, { params: params });
  }

  oppdaterFireIndikasjonerObservasjon(observasjon: FireIndikasjonerObservasjon) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observasjon/fireindikasjoner/oppdater`;
    return this.http.put<boolean>(url, observasjon);
  }

  slettFireIndikasjonerObservasjon(observasjonId: string, sesjonId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/fireindikasjoner/slett?observajonId=${observasjonId}&sesjonId=${sesjonId}`;
    return this.http.delete<boolean>(url);
  }

  oppdaterHandsmykkeObservasjon(observasjon: HandsmykkeObservasjon) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observasjon/handsmykke/oppdater`;
    return this.http.put<boolean>(url, observasjon);
  }

  slettHandsmykkeObservasjon(observasjonId: string, sesjonId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/handsmykke/slett?observajonId=${observasjonId}&sesjonId=${sesjonId}`;
    return this.http.delete<boolean>(url);
  }

  oppdaterHanskeObservasjon(observasjon: HanskeObservasjon) : Observable<boolean>{
    const url = `${environment.apiBaseUrl}/v1/observasjon/hanske/oppdater`;
    return this.http.put<boolean>(url, observasjon);
  }

  slettHanskeObservasjon(observasjonId: string, sesjonId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/hanske/slett?observajonId=${observasjonId}&sesjonId=${sesjonId}`;
    return this.http.delete<boolean>(url);
  }

  oppdaterBeskyttelsesutstyrObservasjon(observasjon: BeskyttelsesutstyrObservasjon) : Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/beskyttelsesutstyr/oppdater`;
    return this.http.put<boolean>(url, observasjon);
  }

  slettBeskyttelsesutstyrObservasjon(observasjonId: string, sesjonId: string): Observable<boolean> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/beskyttelsesutstyr/slett?observajonId=${observasjonId}&sesjonId=${sesjonId}`;
    return this.http.delete<boolean>(url);
  }

  hentBeskyttelsesutstyrObservasjon(observasjonId: string, sesjonId: string) : Observable<BeskyttelsesutstyrObservasjon> {
    const url = `${environment.apiBaseUrl}/v1/observasjon/beskyttelsesutstyr?observasjonId=${observasjonId}&sesjonId=${sesjonId}`;
    return this.http.get<BeskyttelsesutstyrObservasjon>(url);
  }
}
