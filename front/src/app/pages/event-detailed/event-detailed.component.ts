import {AfterViewInit, Component, EventEmitter, inject, Input, ViewChild} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {EventCardComponent} from "../common/event-card/event-card.component";
import {map, merge, startWith, switchMap} from "rxjs";
import {EventService} from "../../services/event.service";
import {UserService} from "../../services/user.service";
import {FilterOptions} from "../../dtos/event/filter-options";
import {EventResponse} from "../../dtos/event/event-response";
import {MatPaginator} from "@angular/material/paginator";
import {EventWithStatusResponse} from "../../dtos/event/event-with-status-response";
import {CommonModule, NgIf} from "@angular/common";

@Component({
  selector: 'app-event-detailed',
  standalone: true,
  imports: [
    CommonModule,
    EventCardComponent,
    NgIf
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

  isLoadingResults = true;

  ngAfterViewInit() {
    this.eventId = this.route.snapshot.paramMap.get('id')!;

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

  onUpdateData() {
    this.refreshEvents.emit();
  }
}
