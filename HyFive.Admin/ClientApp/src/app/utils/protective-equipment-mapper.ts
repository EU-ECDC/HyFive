import {ProtectiveEquipmentTypeConstants} from "../models/api/ProtectiveEquipmentTypeConstants";
import {faUserTimes} from "@fortawesome/free-solid-svg-icons";
import {IconProp} from "@fortawesome/fontawesome-svg-core";
import {ProtectiveEquipmentType } from '../models/api/ProtectiveEquipmentType';
import {ProtectiveEquipment} from "../models/api/ProtectiveEquipment";

export class ProtectiveEquipmentMapper {

  public static getIconTypeMap(): Map<string, IconProp> {
    var iconmap = new Map<string, IconProp>();
    iconmap.set(ProtectiveEquipmentTypeConstants.Gloves, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.InfectionGown, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.FaceMask, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.EyeProtection, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.RespiratoryProtection, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.Hood, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.PlasticApron, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.CareGown, faUserTimes);
    return iconmap;
  }

  public static 
  getProtectiveEquipmentSelection(protectiveEquipment: ProtectiveEquipmentType[], valg: ProtectiveEquipment ): ProtectiveEquipment[] {
    return protectiveEquipment.reduce((acc, item) => {
      acc.push({
        id: valg.id,
        wasUsed: false,
        isIndicated: valg.isIndicated,
        equipmentType: item,
        wasUsedCorrectly: false,
        misuseTypes: [],
        comment: ""
      } as ProtectiveEquipment);
      return acc;
    }, []) as ProtectiveEquipment[];
  }
}
