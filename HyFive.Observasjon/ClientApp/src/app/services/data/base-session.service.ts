import { Session } from '../../models/api/Session';
import { BaseSessionView } from '../../models/registration/base-sessionView.model';
import { Observation } from '../../models/api/Observation';
import { InstitutionService } from './InstitutionService';
import {AjaxResponse} from 'rxjs/ajax';
import { DateHelper } from 'src/app/utils/datehelper';

export abstract class BaseSessionService<TSesjonsvisning extends BaseSessionView, TSesjon extends Session<TObservasjon>, TObservasjon extends Observation>  {

  abstract sessionShowLocalStoragePath: string;
  abstract sessionLocalStoragePath: string;

  constructor(
    public institutionService: InstitutionService) {
  }

  protected saveSessionViews(sessionViews: TSesjonsvisning[]) {
    localStorage.setItem(this.sessionShowLocalStoragePath, JSON.stringify(sessionViews));
  }

  protected lagreSesjoner(sessions: TSesjon[]) {
    localStorage.setItem(this.sessionLocalStoragePath, JSON.stringify(sessions, DateHelper.dateTimeSomLocaleStringReplacer));
  }

  public slettSesjon(sessionId: string) {
    let sessions = this.getSessions().filter(s => s.id !== sessionId);
    this.lagreSesjoner(sessions);
    let sessionViews = this.getSessionViews().filter(s => s.sessionId !== sessionId);
    this.saveSessionViews(sessionViews);
  }

  public getSessions(): TSesjon[] {
    let sessions: TSesjon[] = [];
    const sesjonerString = localStorage.getItem(this.sessionLocalStoragePath);
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

  public updateSessionViewForSession(sessionView: TSesjonsvisning): TSesjonsvisning {
    let eksisterendeSesjonsvisning = this.getSessionViewForSession(sessionView.sessionId);
    if (eksisterendeSesjonsvisning) {
      let sessionViews = this.getSessionViews();
      var eksisterendeSesjonsvisningIndex = sessionViews.map(s => s.sessionId).indexOf(sessionView.sessionId);
      sessionViews[eksisterendeSesjonsvisningIndex] = sessionView;
      this.saveSessionViews(sessionViews);
      return sessionView;
    }
  }

  public oppdaterSesjon(sesjon: TSesjon) {
    var sessions = this.getSessions();
    sessions[sessions.map(s => s.id).indexOf(sesjon.id)] = sesjon;
    this.lagreSesjoner(sessions);
  }

  public getSessionViewForSession(sessionId: string): TSesjonsvisning {
    return this.getSessionViews().filter(s => s.sessionId == sessionId)[0] as TSesjonsvisning;
  }

  protected getSessionViews(): TSesjonsvisning[] {
    if (localStorage.getItem(this.sessionShowLocalStoragePath) != null) {
      return JSON.parse(localStorage.getItem(this.sessionShowLocalStoragePath)) as TSesjonsvisning[];
    }
    return [];
  }

  public getSession(sessionId: string): TSesjon {
    if (this.getSessions().filter(s => s.id == sessionId).length > 0) {
      return this.getSessions().filter(s => s.id == sessionId)[0] as TSesjon;
    }
    return null;
  }

  public async registerObservation(observation: TObservasjon): Promise<void> {
    var finnesEksisterendeSesjon = this.getSessions().filter(s => s.id == observation.sessionId).length > 0;
    if (finnesEksisterendeSesjon == false) {
      await this.opprettSesjonMedObservasjon(observation);
      return;
    }

    let sessions = this.getSessions();
    var eksisterendeSesjonIndex = sessions.map(s => s.id).indexOf(observation.sessionId);
    sessions[eksisterendeSesjonIndex].observations.push(observation);
    this.lagreSesjoner(sessions);
  }

  public endreObservasjon(endretObservasjon: TObservasjon) {
    var sessions = this.getSessions();
    var aktuellSesjon = sessions.find(s => s.id == endretObservasjon.sessionId);
    var aktuellSesjonIndeks = sessions.indexOf(aktuellSesjon);
    var observasjonSomEndres = aktuellSesjon.observations.find(o => o.id === endretObservasjon.id);
    var observasjonIndeks = aktuellSesjon.observations.indexOf(observasjonSomEndres);
    aktuellSesjon.observations[observasjonIndeks] = endretObservasjon;
    sessions[aktuellSesjonIndeks] = aktuellSesjon;
    this.lagreSesjoner(sessions);
  }

  public slettObservasjon(observasjonSomSkalSlettes: TObservasjon) {
    var sessions = this.getSessions();
    var aktuellSesjon = sessions.find(s => s.id == observasjonSomSkalSlettes.sessionId);
    var aktuellSesjonIndeks = sessions.indexOf(aktuellSesjon);
    aktuellSesjon.observations = aktuellSesjon.observations.filter(o => o.id !== observasjonSomSkalSlettes.id)
    sessions[aktuellSesjonIndeks] = aktuellSesjon;
    this.lagreSesjoner(sessions);
  }

  public numberOfSessions() : number{
    return this.getSessions().length;
  }

  protected async opprettSesjonMedObservasjon(observation: TObservasjon) {
    let sessionView = this.getSessionViewForSession(observation.sessionId);
    let sessions = this.getSessions();
    let institution = await this.institutionService.getInstitution(sessionView.department.institutionId).toPromise();
    let nySesjon = {
      id: observation.sessionId,
      observations: [observation],
      startTime: new Date(),
      department: sessionView.department,
      institutionsName: institution.name
    } as TSesjon;
    sessions.push(nySesjon);
    this.lagreSesjoner(sessions);
  }

}

