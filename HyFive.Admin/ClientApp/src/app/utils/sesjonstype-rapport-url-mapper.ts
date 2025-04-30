import { SessionType } from '../models/api/SessionType';

export class SessionTypeReportUrlMapper {

  public static getReportUrlMap() : Map<SessionType, string> {
    let reportUrlMap = new Map<SessionType, string>();
    reportUrlMap.set(SessionType.HandJewelry,        '/api/v1/report/avdeling/handsmykker/excel');
    reportUrlMap.set(SessionType.FourIndications,   '/api/v1/report/avdeling/fourindications/excel');
    reportUrlMap.set(SessionType.Gloves,            '/api/v1/report/avdeling/hansker/excel');
    reportUrlMap.set(SessionType.ProtectiveEquipment, '/api/v1/report/avdeling/beskyttelsesutstyr/excel');
    return reportUrlMap;

  }
}
