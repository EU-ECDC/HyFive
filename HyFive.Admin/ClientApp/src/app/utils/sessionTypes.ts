import { SessionType } from "../models/api/SessionType";

export class SessionTypes {
    public static GetSessionTypes() {
        const sessiontypes = [
            { name: 'ProtectiveEquipment', value: SessionType.ProtectiveEquipment },
            { name: 'Four Indications', value: SessionType.FourIndications },
            { name: 'Gloves', value: SessionType.Gloves },
            { name: 'Handjewelry', value: SessionType.Handjewelry }
          ];
        
        return sessiontypes;
    }
    
}