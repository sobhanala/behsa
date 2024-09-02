import {ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot} from '@angular/router';
import {inject} from "@angular/core";
import {UserService} from "../../services/user/user.service";

export const loggedInGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const userService = inject(UserService);
  const router = inject(Router);
  const user = userService.getUser();
  if (!user && !route.data['requiresAuth'])
    return true;
  else if (user && !route.data['requiresAuth'])
    return false;
  else if (!user) {
    router.navigate(['login']);
    return false;
  } else if (user && state.url === '/dashboard') {
    router.navigate(['dashboard/home']);
    return true;
  }
  return true;
};
