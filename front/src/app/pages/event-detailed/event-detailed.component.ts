import {AfterViewInit, Component, EventEmitter, inject, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {EventCardComponent} from "../common/event-card/event-card.component";
import {map, merge, startWith, switchMap} from "rxjs";
import {EventService} from "../../services/event.service";
import {EventWithStatusResponse} from "../../dtos/event/event-with-status-response";
import {CommonModule, NgIf} from "@angular/common";
import {UserListComponent} from "../common/user-list/user-list.component";

@Component({
  selector: 'app-event-detailed',
  standalone: true,
  imports: [
    CommonModule,
    EventCardComponent,
    NgIf,
    UserListComponent
  ],
  templateUrl: './event-detailed.component.html',
  styleUrl: './event-detailed.component.css'
})
export class EventDetailedComponent implements AfterViewInit {
  private eventService = inject(EventService);
  private route = inject(ActivatedRoute);

  eventId: string = '';
  data!: EventWithStatusResponse | null;

  refreshEvents = new EventEmitter<void>();

  @ViewChild(UserListComponent) userList!: UserListComponent;

  isLoadingResults = true;

  constructor() {
    this.eventId = this.route.snapshot.paramMap.get('id')!;
  }

  ngAfterViewInit() {
    const refreshEvent = this.refreshEvents.pipe(map(() => ({eventType: 'refresh'})));

    merge(refreshEvent)
      .pipe(
        startWith({eventType: 'init'}),
        switchMap(async () => {
          this.isLoadingResults = true;

          try {
            return this.eventService.getEventById(this.eventId);
          } catch (e) {
            return null;
          }
        }),
        map(data => {
          this.isLoadingResults = false;

          if (data === null) {
            return null;
          }

          return data;
        }),
      )
      .subscribe(data => (this.data = data));
  }

  onEventUpdate() {
    this.refreshEvents.emit();
    this.userList.updateList();
  }

  onUserListUpdate() {
    this.refreshEvents.emit();
  }
}
