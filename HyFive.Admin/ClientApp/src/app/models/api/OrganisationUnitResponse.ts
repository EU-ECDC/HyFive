import { OrganisationUnit } from "./OrganisationUnit";

export interface OrganisationUnitResponse extends OrganisationUnit {
    facilityId: number;
}