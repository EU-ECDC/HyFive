import { SessionType } from '../models/api/SessionType';

export class SesjonstypeRapportUrlMapper {

  public static getRapportUrlMap() : Map<SessionType, string> {
    let rapportUrlMap = new Map<SessionType, string>();
    rapportUrlMap.set(SessionType.Handjewelry,        '/api/v1/report/avdeling/handsmykker/excel');
    rapportUrlMap.set(SessionType.FourIndications,   '/api/v1/report/avdeling/fourindications/excel');
    rapportUrlMap.set(SessionType.Gloves,            '/api/v1/report/avdeling/hansker/excel');
    rapportUrlMap.set(SessionType.ProtectiveEquipment, '/api/v1/report/avdeling/beskyttelsesutstyr/excel');
    return rapportUrlMap;

  }
}
