import { Component, OnInit, OnDestroy } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { HandJewelryType } from '../../../models/api/HandJewelryType';
import { HandJewelryTypeService } from '../../../services/data/handJewelryType.service';
import { KeyEventService } from '../../../services/events/key-event.service';

@Component({
  selector: 'app-editing-of-handjewelry-type',
  templateUrl: './editing-of-handjewelry-type.component.html'
})
export class EditingByHandjewelryTypeComponent implements OnInit, OnDestroy {

  handJewelryTypes: HandJewelryType[] = [];
  handJewelryTypeAsChanged: HandJewelryType = null;

  constructor(
    private handJewelryTypeService: HandJewelryTypeService,
    private toastrService: ToastrService,
    private keyEventService: KeyEventService
  ) { }

  ngOnInit(): void {
    this.keyEventService.escapeKeyEvent.subscribe((event: KeyboardEvent) => {
      this.cancelEdit();
    });

    this.loadHandJewelryTypes();
  }

  ngOnDestroy(): void {
    this.toastrService.clear();
  }


  loadHandJewelryTypes() {
    this.handJewelryTypeService.getHandJewelryTypes().subscribe(
      (result) => this.handJewelryTypes = result,
      (error) => this.toastrService.error('An error occurred while loading Hand Jewelry Types: ' + error?.message, '', { disableTimeOut: true}),
    );
  }

  selectedHandJewelryType(handjewelryType: HandJewelryType): void {
    if (this.handJewelryTypeAsChanged?.id == handjewelryType.id) return;
    this.handJewelryTypeAsChanged = JSON.parse(JSON.stringify(handjewelryType));
  }

  updateHandJewelryType(handjewelryType: HandJewelryType): void {
    this.handJewelryTypeService.updateHandJewelryType(handjewelryType).subscribe(
      (updateHandJewelryType) => {
        this.toastrService.success("HandJewelryType updated");
        this.loadHandJewelryTypes();
      },
      error => this.toastrService.error('An error occurred while updating HandJewelryType: ' + error?.error, '', { disableTimeOut: true}),
      () => this.handJewelryTypeAsChanged = null
    );
  }

  cancelEdit($event: Event = null) {
    if($event){
      $event.stopPropagation();
      $event.preventDefault();
    }
    this.handJewelryTypeAsChanged = null;
  }
}
