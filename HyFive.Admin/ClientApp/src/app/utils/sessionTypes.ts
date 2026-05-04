import { SessionType } from "../models/api/SessionType";

export class SessionTypes {
    public static GetSessionTypes() {
        const sessiontypes = [
            // { name: 'Protective Equipment', value: SessionType.ProtectiveEquipment },
            { name: 'Hand Hygiene', value: SessionType.FiveIndications },
            { name: 'Gloves', value: SessionType.Gloves },
            { name: 'Bare Below Elbows', value: SessionType.HandJewelry }
          ];
        
        return sessiontypes;
    }
    
}