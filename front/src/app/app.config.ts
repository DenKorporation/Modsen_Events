import {ApplicationConfig, provideZoneChangeDetection} from '@angular/core';
import {provideRouter} from '@angular/router';

import {routes} from './app.routes';
import {provideAnimationsAsync} from '@angular/platform-browser/animations/async';
import {provideHttpClient, withInterceptors} from "@angular/common/http";
import {provideOAuthClient} from "angular-oauth2-oidc";
import {authInterceptor} from "./interceptors/auth.interceptor";
import {invalidTokenInterceptor} from "./interceptors/invalid-token.interceptor";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({eventCoalescing: true}),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, invalidTokenInterceptor])),
    provideOAuthClient(),
    provideAnimationsAsync()
  ]
};
