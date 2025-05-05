import {Card} from './card.model';
import {ProtectiveEquipmentType} from '../api/ProtectiveEquipmentType';

export interface ProtectiveEquipmentCard extends Card {
  utstyr: ProtectiveEquipmentType[];
}
