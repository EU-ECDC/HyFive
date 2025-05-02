import { SessionType } from '../models/api/SessionType';

export class SesjonTypeMapper {

  public static getNameMap(): Map<SessionType, string>{
    var navnMap = new Map<SessionType, string>();
    navnMap.set(SessionType.Handsmykker, 'Håndsmykker');
    navnMap.set(SessionType.FireIndikasjoner, 'Fire indikasjoner');
    navnMap.set(SessionType.Hansker, 'Hansker');
    navnMap.set(SessionType.InnUt, 'Inn/Ut');
    navnMap.set(SessionType.ProtectiveEquipment, 'Verneutstyr');
    return navnMap;
  }
}
