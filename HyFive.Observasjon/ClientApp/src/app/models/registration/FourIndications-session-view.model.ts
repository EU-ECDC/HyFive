import { Role } from '../api/Role';
import { Card } from './card.model';
import {Department} from '../api/Department';
import {BaseSessionView} from './base-sessionView.model';

export interface FourIndicationsSessionView extends BaseSessionView
{
  card?: Card[];
  gloveUseMustBeRegistered: boolean;
  timeShouldBeRegistred: boolean;
}
