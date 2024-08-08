import {Component, inject} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatCardModule} from "@angular/material/card";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatIconModule} from "@angular/material/icon";
import {CommonModule, NgStyle} from "@angular/common";
import {MatButtonModule, MatIconButton} from "@angular/material/button";
import {MatInputModule} from "@angular/material/input";
import {digitsValidator} from "../../validators/digits.validator";
import {lowercaseValidator} from "../../validators/lowercase.validator";
import {uppercaseValidator} from "../../validators/uppercase.validator";
import {specialCharactersValidator} from "../../validators/special-characters.validator";
import {Router, RouterLink} from "@angular/router";
import {MatDialog, MatDialogRef} from "@angular/material/dialog";
import {ErrorDialogComponent} from "../error-dialog/error-dialog/error-dialog.component";

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
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
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})

export class LoginComponent {
  private authService = inject(AuthService);
  public errorDialog = inject(MatDialog);

  public hide = true;

  loginFormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('',
      [
        Validators.required,
        Validators.minLength(6),
        digitsValidator(),
        lowercaseValidator(),
        uppercaseValidator(),
        specialCharactersValidator()
      ]),
  });

  signIn() {
    this.authService.login(this.loginFormGroup.controls.email.value!, this.loginFormGroup.controls.password.value!).catch((error) => {
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      }
    );
  }

  getPasswordError() {
    const control = this.loginFormGroup.controls.password;
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

