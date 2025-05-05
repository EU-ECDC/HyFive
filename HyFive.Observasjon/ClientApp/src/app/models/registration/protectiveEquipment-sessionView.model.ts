import {Department} from '../api/Department';
import {ProtectiveEquipmentSettingType} from '../api/ProtectiveEquipmentSettingType';
import {ProtectiveEquipmentCard} from './protectiveEquipment-card.model';

export class ProtectiveEquipmentSessionView {
  sessionId: string;
  department: Department;
  setting: ProtectiveEquipmentSettingType;
  card: ProtectiveEquipmentCard[];
}

