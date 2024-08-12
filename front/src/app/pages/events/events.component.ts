import {Component, ViewChild} from '@angular/core';
import {EventListComponent} from "../common/event-list/event-list.component";
import {MatFormFieldModule} from "@angular/material/form-field";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import _moment from 'moment';
import {default as _rollupMoment} from 'moment';
import {provideMomentDateAdapter} from "@angular/material-moment-adapter";
import {MatInputModule} from "@angular/material/input";
import {NgIf} from "@angular/common";
import {MatButtonModule} from "@angular/material/button";
import {MatDatepickerModule,} from "@angular/material/datepicker";
import {FilterOptions} from "../../dtos/event/filter-options";

const moment = _rollupMoment || _moment;

export const MY_FORMATS = {
  parse: {
    dateInput: 'DD.MM.YYYY',
  },
  display: {
    dateInput: 'DD.MM.YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [
    EventListComponent,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    NgIf,
    MatButtonModule,
    MatDatepickerModule
  ],
  providers: [provideMomentDateAdapter(MY_FORMATS),],
  templateUrl: './events.component.html',
  styleUrl: './events.component.css'
})
export class EventsComponent {
  filterFormGroup = new FormGroup({
    name: new FormControl('', [Validators.maxLength(100)]),
    address: new FormControl('', [Validators.maxLength(150)]),
    category: new FormControl('', [Validators.maxLength(50)]),
    startDate: new FormControl(moment()),
    endDate: new FormControl(moment()),
  });

  @ViewChild(EventListComponent) eventList!: EventListComponent;

  searchData() {
    this.eventList.setFilter({
      pageSize: null!,
      pageNumber: null!,
      name: this.handleData(this.filterFormGroup.controls.name.value),
      address: this.handleData(this.filterFormGroup.controls.address.value),
      category: this.handleData(this.filterFormGroup.controls.category.value),
      startDate: this.filterFormGroup.controls.startDate.value?.format('YYYY-MM-DDTHH:mm:ss.SSS') ?? null,
      endDate: this.filterFormGroup.controls.endDate.value?.format('YYYY-MM-DDTHH:mm:ss.SSS') ?? null,
    });
  }

  handleData(str: string | null) {
    if (str === null || str.trim() === '') {
      return null;
    } else {
      return str.trim();
    }
  }
}
