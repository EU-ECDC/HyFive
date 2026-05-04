import { Card } from './card.model';
import {BaseSessionView} from './base-sessionView.model';

export interface HandHygieneSessionView extends BaseSessionView
{
  card?: Card[];
  gloveUseMustBeRegistered: boolean;
  timeShouldBeRegistred: boolean;
}
