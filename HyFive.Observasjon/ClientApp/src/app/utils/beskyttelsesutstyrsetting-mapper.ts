import {IconProp} from '@fortawesome/fontawesome-svg-core';
import { faHandSpock } from '@fortawesome/free-regular-svg-icons';
import {ProtectiveEquipmentSettingTypeConstants} from '../models/api/ProtectiveEquipmentSettingTypeConstants';
import { faDroplet, faFaceSideCough, faBacteria } from './customIkoner';

export class BeskyttelsesutstyrsettingMapper {
  public static getIkontypeMap() : Map<string, IconProp>{
    var ikonmap = new Map<string, IconProp>();
    ikonmap.set(ProtectiveEquipmentSettingTypeConstants.BasicInfectionControlRoutines, faBacteria);
    ikonmap.set(ProtectiveEquipmentSettingTypeConstants.ContactTransmission, faHandSpock);
    ikonmap.set(ProtectiveEquipmentSettingTypeConstants.DropletTransmission, faDroplet);
    ikonmap.set(ProtectiveEquipmentSettingTypeConstants.AirborneTransmission, faFaceSideCough);
    return ikonmap;
  }
}
