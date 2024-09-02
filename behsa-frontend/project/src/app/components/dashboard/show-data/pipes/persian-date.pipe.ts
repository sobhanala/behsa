import { Pipe, PipeTransform } from '@angular/core';
import * as jalaali from 'jalaali-js';

@Pipe({
  name: 'persianDate',
  standalone: true
})
export class PersianDatePipe implements PipeTransform {

  transform(value: string): string {
    if (!value) return '';

    const date = new Date(value);
    const jDate = jalaali.toJalaali(date.getFullYear(), date.getMonth() + 1, date.getDate());

    // Format the Persian date as desired (YYYY/MM/DD)
    return `${jDate.jy}/${this.padZero(jDate.jm)}/${this.padZero(jDate.jd)}`;
  }

  // Helper method to pad single digits with a leading zero
  private padZero(value: number): string {
    return value < 10 ? '0' + value : value.toString();
  }
}
