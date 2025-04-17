import { SessionType } from "../models/api/SessionType";

export class Sesjonstyper {
    public static HentSesjonstyper() {
        const sesjontyper = [
            { navn: 'Beskyttelsesutstyr', verdi: SessionType.Beskyttelsesutstyr },
            { navn: 'Fire indikasjoner', verdi: SessionType.FireIndikasjoner },
            { navn: 'Hansker', verdi: SessionType.Hansker },
            { navn: 'Håndsmykker', verdi: SessionType.Handsmykker }
          ];
        
        return sesjontyper;
    }
    
}