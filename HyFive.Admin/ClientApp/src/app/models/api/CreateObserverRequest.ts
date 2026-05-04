export interface CreateObserverRequest {
  id: number,
  facilityId: number,
  firstName: string,
  lastName: string,
  email: string,
  identityPseudonym: string,
  isDeactivated: boolean 
}