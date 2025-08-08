import { SessionType } from "../models/api/SessionType";

export class SessionTypes {
    public static GetSessionTypes() {
        const sessiontypes = [
            { name: 'Protective Equipment', value: SessionType.ProtectiveEquipment },
            { name: 'Five Indications', value: SessionType.FiveIndications },
            { name: 'Gloves', value: SessionType.Gloves },
            { name: 'Hand Jewelry', value: SessionType.HandJewelry }
          ];
        
        return sessiontypes;
    }
    
}