import { OrganisationUnit } from "./OrganisationUnit";

export interface UnitResponse
{
    id: number; 
    facilityId: number;
    name: string;
    departments: OrganisationUnit[];
} 