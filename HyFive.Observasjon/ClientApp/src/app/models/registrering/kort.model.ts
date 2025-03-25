import { Rolle } from '../api/Rolle';

export interface Kort
{
  id: string;
  rolle: Rolle;
  erAktivt: boolean
}
