import {Component, inject} from '@angular/core';
import {MatCardModule} from "@angular/material/card";
import {DatePipe, NgIf} from "@angular/common";
import {MatButton} from "@angular/material/button";
import {
  MatDatepicker,
  MatDatepickerActions,
  MatDatepickerApply,
  MatDatepickerCancel, MatDatepickerInput, MatDatepickerToggle
} from "@angular/material/datepicker";
import {MatError, MatFormField, MatHint, MatLabel, MatSuffix} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {NgxMatTimepickerComponent, NgxMatTimepickerModule} from "ngx-mat-timepicker";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatDialog, MatDialogClose} from "@angular/material/dialog";
import _moment, {default as _rollupMoment} from "moment";
import {provideMomentDateAdapter} from "@angular/material-moment-adapter";
import {EventService} from "../../services/event.service";
import {ErrorDialogComponent} from "../common/error-dialog/error-dialog.component";
import {CreateEvent} from "../../dtos/event/create-event";
import {Router} from "@angular/router";

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
  selector: 'app-create-event',
  standalone: true,
  imports: [
    MatCardModule,
    DatePipe,
    NgIf,
    MatButton,
    MatDatepicker,
    MatDatepickerActions,
    MatDatepickerApply,
    MatDatepickerCancel,
    MatDatepickerInput,
    MatDatepickerToggle,
    MatError,
    MatFormField,
    MatHint,
    MatInput,
    MatLabel,
    MatSuffix,
    NgxMatTimepickerComponent,
    ReactiveFormsModule,
    MatDialogClose,
    NgxMatTimepickerModule
  ],
  providers: [provideMomentDateAdapter(MY_FORMATS),],
  templateUrl: './create-event.component.html',
  styleUrl: './create-event.component.css'
})
export class CreateEventComponent {
  private eventService = inject(EventService);
  public errorDialog = inject(MatDialog);
  public router = inject(Router);

  selectedFile: File | null = null;

  createEventFormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(250)]),
    address: new FormControl('', [Validators.required, Validators.maxLength(150)]),
    category: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    capacity: new FormControl(0, [Validators.required, Validators.min(0)]),
    date: new FormControl(moment(null), [Validators.required]),
    time: new FormControl('', [Validators.required])
  });

  async createEvent() {
    const eventRequest: CreateEvent = {
      name: this.createEventFormGroup.controls.name.value!,
      description: this.createEventFormGroup.controls.description.value!,
      category: this.createEventFormGroup.controls.category.value!,
      capacity: this.createEventFormGroup.controls.capacity.value!,
      address: this.createEventFormGroup.controls.address.value!,
      date: `${this.createEventFormGroup.controls.date.value!.format('YYYY-MM-DD')}T${this.createEventFormGroup.controls.time.value}:00`
    };
    await this.eventService.createEvent(eventRequest)
      .then(async result => {
        if (this.selectedFile) {
          await this.uploadImage(result.id);
        } else {
          this.router.navigate(['/events']);
        }
      })
      .catch((error) => {
        error.message
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      });
  }

  async uploadImage(eventId: string) {
    await this.eventService.uploadImage(eventId, this.selectedFile!)
      .then(() => {
        this.router.navigate(['/events']);
      })
      .catch((error) => {
        error.message
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  backToEvents() {
    this.router.navigate(['/events']);
  }
}
