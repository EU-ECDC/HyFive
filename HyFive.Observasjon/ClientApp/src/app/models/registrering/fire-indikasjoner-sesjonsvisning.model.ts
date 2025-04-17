import { Role } from '../api/Role';
import { Kort } from './kort.model';
import {Department} from '../api/Department';
import {BaseSesjonsvisning} from './base-sesjonsvisning.model';

export interface FireIndikasjonerSesjonsvisning extends BaseSesjonsvisning
{
  kort?: Kort[];
  hanskebrukSkalRegistreres: boolean;
  tidtakingSkalRegistreres: boolean;
}
