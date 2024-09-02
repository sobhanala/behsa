import {Inject, Injectable, PLATFORM_ID} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {isPlatformBrowser} from "@angular/common";
import {API_BASE_URL} from "../../app.config";
import {firstValueFrom} from "rxjs";
import {AccountTransaction, Transaction} from "../../interfaces/other";

@Injectable({
  providedIn: 'root'
})
export class FetchDataService {

  constructor(private http: HttpClient, @Inject(PLATFORM_ID) private platform: object) { }

  fetchData(): Promise<Transaction[]> {
    const token = this.getToken();
    return firstValueFrom(this.http.get<Transaction[]>(API_BASE_URL + 'transactions', {headers: {'Authorization': "Bearer " + token}}))
  }

  fetchDataById(accountId: string): Promise<AccountTransaction[]> {
    const token = this.getToken();
    return firstValueFrom(this.http.get<AccountTransaction[]>(API_BASE_URL + 'transactions/by-account/' + accountId, {headers: {'Authorization': "Bearer " + token}}))
  }

  getToken(): string | null {
    if(isPlatformBrowser(this.platform)) {
      let token = localStorage.getItem("token");
      if (token) {
        token = token.substring(1, token.length - 1);
      }

      return token;
    }
    return null;
  }
}
