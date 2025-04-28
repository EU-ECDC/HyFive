import { Component, OnInit, OnDestroy } from '@angular/core';
import { RegionService } from '../../../services/data/region.service';
import { ToastrService } from 'ngx-toastr';
import { Region } from 'src/app/models/api/Region';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-region',
  templateUrl: './editing-region.component.html'
})
export class EditingRegionComponent implements OnInit, OnDestroy {

  regions: Region[] = [];
  regionAsChanged: Region = null;
  newRegion: Region = this.emptyRequest();

  constructor(
    private regionService: RegionService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadRegions();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }
  
  loadRegions() {
    this.regionService.getRegions().subscribe(
      (result) => this.regions = result,
      (error) => this.toastrService.error('An error occurred while loading regions: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  emptyRequest(): Region {
    return {
      id: 0,
      code: null,
      name: null
    };
  }

  createRegion(): void {
    this.regionService.createRegion(this.newRegion).subscribe(
      (region) => this.toastrService.success(`Region created.`),
      error => this.toastrService.error(`An error occurred while creating region ${this.newRegion.name}. Error: "${error.error}"`, '', { disableTimeOut: true}),
      () => { this.newRegion = this.emptyRequest(); this.loadRegions(); }
    );
  }

  selectedRegion(region: Region): void {
    if (this.regionAsChanged?.id == region.id) return;
    this.regionAsChanged = JSON.parse(JSON.stringify(region));
  }

  updateRegion(region: Region): void {
    this.regionService.updateRegion(region).subscribe(
      (updateRegion) => {
        this.toastrService.success(`Region ${region.id} was updated`);
        this.loadRegions();
      },
      error => this.toastrService.error('An error occurred while updating region: ' + error?.error, '', { disableTimeOut: true}),
      () => this.regionAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.regionAsChanged = null;
  }
}
