import { IconProp } from '@fortawesome/fontawesome-svg-core';
import { ProtectiveEquipmentType } from '../models/api/ProtectiveEquipmentType';

import { ProtectiveEquipment } from '../models/api/ProtectiveEquipment';
import { ProtectiveEquipmentTypeConstants } from '../models/api/ProtectiveEquipmentTypeConstants';
import { faHeadSideMask } from '@fortawesome/free-solid-svg-icons';
import { faHandPaper } from '@fortawesome/free-regular-svg-icons';
import { faAnderettsvern, faHette, faPlastforkle, faSmittefrakk, faStellefrakk, faVernebriller } from './customIkoner';

export class BeskyttelsesutstyrMapper {

  public static getIkontypeMap(): Map<string, IconProp> {
    var ikonmap = new Map<string, IconProp>();
    ikonmap.set(ProtectiveEquipmentTypeConstants.Gloves, faHandPaper);
    ikonmap.set(ProtectiveEquipmentTypeConstants.InfectionGown, faSmittefrakk);
    ikonmap.set(ProtectiveEquipmentTypeConstants.FaceMask, faHeadSideMask);
    ikonmap.set(ProtectiveEquipmentTypeConstants.EyeProtection, faVernebriller);
    ikonmap.set(ProtectiveEquipmentTypeConstants.RespiratoryProtection, faAnderettsvern);
    ikonmap.set(ProtectiveEquipmentTypeConstants.Hood, faHette);
    ikonmap.set(ProtectiveEquipmentTypeConstants.PlasticApron, faPlastforkle);
    ikonmap.set(ProtectiveEquipmentTypeConstants.CareGown, faStellefrakk);
    return ikonmap;
  }

  public static getBeskyttelsesutstyrvalg(protectiveEquipment: ProtectiveEquipmentType[]): ProtectiveEquipment[] {
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
