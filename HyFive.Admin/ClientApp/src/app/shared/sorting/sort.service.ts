import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SortService {

  private columnSortedSource = new Subject<IColumnSortedEvent>();

  columnSorted$ = this.columnSortedSource.asObservable();

  columnSorted(event: IColumnSortedEvent) {
    this.columnSortedSource.next(event);
  }
}


export interface IColumnSortedEvent {
  columnName: string;
  sortDirection: string;
}