import {Department} from '../api/Department';
import {BeskyttelsesutstyrsettingType} from '../api/BeskyttelsesutstyrsettingType';
import {BeskyttelsesutstyrKort} from './beskyttelsesutstyr-kort.model';

export class BeskyttelsesutstyrSesjonsvisning {
  sesjonId: string;
  avdeling: Department;
  setting: BeskyttelsesutstyrsettingType;
  kort: BeskyttelsesutstyrKort[];
}

