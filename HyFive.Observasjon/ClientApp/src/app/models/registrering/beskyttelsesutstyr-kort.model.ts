import {Kort} from './kort.model';
import {ProtectiveEquipmentType} from '../api/ProtectiveEquipmentType';

export interface BeskyttelsesutstyrKort extends Kort {
  utstyr: ProtectiveEquipmentType[];
}
