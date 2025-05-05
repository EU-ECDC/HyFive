import { Session } from '../../models/api/Session';
import { BaseSesjonsvisning } from '../../models/registrering/base-sesjonsvisning.model';
import { Observation } from '../../models/api/Observation';
import { InstitusjonService } from './institusjon.service';
import {AjaxResponse} from 'rxjs/ajax';
import { DatoHjelper } from 'src/app/utils/datohjelper';

export abstract class BaseSesjonService<TSesjonsvisning extends BaseSesjonsvisning, TSesjon extends Session<TObservasjon>, TObservasjon extends Observation>  {

  abstract sesjonsvisningLocalStoragePath: string;
  abstract sesjonLocalStoragePath: string;

  constructor(
    public institusjonService: InstitusjonService) {
  }

  protected lagreSesjonsvisninger(sesjonsvisninger: TSesjonsvisning[]) {
    localStorage.setItem(this.sesjonsvisningLocalStoragePath, JSON.stringify(sesjonsvisninger));
  }

  protected lagreSesjoner(sessions: TSesjon[]) {
    localStorage.setItem(this.sesjonLocalStoragePath, JSON.stringify(sessions, DatoHjelper.dateTimeSomLocaleStringReplacer));
  }

  public slettSesjon(sessionId: string) {
    let sessions = this.hentSesjoner().filter(s => s.id !== sessionId);
    this.lagreSesjoner(sessions);
    let sesjonsvisninger = this.hentSesjonsvisninger().filter(s => s.sessionId !== sessionId);
    this.lagreSesjonsvisninger(sesjonsvisninger);
  }

  public hentSesjoner(): TSesjon[] {
    let sessions: TSesjon[] = [];
    const sesjonerString = localStorage.getItem(this.sesjonLocalStoragePath);
    if (sesjonerString != null) {
      sessions = JSON.parse(sesjonerString);
    }
    return sessions.sort((s1, s2) => {
      if (s1.startTime == null && s2.startTime != null)
        return 1;
      if (s2.startTime == null && s1.startTime != null)
        return -1;
      if (s2.startTime === s1.startTime)
        return 0;
      return s1.startTime < s2.startTime ? 1 : -1
    });
  }

  public oppdaterSesjonsvisningForSesjon(sesjonsvisning: TSesjonsvisning): TSesjonsvisning {
    let eksisterendeSesjonsvisning = this.hentSesjonsvisningForSesjon(sesjonsvisning.sessionId);
    if (eksisterendeSesjonsvisning) {
      let sesjonsvisninger = this.hentSesjonsvisninger();
      var eksisterendeSesjonsvisningIndex = sesjonsvisninger.map(s => s.sessionId).indexOf(sesjonsvisning.sessionId);
      sesjonsvisninger[eksisterendeSesjonsvisningIndex] = sesjonsvisning;
      this.lagreSesjonsvisninger(sesjonsvisninger);
      return sesjonsvisning;
    }
  }

  public oppdaterSesjon(sesjon: TSesjon) {
    var sessions = this.hentSesjoner();
    sessions[sessions.map(s => s.id).indexOf(sesjon.id)] = sesjon;
    this.lagreSesjoner(sessions);
  }

  public hentSesjonsvisningForSesjon(sessionId: string): TSesjonsvisning {
    return this.hentSesjonsvisninger().filter(s => s.sessionId == sessionId)[0] as TSesjonsvisning;
  }

  protected hentSesjonsvisninger(): TSesjonsvisning[] {
    if (localStorage.getItem(this.sesjonsvisningLocalStoragePath) != null) {
      return JSON.parse(localStorage.getItem(this.sesjonsvisningLocalStoragePath)) as TSesjonsvisning[];
    }
    return [];
  }

  public hentSesjon(sessionId: string): TSesjon {
    if (this.hentSesjoner().filter(s => s.id == sessionId).length > 0) {
      return this.hentSesjoner().filter(s => s.id == sessionId)[0] as TSesjon;
    }
    return null;
  }

  public async registrerObservasjon(observasjon: TObservasjon): Promise<void> {
    var finnesEksisterendeSesjon = this.hentSesjoner().filter(s => s.id == observasjon.sessionId).length > 0;
    if (finnesEksisterendeSesjon == false) {
      await this.opprettSesjonMedObservasjon(observasjon);
      return;
    }

    let sessions = this.hentSesjoner();
    var eksisterendeSesjonIndex = sessions.map(s => s.id).indexOf(observasjon.sessionId);
    sessions[eksisterendeSesjonIndex].observations.push(observasjon);
    this.lagreSesjoner(sessions);
  }

  public endreObservasjon(endretObservasjon: TObservasjon) {
    var sessions = this.hentSesjoner();
    var aktuellSesjon = sessions.find(s => s.id == endretObservasjon.sessionId);
    var aktuellSesjonIndeks = sessions.indexOf(aktuellSesjon);
    var observasjonSomEndres = aktuellSesjon.observations.find(o => o.id === endretObservasjon.id);
    var observasjonIndeks = aktuellSesjon.observations.indexOf(observasjonSomEndres);
    aktuellSesjon.observations[observasjonIndeks] = endretObservasjon;
    sessions[aktuellSesjonIndeks] = aktuellSesjon;
    this.lagreSesjoner(sessions);
  }

  public slettObservasjon(observasjonSomSkalSlettes: TObservasjon) {
    var sessions = this.hentSesjoner();
    var aktuellSesjon = sessions.find(s => s.id == observasjonSomSkalSlettes.sessionId);
    var aktuellSesjonIndeks = sessions.indexOf(aktuellSesjon);
    aktuellSesjon.observations = aktuellSesjon.observations.filter(o => o.id !== observasjonSomSkalSlettes.id)
    sessions[aktuellSesjonIndeks] = aktuellSesjon;
    this.lagreSesjoner(sessions);
  }

  public numberOfSessions() : number{
    return this.hentSesjoner().length;
  }

  protected async opprettSesjonMedObservasjon(observasjon: TObservasjon) {
    let sesjonsvisning = this.hentSesjonsvisningForSesjon(observasjon.sessionId);
    let sessions = this.hentSesjoner();
    let institusjon = await this.institusjonService.getInstitusjon(sesjonsvisning.department.institutionId).toPromise();
    let nySesjon = {
      id: observasjon.sessionId,
      observations: [observasjon],
      startTime: new Date(),
      department: sesjonsvisning.department,
      institutionsName: institusjon.name
    } as TSesjon;
    sessions.push(nySesjon);
    this.lagreSesjoner(sessions);
  }

}

