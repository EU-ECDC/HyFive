import { Address } from "./Address";

export interface UpdateFacilityRequest {
  id: number;
  name: string;
  abbreviation: string;
  description: string;
  organisationUnitTypeId: number;
  address: Address;
}