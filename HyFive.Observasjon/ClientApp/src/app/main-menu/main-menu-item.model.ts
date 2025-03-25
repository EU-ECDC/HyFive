import {IconProp} from '@fortawesome/fontawesome-svg-core';
export interface MainMenuItem {
  name: string;
  routerLink: string;
  exactRouteMatch: boolean;
  icon: IconProp;
}
