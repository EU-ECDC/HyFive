import { SesjonType } from "../models/api/SesjonType";

export class Sesjonstyper {
    public static HentSesjonstyper() {
        const sesjontyper = [
            { navn: 'Beskyttelsesutstyr', verdi: SesjonType.Beskyttelsesutstyr },
            { navn: 'Fire indikasjoner', verdi: SesjonType.FireIndikasjoner },
            { navn: 'Hansker', verdi: SesjonType.Hansker },
            { navn: 'Håndsmykker', verdi: SesjonType.Handsmykker }
          ];
        
        return sesjontyper;
    }
    
}