import { IColumnSortedEvent } from "../shared/sorting/sort.service";

// utils/sort-helper.ts
export interface SortConfig<T> {
  [key: string]: (item: T) => any;
}

export class SortHelper {
  static sort<T, E extends { columnName: string; sortDirection: 'asc' | 'desc' }>(
    items: T[],
    event: IColumnSortedEvent,
    config: SortConfig<T>
  ): T[] {
    const propertyGetter = config[event.columnName];
    
    if (!propertyGetter) {
      throw new Error(`Invalid sort column: ${event.columnName}`);
    }

    const sortOrder = event.sortDirection === 'asc' ? 1 : -1;

    const sortFunc = (a: T, b: T) => {
      const valueA = propertyGetter(a);
      const valueB = propertyGetter(b);
      
      let result = 0;
      if (valueA > valueB) result = 1;
      if (valueA < valueB) result = -1;
      
      return result * sortOrder;
    };

    return items.toSorted(sortFunc);
  }
}