import { Session } from '../../models/api/Session';
import { BaseSessionView } from '../../models/registration/base-sessionView.model';
import { Observation } from '../../models/api/Observation';
import { InstitutionService } from './InstitutionService';
import {AjaxResponse} from 'rxjs/ajax';
import { DateHelper } from 'src/app/utils/datehelper';

export abstract class BaseSessionService<TSessionView extends BaseSessionView, TSession extends Session<TObservation>, TObservation extends Observation>  {

  abstract sessionShowLocalStoragePath: string;
  abstract sessionLocalStoragePath: string;

  constructor(
    public institutionService: InstitutionService) {
  }

  protected saveSessionViews(sessionViews: TSessionView[]) {
    localStorage.setItem(this.sessionShowLocalStoragePath, JSON.stringify(sessionViews));
  }

  protected saveSessions(sessions: TSession[]) {
    localStorage.setItem(this.sessionLocalStoragePath, JSON.stringify(sessions, DateHelper.dateTimeAsLocaleStringReplacer));
  }

  public deleteSession(sessionId: string) {
    let sessions = this.getSessions().filter(s => s.id !== sessionId);
    this.saveSessions(sessions);
    let sessionViews = this.getSessionViews().filter(s => s.sessionId !== sessionId);
    this.saveSessionViews(sessionViews);
  }

  public getSessions(): TSession[] {
    let sessions: TSession[] = [];
    const sessionsString = localStorage.getItem(this.sessionLocalStoragePath);
    if (sessionsString != null) {
      sessions = JSON.parse(sessionsString);
    }
    return sessions.sort((s1, s2) => {
      if (s1.createdDate == null && s2.createdDate != null)
        return 1;
      if (s2.createdDate == null && s1.createdDate != null)
        return -1;
      if (s2.createdDate === s1.createdDate)
        return 0;
      return s1.createdDate < s2.createdDate ? 1 : -1
    });
  }

  public updateSessionViewForSession(sessionView: TSessionView): TSessionView {
    let existingSessionView = this.getSessionViewForSession(sessionView.sessionId);
    if (existingSessionView) {
      let sessionViews = this.getSessionViews();
      var existingSessionViewIndex = sessionViews.map(s => s.sessionId).indexOf(sessionView.sessionId);
      sessionViews[existingSessionViewIndex] = sessionView;
      this.saveSessionViews(sessionViews);
      return sessionView;
    }
  }

  public updateSession(session: TSession) {
    var sessions = this.getSessions();
    sessions[sessions.map(s => s.id).indexOf(session.id)] = session;
    this.saveSessions(sessions);
  }

  public getSessionViewForSession(sessionId: string): TSessionView {
    return this.getSessionViews().filter(s => s.sessionId == sessionId)[0] as TSessionView;
  }

  protected getSessionViews(): TSessionView[] {
    if (localStorage.getItem(this.sessionShowLocalStoragePath) != null) {
      return JSON.parse(localStorage.getItem(this.sessionShowLocalStoragePath)) as TSessionView[];
    }
    return [];
  }

  public getSession(sessionId: string): TSession {
    if (this.getSessions().filter(s => s.id == sessionId).length > 0) {
      return this.getSessions().filter(s => s.id == sessionId)[0] as TSession;
    }
    return null;
  }

  public async registerObservation(observation: TObservation): Promise<void> {
    var existsExistingSession = this.getSessions().filter(s => s.id == observation.sessionId).length > 0;
    if (existsExistingSession == false) {
      await this.createSessionWithObservation(observation);
      return;
    }

    let sessions = this.getSessions();
    var existingSessionIndex = sessions.map(s => s.id).indexOf(observation.sessionId);
    sessions[existingSessionIndex].observations.push(observation);
    this.saveSessions(sessions);
  }

  public changeObservation(changedObservation: TObservation) {
    var sessions = this.getSessions();
    var currentSession = sessions.find(s => s.id == changedObservation.sessionId);
    var currentSessionIndex = sessions.indexOf(currentSession);
    var observationAsChanged = currentSession.observations.find(o => o.id === changedObservation.id);
    var observationIndex = currentSession.observations.indexOf(observationAsChanged);
    currentSession.observations[observationIndex] = changedObservation;
    sessions[currentSessionIndex] = currentSession;
    this.saveSessions(sessions);
  }

  public deleteObservation(observationToBeDeleted: TObservation) {
    var sessions = this.getSessions();
    var currentSession = sessions.find(s => s.id == observationToBeDeleted.sessionId);
    var currentSessionIndex = sessions.indexOf(currentSession);
    currentSession.observations = currentSession.observations.filter(o => o.id !== observationToBeDeleted.id)
    sessions[currentSessionIndex] = currentSession;
    this.saveSessions(sessions);
  }

  public numberOfSessions() : number{
    return this.getSessions().length;
  }

  protected async createSessionWithObservation(observation: TObservation) {
    let sessionView = this.getSessionViewForSession(observation.sessionId);
    let sessions = this.getSessions();
    let institution = await this.institutionService.getInstitution(sessionView.department.institutionId).toPromise();
    let newSession = {
      id: observation.sessionId,
      observations: [observation],
      createdDate: new Date(),
      department: sessionView.department,
      institutionsName: institution.name
    } as TSession;
    sessions.push(newSession);
    this.saveSessions(sessions);
  }

}

