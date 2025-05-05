import { SessionType } from '../models/api/SessionType';

export class SesjonTypeMapper {

  public static getNameMap(): Map<SessionType, string>{
    var navnMap = new Map<SessionType, string>();
    navnMap.set(SessionType.HandJewelry, 'Håndsmykker');
    navnMap.set(SessionType.FourIndications, 'Fire indikasjoner');
    navnMap.set(SessionType.Gloves, 'Gloves');
    navnMap.set(SessionType.InOut, 'Inn/Ut');
    navnMap.set(SessionType.ProtectiveEquipment, 'Verneutstyr');
    return navnMap;
  }
}
