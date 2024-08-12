import {Component, inject} from '@angular/core';
import {CommonModule} from "@angular/common";
import {MAT_DIALOG_DATA, MatDialog, MatDialogModule, MatDialogRef} from "@angular/material/dialog";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";
import {EventService} from "../../../services/event.service";
import {UpdateEvent} from "../../../dtos/event/update-event";
import {ErrorDialogComponent} from "../error-dialog/error-dialog.component";
import _moment, {default as _rollupMoment} from "moment/moment";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {EventResponse} from "../../../dtos/event/event-response";
import {
  MatDatepicker,
  MatDatepickerActions,
  MatDatepickerApply,
  MatDatepickerCancel, MatDatepickerInput, MatDatepickerToggle
} from "@angular/material/datepicker";
import {provideMomentDateAdapter} from "@angular/material-moment-adapter";
import {NgxMatTimepickerModule} from "ngx-mat-timepicker";
import {MatCardModule} from "@angular/material/card";
import {MatProgressBar} from "@angular/material/progress-bar";

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
  selector: 'app-update-event-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatDatepicker,
    MatDatepickerActions,
    MatDatepickerApply,
    MatDatepickerCancel,
    MatDatepickerInput,
    MatDatepickerToggle,
    NgxMatTimepickerModule,
    MatCardModule,
    MatProgressBar
  ],
  providers: [provideMomentDateAdapter(MY_FORMATS),],
  templateUrl: './update-event-dialog.component.html',
  styleUrl: './update-event-dialog.component.css'
})
export class UpdateEventDialogComponent {
  private eventService = inject(EventService);
  public errorDialog = inject(MatDialog);
  public dialogRef = inject(MatDialogRef<UpdateEventDialogComponent>);
  public event: EventResponse = inject(MAT_DIALOG_DATA);

  selectedFile: File | null = null;

  updateEventFormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(250)]),
    address: new FormControl('', [Validators.required, Validators.maxLength(150)]),
    category: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    capacity: new FormControl(0, [Validators.required, Validators.min(0)]),
    date: new FormControl(moment(null), [Validators.required]),
    time: new FormControl('', [Validators.required])
  });

  constructor() {
    this.updateEventFormGroup.controls.name.setValue(this.event.name);
    this.updateEventFormGroup.controls.description.setValue(this.event.description);
    this.updateEventFormGroup.controls.address.setValue(this.event.address);
    this.updateEventFormGroup.controls.category.setValue(this.event.category);
    this.updateEventFormGroup.controls.capacity.setValue(this.event.capacity);
    let dateTime = moment(this.event.date, 'YYYY-MM-DDTHH:mm:ss');
    this.updateEventFormGroup.controls.date.setValue(dateTime);
    this.updateEventFormGroup.controls.time.setValue(dateTime.format('HH:mm'));
  }

  async updateEvent() {
    const eventRequest: UpdateEvent = {
      id: this.event.id,
      name: this.updateEventFormGroup.controls.name.value!,
      description: this.updateEventFormGroup.controls.description.value!,
      category: this.updateEventFormGroup.controls.category.value!,
      capacity: this.updateEventFormGroup.controls.capacity.value!,
      address: this.updateEventFormGroup.controls.address.value!,
      date: `${this.updateEventFormGroup.controls.date.value!.format('YYYY-MM-DD')}T${this.updateEventFormGroup.controls.time.value}:00`
    };
    await this.eventService.updateEvent(eventRequest)
      .catch((error) => {
        error.message
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      });

    this.dialogRef.close();
  }

  async updateImage(){
    await this.eventService.updateImage(this.event.id, this.selectedFile!)
      .then(() => this.dialogRef.close())
      .catch((error) => {
        error.message
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      });
  }

  async deleteImage(){
    await this.eventService.deleteImage(this.event.id)
      .then(() => this.dialogRef.close())
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
}
