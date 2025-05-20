import { Card } from './card.model';
import {BaseSessionView} from './base-sessionView.model';

export interface HandJewelrySessionView extends BaseSessionView
{
  card?: Card[];
}
