declare module 'jalaali-js' {
  export interface JalaaliDate {
    jy: number; // Jalali year
    jm: number; // Jalali month
    jd: number; // Jalali day
  }

  export function toJalaali(
    gy: number,
    gm: number,
    gd: number
  ): JalaaliDate;

  export function toGregorian(
    jy: number,
    jm: number,
    jd: number
  ): { gy: number; gm: number; gd: number };
}
