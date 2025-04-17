import { SessionType } from '../models/api/SessionType';

export class SesjonstypeRapportUrlMapper {

  public static getRapportUrlMap() : Map<SessionType, string> {
    let rapportUrlMap = new Map<SessionType, string>();
    rapportUrlMap.set(SessionType.Handsmykker,        '/api/v1/rapport/avdeling/handsmykker/excel');
    rapportUrlMap.set(SessionType.FireIndikasjoner,   '/api/v1/rapport/avdeling/fireindikasjoner/excel');
    rapportUrlMap.set(SessionType.Hansker,            '/api/v1/rapport/avdeling/hansker/excel');
    rapportUrlMap.set(SessionType.Beskyttelsesutstyr, '/api/v1/rapport/avdeling/beskyttelsesutstyr/excel');
    return rapportUrlMap;

  }
}
