import {Component, inject} from '@angular/core';
import {EventListComponent} from "../common/event-list/event-list.component";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-user-events',
  standalone: true,
  imports: [
    EventListComponent
  ],
  templateUrl: './user-events.component.html',
  styleUrl: './user-events.component.css'
})
export class UserEventsComponent {
  authService = inject(AuthService);

  userId: string | null = null;

  constructor() {
    this.authService.userInfo$.subscribe(value => this.userId = value?.id ?? null)
  }
}
