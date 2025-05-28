import { Role } from '../api/Role';

export interface Card
{
  id: string;
  role: Role;
  isActive: boolean
}
