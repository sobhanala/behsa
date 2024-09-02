import {CanActivateFn, Router} from '@angular/router';
import {inject} from "@angular/core";
import {UserService} from "../../services/user/user.service";

export const isAdminGuard: CanActivateFn = (route, state) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const user = userService.getUser();
  if (!user || user.role?.toLowerCase() !== "admin") {
    router.navigateByUrl('dashboard/home')
    return false;
  }
  return true;
};
