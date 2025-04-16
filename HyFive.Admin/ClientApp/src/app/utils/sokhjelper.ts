import { User } from "../models/api/User";

export class SokHjelper {
    public static filtrerBrukere(sokeord: string, brukerListe: User[]) : User[] {
        
            var filtrerteBrukere = brukerListe.filter(k => 
                                        k.fornavn?.toLowerCase().includes(sokeord.toLowerCase()) || 
                                        k.etternavn?.toLocaleLowerCase().includes(sokeord.toLowerCase()) ||
                                        (k.fornavn + ' ' + k.etternavn).toLowerCase().includes(sokeord.toLowerCase()) ||
                                        k.hprNummer?.includes(sokeord));

            return filtrerteBrukere;
    }
}