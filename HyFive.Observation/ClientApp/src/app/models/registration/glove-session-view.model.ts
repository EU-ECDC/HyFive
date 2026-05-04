import { Card } from './card.model';
import {BaseSessionView} from './base-sessionView.model';

export interface GloveSessionView extends BaseSessionView
{
  card?: Card[];
  gloveUseMustBeRegistered: boolean;
}
