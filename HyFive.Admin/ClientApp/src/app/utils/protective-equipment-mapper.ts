import {ProtectiveEquipmentTypeConstants} from "../models/api/ProtectiveEquipmentTypeConstants";
import {faUserTimes} from "@fortawesome/free-solid-svg-icons";
import {IconProp} from "@fortawesome/fontawesome-svg-core";
import { ProtectiveEquipmentTypeq } from '../models/api/ProtectiveEquipmentTypeq';
import {ProtectiveEquipment} from "../models/api/ProtectiveEquipment";

export class ProtectiveEquipmentMapper {

  public static getIconTypeMap(): Map<string, IconProp> {
    var iconmap = new Map<string, IconProp>();
    iconmap.set(ProtectiveEquipmentTypeConstants.Gloves, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.InfectiousCoat, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.FaceMask, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.EyeProtection, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.RespiratoryProtection, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.Hood, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.PlasticApron, faUserTimes);
    iconmap.set(ProtectiveEquipmentTypeConstants.ChangingCoat, faUserTimes);
    return iconmap;
  }

  public static 
  getProtectiveEquipmentSelection(protectiveEquipment: ProtectiveEquipmentTypeq[], valg: ProtectiveEquipment ): ProtectiveEquipment[] {
    return protectiveEquipment.reduce((acc, item) => {
      acc.push({
        id: valg.id,
        wasUsed: false,
        isIndicated: valg.isIndicated,
        equipmentTypeq: item,
        wasUsedProperly: false,
        misuseTypes: [],
        comment: ""
      } as ProtectiveEquipment);
      return acc;
    }, []) as ProtectiveEquipment[];
  }
}
