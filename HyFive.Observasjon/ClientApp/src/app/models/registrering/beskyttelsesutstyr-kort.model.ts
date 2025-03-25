import {Kort} from './kort.model';
import {BeskyttelsesutstyrType} from '../api/BeskyttelsesutstyrType';

export interface BeskyttelsesutstyrKort extends Kort {
  utstyr: BeskyttelsesutstyrType[];
}
