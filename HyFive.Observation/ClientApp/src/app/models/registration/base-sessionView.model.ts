import { OrganisationUnit } from "../api/OrganisationUnit";

export interface BaseSessionView
{
  sessionId: string;
  department: OrganisationUnit;
  unit?: OrganisationUnit;
}
