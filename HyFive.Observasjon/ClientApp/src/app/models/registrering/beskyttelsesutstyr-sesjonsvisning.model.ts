import {Department} from '../api/Department';
import {ProtectiveEquipmentSettingType} from '../api/ProtectiveEquipmentSettingType';
import {BeskyttelsesutstyrKort} from './beskyttelsesutstyr-kort.model';

export class BeskyttelsesutstyrSesjonsvisning {
  sesjonId: string;
  department: Department;
  setting: ProtectiveEquipmentSettingType;
  kort: BeskyttelsesutstyrKort[];
}

