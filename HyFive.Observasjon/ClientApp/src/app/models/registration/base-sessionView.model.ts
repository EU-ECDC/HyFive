import {Department} from '../api/Department';

export interface BaseSessionView
{
  sessionId: string;
  department: Department;
}
