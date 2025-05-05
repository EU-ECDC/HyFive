import {IconProp} from '@fortawesome/fontawesome-svg-core';
import { faHandSpock } from '@fortawesome/free-regular-svg-icons';
import {ProtectiveEquipmentSettingTypeConstants} from '../models/api/ProtectiveEquipmentSettingTypeConstants';
import { faDroplet, faFaceSideCough, faBacteria } from './customIconer';

export class BeskyttelsesutstyrsettingMapper {
  public static getIconTypeMap() : Map<string, IconProp>{
    var iconmap = new Map<string, IconProp>();
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.BasicInfectionControlRoutines, faBacteria);
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.ContactTransmission, faHandSpock);
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.DropletTransmission, faDroplet);
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.AirborneTransmission, faFaceSideCough);
    return iconmap;
  }
}
