import { Bruker } from "../models/api/Bruker";

export class SokHjelper {
    public static filtrerBrukere(sokeord: string, brukerListe: Bruker[]) : Bruker[] {
        
            var filtrerteBrukere = brukerListe.filter(k => 
                                        k.fornavn?.toLowerCase().includes(sokeord.toLowerCase()) || 
                                        k.etternavn?.toLocaleLowerCase().includes(sokeord.toLowerCase()) ||
                                        (k.fornavn + ' ' + k.etternavn).toLowerCase().includes(sokeord.toLowerCase()) ||
                                        k.hprNummer?.includes(sokeord));

            return filtrerteBrukere;
    }
}