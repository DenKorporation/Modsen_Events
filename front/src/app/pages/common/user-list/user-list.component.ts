import {AfterViewInit, Component, EventEmitter, inject, Input, ViewChild} from '@angular/core';
import {EventCardComponent} from "../event-card/event-card.component";
import {MatPaginator} from "@angular/material/paginator";
import {NgForOf} from "@angular/common";
import {ProfileCardComponent} from "../profile-card/profile-card.component";
import {EventService} from "../../../services/event.service";
import {UserService} from "../../../services/user.service";
import {map, merge, startWith, switchMap} from "rxjs";
import {UserResponse} from "../../../dtos/user/user-response";

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    EventCardComponent,
    MatPaginator,
    NgForOf,
    ProfileCardComponent
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements AfterViewInit {
  private userService = inject(UserService);
  private eventService = inject(EventService);

  @Input() eventId: string | null = null;

  refreshEvents = new EventEmitter<void>();

  data: UserResponse[] = [];

  resultsLength = 0;
  isLoadingResults = true;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngAfterViewInit() {
    const pageEvents = this.paginator.page.pipe(map(() => ({eventType: 'page'})));
    const refreshEvent = this.refreshEvents.pipe(map(() => ({eventType: 'refresh'})));

    merge(pageEvents, refreshEvent)
      .pipe(
        startWith({eventType: 'init'}),
        switchMap(async (event) => {
          this.isLoadingResults = true;

          if (event.eventType !== 'page') {
            this.paginator.pageIndex = 0;
          }

          try {
            if (this.eventId !== null) {
              return await this.eventService!.getAllUsersFromEvent(
                this.eventId,
                this.paginator.pageIndex + 1,
                this.paginator.pageSize);
            } else {
              return await this.userService!.getAllUsers(this.paginator.pageIndex + 1, this.paginator.pageSize);
            }
          } catch (e) {
            return null;
          }
        }),
        map(data => {
          this.isLoadingResults = false;

          if (data === null) {
            return [];
          }

          this.resultsLength = data.totalCount;
          return data.items;
        }),
      )
      .subscribe(data => (this.data = data));
  }

  onUpdateData() {
    this.refreshEvents.emit();
  }
}
