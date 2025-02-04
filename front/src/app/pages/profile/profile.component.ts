import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {UserInfo} from "../../dtos/user/user-info";
import {Subscription} from "rxjs";
import {AuthService} from "../../services/auth.service";
import {CommonModule} from "@angular/common";
import {ProfileCardComponent} from "../common/profile-card/profile-card.component";

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ProfileCardComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnDestroy, OnInit {
  public userInfo!: UserInfo | null;
  private userInfoSubscription!: Subscription;

  private authService = inject(AuthService);


  ngOnInit(): void {
    this.authService.initConfiguration().then(() => {
      this.userInfoSubscription = this.authService.userInfo$.subscribe(info => {
        this.userInfo = info;
      });
    });
  }

  ngOnDestroy(): void {
    if (this.userInfoSubscription) {
      this.userInfoSubscription.unsubscribe();
    }
  }
}
