import {Component, ElementRef, ViewChild} from '@angular/core';
import User from "../../../interfaces/user";
import {HttpClient} from "@angular/common/http";
import {API_BASE_URL} from "../../../app.config";
import {ModifyUserService} from "../../../services/modify-user/modify-user.service";
import {NgIconComponent, provideIcons} from "@ng-icons/core";
import {heroUserPlus, heroXMark} from "@ng-icons/heroicons/outline";
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {UserService} from "../../../services/user/user.service";
import {Router} from "@angular/router";
import {BlurClickDirective} from "../../../directives/blur-click.directive";

@Component({
  selector: 'app-manage-users',
  standalone: true,
  imports: [NgIconComponent, FormsModule, ReactiveFormsModule, BlurClickDirective],
  templateUrl: './manage-users.component.html',
  styleUrl: './manage-users.component.scss',
  providers: [provideIcons({heroUserPlus, heroXMark})]
})
export class ManageUsersComponent {
  users: User[] | undefined = undefined;
  finalUsers: User[] | undefined = undefined;

  formGroup = new FormGroup({
    firstName: new FormControl('', Validators.required),
    lastName: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    userName: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
    role: new FormControl('Admin', Validators.required),
  })

  @ViewChild('formElement') formElement!: ElementRef<HTMLFormElement>;
  @ViewChild('searchElement') searchElement!: ElementRef<HTMLInputElement>;

  constructor(private modifyService: ModifyUserService, private http: HttpClient, private userService: UserService,
    private router: Router) {}

  ngOnInit(): void {
    const token = this.getToken();
    this.http.get<User[]>(API_BASE_URL + 'users', {headers: {'Authorization': "bearer " + token}})
      .subscribe((response) => {
      this.users = response;
      this.finalUsers = response;
    });
  }

  handleChangeRole(user: User): void {
    const index = this?.finalUsers?.indexOf(user);
    if (index !== -1 && index !== undefined && this.finalUsers !== undefined) {
      const selected = (document.getElementById(this.finalUsers[index].userName!) as HTMLSelectElement).value;
      if (selected === this.finalUsers[index].role!.toLowerCase()) {
        return;
      }
      this.finalUsers[index].role = selected;
      this.modifyService.alterRole(this.finalUsers[index].userName!, selected);
      const user = this.userService.getUser();
      if (user?.userName === this.finalUsers[index].userName!) {
        user.role = selected;
        this.userService.setUser(user);
        this.router.navigateByUrl('dashboard/home');
      }
    }
  }

  getToken(): string | null {
    let token = localStorage.getItem("token");
    if (token) {
      token = token.substring(1, token.length - 1);
    }
    return token;
  }

  handleAdd(): void {
    if (this.formGroup.valid) {
      const data = {
        firstName: this.formGroup.value.firstName,
        lastName: this.formGroup.value.lastName,
        userName: this.formGroup.value.userName,
        email: this.formGroup.value.email,
        password: this.formGroup.value.password,
        role: this.formGroup.value.role,
      }
      const token = this.getToken();
      this.http.post<User>(API_BASE_URL + 'users/signup', data, {headers: {'Authorization': "Bearer " + token}})
        .subscribe((res) => {
          this.finalUsers?.push(res);
          this.handleClose();
        });
    } else {
      console.log(this.formGroup.value);
    }
  }

  handleShowForm(): void {
    this.formElement.nativeElement.style.display = 'flex';
  }

  handleClose(): void {
   this.formElement.nativeElement.style.display = 'none';
  }

  handleSearch() {
    const value = this.searchElement.nativeElement.value;
    this.finalUsers = this.users?.filter(user => user.userName?.includes(value) ||
      user.firstName?.includes(value) || user.lastName?.includes(value) || user.email?.includes(value));
  }
}
