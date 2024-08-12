import {Component, EventEmitter, inject, Input, Output} from '@angular/core';
import {CommonModule, NgIf} from "@angular/common";
import {MatCardModule} from "@angular/material/card";
import {UserResponse} from "../../../dtos/user/user-response";
import {UserInfo} from "../../../dtos/user/user-info";
import {Role} from "../../../enums/role";
import {EventUserResponse} from "../../../dtos/event/event-user-response";
import {MatButtonModule} from "@angular/material/button";
import {ErrorDialogComponent} from "../error-dialog/error-dialog.component";
import {UserService} from "../../../services/user.service";
import {MatDialog} from "@angular/material/dialog";
import {AuthService} from "../../../services/auth.service";
import {UpdateRoleDialogComponent} from "../update-role-dialog/update-role-dialog.component";

@Component({
  selector: 'app-profile-card',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    NgIf,
  ],
  templateUrl: './profile-card.component.html',
  styleUrl: './profile-card.component.css'
})
export class ProfileCardComponent {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private errorDialog = inject(MatDialog);
  private dialog = inject(MatDialog);

  @Input() userResponse: UserResponse | null = null;
  @Input() isAdminPage: boolean = false;
  @Input() isEventPage: boolean = false;
  @Input() eventId: string | null = null;

  @Output() updateData = new EventEmitter();

  userId: string | null = null;

  constructor() {
    this.authService.userInfo$.subscribe(value => this.userId = value?.id ?? null)
  }

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

  async deleteUser() {
    await this.userService.deleteUser(this.userResponse?.id!).then(() => {
      this.updateData.emit();
    }).catch((error) => {
      this.errorDialog.open(ErrorDialogComponent, {
        data: error.message
      });
    });
  }

  changeRole() {
    const dialogRef = this.dialog.open(UpdateRoleDialogComponent, {
      data: this.userResponse
    });

    dialogRef.afterClosed().subscribe(() => {
      this.updateData.emit();
    });
  }

  isOwnCard() {
    return this.userResponse?.id === this.userId
  }

  unregister() {
    this.userService.unregisterFromEvent(this.userResponse!.id, this.eventId!).then(() => {
      this.updateData.emit();
    }).catch((error) => {
      this.errorDialog.open(ErrorDialogComponent, {
        data: error.message
      });
    });
  }
}
