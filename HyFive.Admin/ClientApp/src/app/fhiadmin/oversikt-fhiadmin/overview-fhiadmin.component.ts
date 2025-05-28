import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/data/user.service';

@Component({
  selector: 'app-overview-fhiadmin',
  templateUrl: './overview-fhiadmin.component.html'
})
export class OverviewFhiAdminComponent implements OnInit {

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
  }

}
