import { SessionType } from "../models/api/SessionType";

export class SessionTypes {
    public static GetSessionTypes() {
        const sessiontypes = [
            { name: 'ProtectiveEquipment', value: SessionType.ProtectiveEquipment },
            { name: 'Five Indications', value: SessionType.FiveIndications },
            { name: 'Gloves', value: SessionType.Gloves },
            { name: 'HandJewelry', value: SessionType.HandJewelry }
          ];
        
        return sessiontypes;
    }
    
}