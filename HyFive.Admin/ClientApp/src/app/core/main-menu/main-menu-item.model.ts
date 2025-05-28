import { AuthorizedRole } from '../../_common/authorization/authorized-role';

export interface MainMenuItem {
  name: string;
  routerLink: string;
  roles: AuthorizedRole[];
}
