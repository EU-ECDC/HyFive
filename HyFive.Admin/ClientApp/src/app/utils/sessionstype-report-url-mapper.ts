import { SessionType } from '../models/api/SessionType';

export class SessionTypeReportUrlMapper {

  public static getReportUrlMap() : Map<SessionType, string> {
    let reportUrlMap = new Map<SessionType, string>();
    reportUrlMap.set(SessionType.HandJewelry,        '/api/v1/report/department/handJewelry/excel');
    reportUrlMap.set(SessionType.FiveIndications,   '/api/v1/report/department/fiveIndications/excel');
    reportUrlMap.set(SessionType.Gloves,            '/api/v1/report/department/gloves/excel');
    reportUrlMap.set(SessionType.ProtectiveEquipment, '/api/v1/report/department/protectiveEquipment/excel');
    return reportUrlMap;

  }
}
