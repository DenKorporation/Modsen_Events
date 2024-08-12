import {CanActivateFn, Router} from '@angular/router';
import {AuthService} from "../services/auth.service";
import {inject} from "@angular/core";
import {Role} from "../enums/role";

export const authGuard: CanActivateFn = (route, state) => {
  let authService = inject(AuthService);
  let router = inject(Router);

  let stateUrl = state.url;

  if (authService.isAuthorized) {
    if (stateUrl === '/login' || stateUrl === '/sign-up') {
      router.navigate(['/events']);
      return false;
    }
    if ((stateUrl === '/create-event' || stateUrl === '/users') && authService.role !== Role.Administrator) {
      router.navigate(['/events']);
      return false;
    }
    return true;
  } else {
    if (stateUrl === '/login' || stateUrl === '/sign-up') {
      return true;
    } else {
      router.navigate(['/login']);
      return false;
    }
  }
};
