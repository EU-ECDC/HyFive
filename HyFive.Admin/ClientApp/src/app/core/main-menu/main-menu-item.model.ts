import { AuthorizedRole } from '../../_felles/authorization/authorized-role';

export interface MainMenuItem {
  name: string;
  routerLink: string;
  roles: AuthorizedRole[];
}
