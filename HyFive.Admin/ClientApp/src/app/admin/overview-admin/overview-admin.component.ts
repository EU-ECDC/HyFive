import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/data/user.service';

@Component({
  selector: 'app-overview-admin',
  templateUrl: './overview-admin.component.html'
})
export class OverviewAdminComponent implements OnInit {

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
  }

}
