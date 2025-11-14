import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { ProtectiveEquipmentType } from '../models/api/ProtectiveEquipmentType';

import { ProtectiveEquipment } from '../models/api/ProtectiveEquipment';
import { ProtectiveEquipmentTypeConstants } from '../models/api/ProtectiveEquipmentTypeConstants';
import { faHeadSideMask } from '@fortawesome/free-solid-svg-icons';
import { faHandPaper } from '@fortawesome/free-regular-svg-icons';
import { faAnderettsvern, faHette, faPlastforkle, faSmittefrakk, faStellefrakk, faVernebriller } from './customIcons';

export class ProtectiveEquipmentMapper {

  public static getIconTypeMap(): Map<string, IconProp> {
    let iconmap = new Map<string, IconProp>();
    iconmap.set(ProtectiveEquipmentTypeConstants.Gloves, faHandPaper);
    iconmap.set(ProtectiveEquipmentTypeConstants.InfectionGown, faSmittefrakk);
    iconmap.set(ProtectiveEquipmentTypeConstants.FaceMask, faHeadSideMask);
    iconmap.set(ProtectiveEquipmentTypeConstants.EyeProtection, faVernebriller);
    iconmap.set(ProtectiveEquipmentTypeConstants.RespiratoryProtection, faAnderettsvern);
    iconmap.set(ProtectiveEquipmentTypeConstants.Hood, faHette);
    iconmap.set(ProtectiveEquipmentTypeConstants.PlasticApron, faPlastforkle);
    iconmap.set(ProtectiveEquipmentTypeConstants.CareGown, faStellefrakk);
    return iconmap;
  }

  public static getProtectiveEquipmentSelection(protectiveEquipment: ProtectiveEquipmentType[]): ProtectiveEquipment[] {
    return protectiveEquipment.reduce((acc, item) => {
      acc.push({
        wasUsed: false,
        isRequired: item.isRequired,
        equipmentType: item,
        wasUsedCorrectly: false,
        misuseTypes: [],
        comment: ""
      } as ProtectiveEquipment);
      return acc;
    }, []) as ProtectiveEquipment[];
  }
}
