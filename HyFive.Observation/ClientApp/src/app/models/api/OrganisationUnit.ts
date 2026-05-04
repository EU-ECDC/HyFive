import { Address } from "./Address";
import { OrganisationUnitLevel } from "./OrganisationUnitLevel";
import { OrganisationUnitType } from "./OrganisationUnitType";
import { Role } from "./Role";

export interface OrganisationUnit {
    id: number;
    parentId?: number;
    name: string;
    abbreviation: string;
    description: string;
    levelRef: OrganisationUnitLevel;
    type: OrganisationUnitType;
    address?: Address;
    children?: OrganisationUnit[];
    roles: Role[];
    hasObservations: boolean;
}