import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'rialPipe',
  standalone: true
})
export class RialPipePipe implements PipeTransform {

  transform(value: number): string {
    value /= 10;
    let result = String(value);
    let cntr = result.length - 3;
    while (cntr > 0) {
      result = result.substring(0, cntr) + ',' + result.substring(cntr, result.length);
      cntr -= 3;
    }
    return result + ' تومان';
  }

}
