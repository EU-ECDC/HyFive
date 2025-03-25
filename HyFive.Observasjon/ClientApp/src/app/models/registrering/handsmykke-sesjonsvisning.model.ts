import { Kort } from './kort.model';
import {BaseSesjonsvisning} from './base-sesjonsvisning.model';

export interface HandsmykkeSesjonsvisning extends BaseSesjonsvisning
{
  kort?: Kort[];
}
