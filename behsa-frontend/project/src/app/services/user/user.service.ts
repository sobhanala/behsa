import {Inject, Injectable, PLATFORM_ID} from '@angular/core';
import User from "../../interfaces/user";
import {HttpClient} from "@angular/common/http";
import {API_BASE_URL} from "../../app.config";
import {Router} from "@angular/router";
import {isPlatformBrowser} from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private user: User | undefined = undefined;
  constructor(private http: HttpClient, private router: Router, @Inject(PLATFORM_ID) private platform: object) {
    if (isPlatformBrowser(this.platform)) {
      const u = localStorage.getItem('userData');
      if (u) {
        this.user = JSON.parse(u);
      } else {
        this.user = undefined;
      }
    }
  }

  getUser(): User | undefined {
    return this.user;
  }

  setUser(user: User): void {
    this.user = user;
    localStorage.setItem('userData', JSON.stringify(user));
  }

  login(user: {identifier: string, password: string}): void {
    const obj = {
      email: user.identifier.includes('@') ? user.identifier : null,
      username: user.identifier.includes('@') ? null : user.identifier,
      password: user.password,
    }
    this.http.post(`${API_BASE_URL}users/login`, obj).subscribe((res: any) => {
      this.user = {};
      this.user.userName = res?.username;
      localStorage.setItem('token', JSON.stringify(res?.token));
      const userData = {
        firstName: res.firstName,
        lastName: res.lastName,
        email: res.email,
        userName: res.userName,
        role: res.role,
      };
      this.user = userData;
      localStorage.setItem("userData", JSON.stringify(userData));
      this.router.navigate(['dashboard/profile']);
    });
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('userData');
    this.user = undefined;
    this.router.navigate(['login']);
  }
}
