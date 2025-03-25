import { Injectable } from '@angular/core';
import { NgbDateAdapter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

@Injectable({
  providedIn: 'root'
})
export class DatepickerDateAdapterService extends NgbDateAdapter<string> {

   readonly DELIMITER = '.';

  fromModel(value: any): NgbDateStruct {
    let result: NgbDateStruct = null;
    if (value) {
      if(value.length===10)
      {
        const date = value.split(this.DELIMITER);
        result = {
          day: parseInt(date[0], 10),
          month: parseInt(date[1], 10),
          year: parseInt(date[2], 10)
        };
      }
      else if(value.day > 0) {
        result = {
          day: parseInt(value.day,10),
          month: parseInt(value.month, 10),
          year: parseInt(value.year, 10)
        };
      }
      else {
        result = {
          day: parseInt(value.getDate(),10),
          month: parseInt(value.getMonth(), 10),
          year: parseInt(value.getFullYear(), 10)
        };
      }
    }
    return result;
  }

  toModel(date: NgbDateStruct): string {
    let result: string = null;
    if (date) {
      const day = date.day.toString().padStart(2, '0');
      const month = date.month.toString().padStart(2, '0');
      result = day + this.DELIMITER + month + this.DELIMITER + date.year;
    }
    return result;
  }

}
