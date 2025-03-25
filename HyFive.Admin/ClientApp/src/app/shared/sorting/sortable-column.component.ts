import { Component, HostListener, Input, OnDestroy, OnInit } from '@angular/core';
import { SortService } from './sort.service';
import { Subscription } from 'rxjs';
import { faCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: '[sortable-column]',
  templateUrl: './sortable-column.component.html'
})
export class SortableColumnComponent implements OnInit, OnDestroy {

  constructor(private sortService: SortService) { }

  @Input("sortable-column") columnName: string = '';
  @Input("sort-direction") sortDirection: string = '';

  private columnSortedSubscription: Subscription | undefined;
  faCircle = faCircle;
  
  @HostListener('click')
  sort() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.sortService.columnSorted({ columnName: this.columnName, sortDirection: this.sortDirection });
  }

  ngOnInit() {
    this.columnSortedSubscription = this.sortService.columnSorted$.subscribe(event => {
      if (this.columnName !== event.columnName) {
        this.sortDirection = '';
      }
    });
  }

  ngOnDestroy() {
    this.columnSortedSubscription?.unsubscribe();
  }

}
