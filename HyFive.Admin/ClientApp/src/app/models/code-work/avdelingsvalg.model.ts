import {Department} from '../api/Department';

export interface Avdelingsvalg {
    avdeling: Department;
    erValgt: boolean;
    erAlleredePaKlinikk: boolean;
}
