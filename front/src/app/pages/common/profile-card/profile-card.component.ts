import {Component, Input} from '@angular/core';
import {CommonModule} from "@angular/common";
import {MatCardModule} from "@angular/material/card";
import {UserResponse} from "../../../dtos/user/user-response";
import {UserInfo} from "../../../dtos/user/user-info";
import {Role} from "../../../enums/role";
import {EventUserResponse} from "../../../dtos/event/event-user-response";

@Component({
  selector: 'app-profile-card',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  templateUrl: './profile-card.component.html',
  styleUrl: './profile-card.component.css'
})
export class ProfileCardComponent {
  @Input() userResponse: UserResponse | null = null;

  getRole(): Role | null {
    if (this.hasRole()) {
      return (this.userResponse as UserInfo).role;
    }
    return null;
  }

  getRegistrationDate(): string | null {
    if (this.hasRegistrationDate()) {
      return (this.userResponse as EventUserResponse).registrationDate;
    }
    return null;
  }

  hasRole() {
    return this.userResponse !== null && this.userResponse !== undefined && 'role' in this.userResponse;
  }

  hasRegistrationDate() {
    return this.userResponse !== null && this.userResponse !== undefined && 'registrationDate' in this.userResponse;
  }
}
