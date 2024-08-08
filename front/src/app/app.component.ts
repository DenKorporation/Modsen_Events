import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {Router, RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {CommonModule, NgFor, NgIf} from "@angular/common";
import {MatToolbarModule} from "@angular/material/toolbar";
import {MatButton} from "@angular/material/button";
import {UserInfo} from "./dtos/user/user-info";
import {Subscription} from "rxjs";
import {AuthService} from "./services/auth.service";
import {MatIcon, MatIconModule} from "@angular/material/icon";
import {MatMenuModule} from "@angular/material/menu";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    MatToolbarModule,
    MatButton,
    MatIconModule,
    MatMenuModule,
    RouterLink,
    RouterLinkActive,
    NgIf,
    NgFor
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit, OnDestroy {
  public userInfo!: UserInfo | null;
  public isAuthorized: boolean = false;
  private userInfoSubscription!: Subscription;
  private isAuthSubscription!: Subscription;

  private authService = inject(AuthService);
  private router = inject(Router);


  ngOnInit(): void {
    this.authService.initConfiguration().then(() => {
      this.userInfoSubscription = this.authService.userInfo$.subscribe(info => {
        this.userInfo = info;
      });

      this.isAuthSubscription = this.authService.isAuthorized$.subscribe(isAuthorized => {
        this.isAuthorized = isAuthorized;
        this.navigate();
      })
    });
  }

  ngOnDestroy(): void {
    if (this.userInfoSubscription) {
      this.isAuthSubscription.unsubscribe();
    }
    if (this.userInfoSubscription) {
      this.isAuthSubscription.unsubscribe();
    }
  }

  logout() {
    this.authService.logout().then();
  }

  navigate() {
    let isSignInUpPage = this.router.url === '/login' || this.router.url === '/sign-up';
    if (isSignInUpPage && this.isAuthorized) {
      this.router.navigate(['/events']);
    }
    if (!isSignInUpPage && !this.isAuthorized) {
      this.router.navigate(['/login']);
    }
  }

  toEventsPage() {
    this.router.navigate(['/events']);
  }

  toProfilePage() {
    this.router.navigate(['/profile']);
  }

  toUserEvents() {
    this.router.navigate(['/my-events'])
  }
}
