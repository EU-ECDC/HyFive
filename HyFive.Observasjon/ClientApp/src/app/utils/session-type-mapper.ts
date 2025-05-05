import { SessionType } from '../models/api/SessionType';

export class SessionTypeMapper {

  public static getNameMap(): Map<SessionType, string>{
    var nameMap = new Map<SessionType, string>();
    nameMap.set(SessionType.HandJewelry, 'Handjewelry');
    nameMap.set(SessionType.FourIndications, 'Four indications');
    nameMap.set(SessionType.Gloves, 'Gloves');
    nameMap.set(SessionType.InOut, 'In/Out');
    nameMap.set(SessionType.ProtectiveEquipment, 'Protective equipment');
    return nameMap;
  }
}
