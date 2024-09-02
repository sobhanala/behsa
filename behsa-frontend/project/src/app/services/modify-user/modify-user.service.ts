import { Injectable } from '@angular/core';
import {UserService} from "../user/user.service";
import {HttpClient} from "@angular/common/http";
import {API_BASE_URL} from "../../app.config";

interface ChangeData {
  firstName: string,
  lastName: string,
  userName: string
}

interface ChangePassword {
  currentPassword: string | null | undefined,
  newPassword: string | null | undefined,
}

@Injectable({
  providedIn: 'root'
})
export class ModifyUserService {

  constructor(private userService: UserService, private http: HttpClient) {}

  getToken(): string | null {
    let token = localStorage.getItem('token');
    if (token) {
      token = token.substring(1, token.length - 1);
    }
    return token;
  }

  modifyUser(data: ChangeData): void {
    const token = this.getToken();
    this.http.put<ChangeData>(API_BASE_URL + 'profile/edit-info', data, {headers: {'Authorization': "Bearer " + token}})
      .subscribe(() => {
      const user = {
        firstName: data.firstName,
        lastName: data.lastName,
        userName: data.userName,
        ...this.userService.getUser()
      }
      this.userService.setUser(user);
    });
  }

  changePassword(data: ChangePassword): void {
    const token = this.getToken();
    this.http.put(API_BASE_URL + 'profile/change-password', data, {headers: {'Authorization': "Bearer " + token}})
      .subscribe(response => {
        console.log(response);
      });
  }

  alterRole(userName: string, role: string): void {
    const token = this.getToken();
    const data = {
      userName,
      role
    }

    this.http.patch(API_BASE_URL + 'users/change-role', data, {headers: {'Authorization': "Bearer " + token}})
      .subscribe((response) => {
        console.log(response);
      })
  }
}
