import { Sesjon } from '../../models/api/Sesjon';
import { BaseSesjonsvisning } from '../../models/registrering/base-sesjonsvisning.model';
import { Observasjon } from '../../models/api/Observasjon';
import { InstitusjonService } from './institusjon.service';
import {AjaxResponse} from 'rxjs/ajax';
import { DatoHjelper } from 'src/app/utils/datohjelper';

export abstract class BaseSesjonService<TSesjonsvisning extends BaseSesjonsvisning, TSesjon extends Sesjon<TObservasjon>, TObservasjon extends Observasjon>  {

  abstract sesjonsvisningLocalStoragePath: string;
  abstract sesjonLocalStoragePath: string;

  constructor(
    public institusjonService: InstitusjonService) {
  }

  protected lagreSesjonsvisninger(sesjonsvisninger: TSesjonsvisning[]) {
    localStorage.setItem(this.sesjonsvisningLocalStoragePath, JSON.stringify(sesjonsvisninger));
  }

  protected lagreSesjoner(sesjoner: TSesjon[]) {
    localStorage.setItem(this.sesjonLocalStoragePath, JSON.stringify(sesjoner, DatoHjelper.dateTimeSomLocaleStringReplacer));
  }

  public slettSesjon(sesjonId: string) {
    let sesjoner = this.hentSesjoner().filter(s => s.id !== sesjonId);
    this.lagreSesjoner(sesjoner);
    let sesjonsvisninger = this.hentSesjonsvisninger().filter(s => s.sesjonId !== sesjonId);
    this.lagreSesjonsvisninger(sesjonsvisninger);
  }

  public hentSesjoner(): TSesjon[] {
    let sesjoner: TSesjon[] = [];
    const sesjonerString = localStorage.getItem(this.sesjonLocalStoragePath);
    if (sesjonerString != null) {
      sesjoner = JSON.parse(sesjonerString);
    }
    return sesjoner.sort((s1, s2) => {
      if (s1.starttidspunkt == null && s2.starttidspunkt != null)
        return 1;
      if (s2.starttidspunkt == null && s1.starttidspunkt != null)
        return -1;
      if (s2.starttidspunkt === s1.starttidspunkt)
        return 0;
      return s1.starttidspunkt < s2.starttidspunkt ? 1 : -1
    });
  }

  public oppdaterSesjonsvisningForSesjon(sesjonsvisning: TSesjonsvisning): TSesjonsvisning {
    let eksisterendeSesjonsvisning = this.hentSesjonsvisningForSesjon(sesjonsvisning.sesjonId);
    if (eksisterendeSesjonsvisning) {
      let sesjonsvisninger = this.hentSesjonsvisninger();
      var eksisterendeSesjonsvisningIndex = sesjonsvisninger.map(s => s.sesjonId).indexOf(sesjonsvisning.sesjonId);
      sesjonsvisninger[eksisterendeSesjonsvisningIndex] = sesjonsvisning;
      this.lagreSesjonsvisninger(sesjonsvisninger);
      return sesjonsvisning;
    }
  }

  public oppdaterSesjon(sesjon: TSesjon) {
    var sesjoner = this.hentSesjoner();
    sesjoner[sesjoner.map(s => s.id).indexOf(sesjon.id)] = sesjon;
    this.lagreSesjoner(sesjoner);
  }

  public hentSesjonsvisningForSesjon(sesjonId: string): TSesjonsvisning {
    return this.hentSesjonsvisninger().filter(s => s.sesjonId == sesjonId)[0] as TSesjonsvisning;
  }

  protected hentSesjonsvisninger(): TSesjonsvisning[] {
    if (localStorage.getItem(this.sesjonsvisningLocalStoragePath) != null) {
      return JSON.parse(localStorage.getItem(this.sesjonsvisningLocalStoragePath)) as TSesjonsvisning[];
    }
    return [];
  }

  public hentSesjon(sesjonId: string): TSesjon {
    if (this.hentSesjoner().filter(s => s.id == sesjonId).length > 0) {
      return this.hentSesjoner().filter(s => s.id == sesjonId)[0] as TSesjon;
    }
    return null;
  }

  public async registrerObservasjon(observasjon: TObservasjon): Promise<void> {
    var finnesEksisterendeSesjon = this.hentSesjoner().filter(s => s.id == observasjon.sesjonId).length > 0;
    if (finnesEksisterendeSesjon == false) {
      await this.opprettSesjonMedObservasjon(observasjon);
      return;
    }

    let sesjoner = this.hentSesjoner();
    var eksisterendeSesjonIndex = sesjoner.map(s => s.id).indexOf(observasjon.sesjonId);
    sesjoner[eksisterendeSesjonIndex].observasjoner.push(observasjon);
    this.lagreSesjoner(sesjoner);
  }

  public endreObservasjon(endretObservasjon: TObservasjon) {
    var sesjoner = this.hentSesjoner();
    var aktuellSesjon = sesjoner.find(s => s.id == endretObservasjon.sesjonId);
    var aktuellSesjonIndeks = sesjoner.indexOf(aktuellSesjon);
    var observasjonSomEndres = aktuellSesjon.observasjoner.find(o => o.id === endretObservasjon.id);
    var observasjonIndeks = aktuellSesjon.observasjoner.indexOf(observasjonSomEndres);
    aktuellSesjon.observasjoner[observasjonIndeks] = endretObservasjon;
    sesjoner[aktuellSesjonIndeks] = aktuellSesjon;
    this.lagreSesjoner(sesjoner);
  }

  public slettObservasjon(observasjonSomSkalSlettes: TObservasjon) {
    var sesjoner = this.hentSesjoner();
    var aktuellSesjon = sesjoner.find(s => s.id == observasjonSomSkalSlettes.sesjonId);
    var aktuellSesjonIndeks = sesjoner.indexOf(aktuellSesjon);
    aktuellSesjon.observasjoner = aktuellSesjon.observasjoner.filter(o => o.id !== observasjonSomSkalSlettes.id)
    sesjoner[aktuellSesjonIndeks] = aktuellSesjon;
    this.lagreSesjoner(sesjoner);
  }

  public antallSesjoner() : number{
    return this.hentSesjoner().length;
  }

  protected async opprettSesjonMedObservasjon(observasjon: TObservasjon) {
    let sesjonsvisning = this.hentSesjonsvisningForSesjon(observasjon.sesjonId);
    let sesjoner = this.hentSesjoner();
    let institusjon = await this.institusjonService.getInstitusjon(sesjonsvisning.avdeling.institusjonId).toPromise();
    let nySesjon = {
      id: observasjon.sesjonId,
      observasjoner: [observasjon],
      starttidspunkt: new Date(),
      avdeling: sesjonsvisning.avdeling,
      institusjonsnavn: institusjon.navn
    } as TSesjon;
    sesjoner.push(nySesjon);
    this.lagreSesjoner(sesjoner);
  }

}

