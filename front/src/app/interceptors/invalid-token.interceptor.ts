import {HttpEventType, HttpInterceptorFn} from '@angular/common/http';
import {tap} from "rxjs";
import {inject} from "@angular/core";
import {AuthService} from "../services/auth.service";

export const invalidTokenInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    tap(event => {
      if (event.type === HttpEventType.Response) {
        if (event.status === 498) { // invalid token (unofficial status code)
          let authService = inject(AuthService);
          authService.refreshToken();
        }
      }
    })
  );
};
