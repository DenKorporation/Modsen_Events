import {Component, EventEmitter, inject, Input, Output} from '@angular/core';
import {EventResponse} from "../../../dtos/event/event-response";
import {MatCardModule} from "@angular/material/card";
import {DatePipe, NgIf, NgOptimizedImage} from "@angular/common";
import {MatButton} from "@angular/material/button";
import {UserEventResponse} from "../../../dtos/user/user-event-response";
import {EventWithStatusResponse} from "../../../dtos/event/event-with-status-response";
import {Router} from "@angular/router";
import {AuthService} from "../../../services/auth.service";
import {MatDialog} from "@angular/material/dialog";
import {UserService} from "../../../services/user.service";
import {ErrorDialogComponent} from "../error-dialog/error-dialog.component";
import {Role} from "../../../enums/role";
import {EventService} from "../../../services/event.service";
import {UpdateEventDialogComponent} from "../update-event-dialog/update-event-dialog.component";
import moment from "moment";

@Component({
  selector: 'app-event-card',
  standalone: true,
  imports: [
    MatCardModule,
    NgIf,
    NgOptimizedImage,
    MatButton,
    DatePipe,
  ],
  templateUrl: './event-card.component.html',
  styleUrl: './event-card.component.css'
})
export class EventCardComponent {
  @Input() event: EventResponse | null = null;
  @Input() isDetailed: boolean = false;

  @Output() updateData = new EventEmitter();

  private userId!: string | null;

  private authService = inject(AuthService);
  private userService = inject(UserService);
  private eventService = inject(EventService);
  private errorDialog = inject(MatDialog);
  private router = inject(Router);
  public dialog = inject(MatDialog);

  constructor() {
    this.authService.userInfo$.subscribe(value => this.userId = value?.id ?? null)
  }

  get hasImage(): boolean {
    return this.event?.imageUrl !== null && this.event?.imageUrl !== undefined;
  }

  getFieldIsRegistered(): boolean | null {
    if (this.hasFieldIsRegistered()) {
      return (this.event as EventWithStatusResponse).isRegistered;
    }
    return null;
  }

  getRegistrationDate(): string | null {
    if (this.hasRegistrationDate()) {
      return (this.event as UserEventResponse).registrationDate;
    }
    return null;
  }

  hasFieldIsRegistered() {
    return this.event !== null && this.event !== undefined && 'isRegistered' in this.event;
  }

  hasRegistrationDate() {
    return this.event !== null && this.event !== undefined && 'registrationDate' in this.event;
  }

  toDetailedPage() {
    if (this.event !== null) {
      this.router.navigate([`event/${this.event.id}`]);
    }
  }

  register() {
    this.userService.registerToEvent(this.userId!, this.event?.id!).then(() => {
      this.updateData.emit();
    }).catch((error) => {
      this.errorDialog.open(ErrorDialogComponent, {
        data: error.message
      });
    });
  }

  unregister() {
    this.userService.unregisterFromEvent(this.userId!, this.event?.id!).then(() => {
      this.updateData.emit();
    }).catch((error) => {
      this.errorDialog.open(ErrorDialogComponent, {
        data: error.message
      });
    });
  }

  isAdmin() {
    return this.authService.role === Role.Administrator;
  }

  updateEvent() {
    const dialogRef = this.dialog.open(UpdateEventDialogComponent, {
      data: this.event,
      maxWidth: '1500px',
    });

    dialogRef.afterClosed().subscribe(() => {
      this.updateData.emit();
    });
  }

  async deleteEvent() {
    await this.eventService.deleteEvent(this.event?.id!).then(() => {
      this.router.navigate(['/events']);
    }).catch((error) => {
      this.errorDialog.open(ErrorDialogComponent, {
        data: error.message
      });
    });
  }

  isRegisterButtonAvailable() {
    if (this.event !== undefined && this.event !== null) {
      return !this.getFieldIsRegistered() && this.event.placesOccupied < this.event.capacity;
    }
    return false;
  }

  formatDate(): string {
    let momentDate = moment(this.event?.date, 'YYYY-MM-DDTHH:mm:ss');
    return momentDate.format('MMMM D, YYYY H:mm');
  }

  protected readonly Date = Date;
}
