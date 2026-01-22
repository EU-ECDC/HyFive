import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/data/user.service';

@Component({
  selector: 'app-overview-admin',
  templateUrl: './overview-admin.component.html'
})
export class OverviewAdminComponent {

  constructor(
    private readonly userService: UserService,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) { }

}
