import { OrganisationUnit } from '../api/OrganisationUnit';
import {ProtectiveEquipmentSettingType} from '../api/ProtectiveEquipmentSettingType';
import {ProtectiveEquipmentCard} from './protectiveEquipment-card.model';

export class ProtectiveEquipmentSessionView {
  sessionId: string;
  department: OrganisationUnit;
  setting: ProtectiveEquipmentSettingType;
  card: ProtectiveEquipmentCard[];
}

