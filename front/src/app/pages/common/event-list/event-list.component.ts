import {AfterViewInit, Component, EventEmitter, inject, Input, ViewChild} from '@angular/core';
import {EventService} from "../../../services/event.service";
import {UserService} from "../../../services/user.service";
import {MatPaginator} from "@angular/material/paginator";
import {map, merge,startWith, switchMap} from "rxjs";
import {EventResponse} from "../../../dtos/event/event-response";
import {CommonModule, NgFor, NgIf} from "@angular/common";
import {MatListModule} from "@angular/material/list";
import {EventCardComponent} from "../event-card/event-card.component";
import {FilterOptions} from "../../../dtos/event/filter-options";

@Component({
  selector: 'app-event-list',
  standalone: true,
  imports: [CommonModule, MatListModule, EventCardComponent, NgFor, NgIf, MatPaginator],
  templateUrl: './event-list.component.html',
  styleUrl: './event-list.component.css'
})
export class EventListComponent implements AfterViewInit {
  private eventService = inject(EventService);
  private userService = inject(UserService);

  @Input() userId: string | null = null;
  filter: FilterOptions | null = null;

  refreshEvents = new EventEmitter<void>();

  data: EventResponse[] = [];

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
            if (this.userId !== null) {
              return await this.userService!.getAllUserEvents(
                this.userId,
                this.paginator.pageIndex + 1,
                this.paginator.pageSize);
            } else {
              return await this.eventService!.getAllEvents({
                  pageNumber: this.paginator.pageIndex + 1,
                  pageSize: this.paginator.pageSize,
                  name: this.filter?.name ?? null,
                  address: this.filter?.address ?? null,
                  category: this.filter?.category ?? null,
                  startDate: this.filter?.startDate ?? null,
                  endDate: this.filter?.endDate ?? null
                }
              );
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

  setFilter(filter: FilterOptions | null) {
    this.filter = filter;

    this.refreshEvents.emit();
  }
}
