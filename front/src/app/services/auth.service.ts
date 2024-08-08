import {inject, Injectable, OnDestroy} from '@angular/core';
import {AuthConfig, OAuthInfoEvent, OAuthService} from "angular-oauth2-oidc";
import {Router} from "@angular/router";
import {environment} from "../../environments/environment";
import {BehaviorSubject, Subscription} from "rxjs";
import {UserInfo} from "../dtos/user/user-info";
import {UserService} from "./user.service";
import {CreateUser} from "../dtos/user/create-user";

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnDestroy {
  private oAuthService = inject(OAuthService);
  private userService = inject(UserService);

  private userSubject = new BehaviorSubject<UserInfo | null>(null);
  private isAuthorizedSubject = new BehaviorSubject<boolean>(false);

  userInfo$ = this.userSubject.asObservable();
  isAuthorized$ = this.isAuthorizedSubject.asObservable();

  private authSubscription!: Subscription;

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  async initConfiguration(): Promise<void> {
    const authConfig: AuthConfig = {
      issuer: environment.identityUrl,
      strictDiscoveryDocumentValidation: false,
      clientId: environment.clientId,
      scope: environment.scope,
      oidc: false,
      logoutUrl: undefined,
      fallbackAccessTokenExpirationTimeInSec: 3600,
      showDebugInformation: true
    }

    this.authSubscription = this.oAuthService.events.subscribe(async value => {
      if (value.type === "silently_refreshed" || value.type === "token_refreshed") {
        await this.oAuthService.loadUserProfile();
      }
      if (value.type === 'token_expires' && (value as OAuthInfoEvent).info === 'access_token') {
        await this.refreshToken();
      }
      this.updateAuthStatus();
    });

    this.oAuthService.configure(authConfig);
    this.oAuthService.setStorage(sessionStorage);
    await this.oAuthService.loadDiscoveryDocument();
    this.oAuthService.logoutUrl = '';
  }

  async login(userName: string, password: string) {
    await this.oAuthService.fetchTokenUsingPasswordFlowAndLoadUserProfile(userName, password);
    this.updateAuthStatus();
  }

  async register(user: CreateUser) {
    await this.userService.createUser(user);
    await this.login(user.email, user.password);
  }

  async logout() {
    await this.oAuthService.revokeTokenAndLogout();
    this.oAuthService.logOut();
    this.updateAuthStatus();
  }

  async refreshToken() {
    await this.oAuthService.refreshToken();
    this.updateAuthStatus();
  };

  private updateAuthStatus() {
    const loggedIn = this.oAuthService.hasValidAccessToken();
    this.isAuthorizedSubject.next(loggedIn);

    if (loggedIn) {
      this.userSubject.next(this.identityClaimsToUserInfo(this.oAuthService.getIdentityClaims()));
    } else {
      this.userSubject.next(null);
    }
  }

  get profile() {
    return this.oAuthService.getIdentityClaims();
  }

  get token() {
    return this.oAuthService.getAccessToken();
  }

  get isAuthorized() {
    return this.isAuthorizedSubject.getValue();
  }

  get role() {
    return this.userSubject.getValue()?.role;
  }

  identityClaimsToUserInfo(claims: Record<string, any>): UserInfo | null {
    try {
      return {
        id: claims['sub'],
        firstName: claims['given_name'],
        lastName: claims['family_name'],
        email: claims['email'],
        role: claims['role'],
        birthdate: claims['birthdate']
      };
    } catch (error) {
      return null;
    }
  }
}
