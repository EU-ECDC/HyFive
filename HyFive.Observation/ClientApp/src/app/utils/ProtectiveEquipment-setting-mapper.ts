import {IconProp} from '@fortawesome/fontawesome-svg-core';
import { faHandSpock } from '@fortawesome/free-regular-svg-icons';
import {ProtectiveEquipmentSettingTypeConstants} from '../models/api/ProtectiveEquipmentSettingTypeConstants';
import { faDroplet, faFaceSideCough, faBacteria } from './customIconer';

export class ProtectiveEquipmentSettingMapper {
  public static getIconTypeMap() : Map<string, IconProp>{
    var iconmap = new Map<string, IconProp>();
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.BasicInfectionControlRoutines, faBacteria);
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.ContactInfection, faHandSpock);
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.DropletInfection, faDroplet);
    iconmap.set(ProtectiveEquipmentSettingTypeConstants.AirInfection, faFaceSideCough);
    return iconmap;
  }
}
