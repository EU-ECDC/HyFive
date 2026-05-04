import { SessionType } from '../models/api/SessionType';

export class SessionTypeMapper {

  public static getNameMap(): Map<SessionType, string>{
    let nameMap = new Map<SessionType, string>();
    nameMap.set(SessionType.HandJewelry, 'Bare below elbows');
    nameMap.set(SessionType.HandHygiene, 'Hand hygiene');
    nameMap.set(SessionType.Gloves, 'Gloves');
    nameMap.set(SessionType.InOut, 'In/Out');
    nameMap.set(SessionType.ProtectiveEquipment, 'Protective equipment');
    return nameMap;
  }
}
