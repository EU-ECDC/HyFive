import {Avdeling} from '../api/Avdeling';

export interface Avdelingsvalg {
    avdeling: Avdeling;
    erValgt: boolean;
    erAlleredePaKlinikk: boolean;
}
