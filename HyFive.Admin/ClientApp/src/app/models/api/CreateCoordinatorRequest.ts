export interface CreateCoordinatorRequest {
  id: number,
  facilityId: number,
  firstName: string,
  lastName: string,
  email: string,
  identityPseudonym: string,
  isDeactivated: boolean
}