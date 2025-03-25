import { Rolle } from '../api/Rolle';
import { Kort } from './kort.model';
import {Avdeling} from '../api/Avdeling';
import {BaseSesjonsvisning} from './base-sesjonsvisning.model';

export interface HanskeSesjonsvisning extends BaseSesjonsvisning
{
  kort?: Kort[];
  hanskebrukSkalRegistreres: boolean;
}
