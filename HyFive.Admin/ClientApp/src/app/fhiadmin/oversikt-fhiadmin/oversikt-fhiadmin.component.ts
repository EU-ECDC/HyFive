import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BrukerService } from '../../services/data/bruker.service';

@Component({
  selector: 'app-oversikt-fhiadmin',
  templateUrl: './oversikt-fhiadmin.component.html'
})
export class OversiktFhiAdminComponent implements OnInit {

  constructor(
    private brukerService: BrukerService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
  }

}
