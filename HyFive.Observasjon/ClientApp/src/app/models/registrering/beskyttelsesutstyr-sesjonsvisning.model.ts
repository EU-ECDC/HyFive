import {Avdeling} from '../api/Avdeling';
import {BeskyttelsesutstyrsettingType} from '../api/BeskyttelsesutstyrsettingType';
import {BeskyttelsesutstyrKort} from './beskyttelsesutstyr-kort.model';

export class BeskyttelsesutstyrSesjonsvisning {
  sesjonId: string;
  avdeling: Avdeling;
  setting: BeskyttelsesutstyrsettingType;
  kort: BeskyttelsesutstyrKort[];
}

