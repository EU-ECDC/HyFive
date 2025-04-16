import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from '../../services/data/user.service';

@Component({
  selector: 'app-oversikt-fhiadmin',
  templateUrl: './oversikt-fhiadmin.component.html'
})
export class OversiktFhiAdminComponent implements OnInit {

  constructor(
    private userService: UserService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
  }

}
