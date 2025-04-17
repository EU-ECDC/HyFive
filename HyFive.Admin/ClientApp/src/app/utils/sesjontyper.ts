import { SessionType } from "../models/api/SessionType";

export class Sesjonstyper {
    public static HentSesjonstyper() {
        const sesjontyper = [
            { name: 'Beskyttelsesutstyr', verdi: SessionType.Beskyttelsesutstyr },
            { name: 'Fire indikasjoner', verdi: SessionType.FireIndikasjoner },
            { name: 'Hansker', verdi: SessionType.Hansker },
            { name: 'Håndsmykker', verdi: SessionType.Handsmykker }
          ];
        
        return sesjontyper;
    }
    
}