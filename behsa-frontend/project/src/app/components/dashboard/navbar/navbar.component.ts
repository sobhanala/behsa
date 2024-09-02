import { Component } from '@angular/core';
import {RouterLink, RouterLinkActive} from "@angular/router";
import {UserService} from "../../../services/user/user.service";
import User from "../../../interfaces/user";
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { heroArrowLeft, heroChartBar, heroHome, heroUserCircle, heroUserGroup } from '@ng-icons/heroicons/outline';
import { heroChartBarSolid, heroHomeSolid, heroUserCircleSolid, heroUserGroupSolid } from '@ng-icons/heroicons/solid';
import { Location } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    RouterLink,
    RouterLinkActive,
    NgIconComponent
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
  providers: [provideIcons({heroHome,heroHomeSolid,heroArrowLeft,heroUserGroup,heroUserGroupSolid,heroUserCircle,heroUserCircleSolid,heroChartBar,heroChartBarSolid})]

})
export class NavbarComponent {
  user!: User | undefined;

  constructor(private userService: UserService ,private location: Location) {
    this.user = this.userService.getUser();
  }

  handleLogout() {
    this.userService.logout();
  }
  isActive(route: string): boolean {
    return this.location.path() ===  '/dashboard/' + route;
  }
}
