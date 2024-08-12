import {Component, inject} from '@angular/core';
import {CommonModule, NgStyle} from "@angular/common";
import {MatCardModule} from "@angular/material/card";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule, MatIconButton} from "@angular/material/button";
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from "@angular/forms";
import {MatIconModule} from "@angular/material/icon";
import {Router, RouterLink} from "@angular/router";
import {AuthService} from "../../services/auth.service";
import {digitsValidator} from "../../validators/digits.validator";
import {lowercaseValidator} from "../../validators/lowercase.validator";
import {uppercaseValidator} from "../../validators/uppercase.validator";
import {specialCharactersValidator} from "../../validators/special-characters.validator";
import _moment from 'moment';
import {default as _rollupMoment} from 'moment';
import {MatDatepickerModule} from "@angular/material/datepicker";
import {MatMomentDateModule, provideMomentDateAdapter} from "@angular/material-moment-adapter";
import {ErrorDialogComponent} from "../common/error-dialog/error-dialog.component";
import {MatDialog} from "@angular/material/dialog";
import {CreateUser} from "../../dtos/user/create-user";

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
  selector: 'app-sign-up',
  standalone: true,
  imports: [
    CommonModule,
    MatDatepickerModule,
    MatMomentDateModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatIconButton,
    NgStyle,
    RouterLink
  ],
  providers: [provideMomentDateAdapter(MY_FORMATS),],
  templateUrl: './sign-up.component.html',
  styleUrl: './sign-up.component.css'
})
export class SignUpComponent {
  private authService = inject(AuthService);
  public errorDialog = inject(MatDialog);

  public passwordHide = true;
  public confirmationHide = true;

  signUpFormGroup = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    birthdate: new FormControl(moment(), [Validators.required]),
    email: new FormControl('', [Validators.required, Validators.email, Validators.maxLength(256)]),
    password: new FormControl('',
      [
        Validators.required,
        Validators.minLength(6),
        digitsValidator(),
        lowercaseValidator(),
        uppercaseValidator(),
        specialCharactersValidator()
      ]),
    confirmPassword: new FormControl
  });

  constructor() {
    this.signUpFormGroup.controls.password.valueChanges.subscribe(() => {
      this.signUpFormGroup.controls.confirmPassword.updateValueAndValidity();
    });

    this.signUpFormGroup.controls.confirmPassword.setValidators([
      Validators.required,
      confirmPasswordValidator(this.signUpFormGroup.controls.password)
    ]);
  }

  signUp() {
    let user: CreateUser = {
      firstName: this.signUpFormGroup.controls.firstName.value!,
      lastName: this.signUpFormGroup.controls.lastName.value!,
      email: this.signUpFormGroup.controls.email.value!,
      password: this.signUpFormGroup.controls.password.value!,
      birthday: this.signUpFormGroup.controls.birthdate.value!.format("YYYY-MM-DD")!
    }
    this.authService.register(user).catch((error) => {
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      }
    );
  }

  getPasswordError() {
    const control = this.signUpFormGroup.controls.password;
    if (control.hasError('required'))
      return 'Password required';
    if (control.hasError('lowercase'))
      return 'Password must contain lowercase letter';
    if (control.hasError('uppercase'))
      return 'Password must contain uppercase letter';
    if (control.hasError('digits'))
      return 'Password must contain digit';
    if (control.hasError('specialCharacters'))
      return 'Password must contain special character';
    if (control.hasError('minlength'))
      return 'Password must contain at least 6 characters';

    return 'Invalid password';
  }
}

export function confirmPasswordValidator(passwordControl: AbstractControl): ValidatorFn {
  return (confirmPasswordControl: AbstractControl): ValidationErrors | null => {
    if (!confirmPasswordControl.value || !passwordControl.value) {
      return null;
    }
    return passwordControl.value === confirmPasswordControl.value ? null : {passwordMismatch: true};
  };
}
