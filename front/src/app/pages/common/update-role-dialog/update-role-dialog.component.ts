import {Component, inject} from '@angular/core';
import {CommonModule} from "@angular/common";
import {MAT_DIALOG_DATA, MatDialog, MatDialogModule, MatDialogRef} from "@angular/material/dialog";
import {MatFormFieldModule} from "@angular/material/form-field";
import {MatInputModule} from "@angular/material/input";
import {MatButtonModule} from "@angular/material/button";
import {MatSelectModule} from "@angular/material/select";
import {Role} from "../../../enums/role";
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {UserInfo} from "../../../dtos/user/user-info";
import {UserService} from "../../../services/user.service";
import {ErrorDialogComponent} from "../error-dialog/error-dialog.component";

interface UserRoleOption {
  name: string,
  role: Role
}

@Component({
  selector: 'app-update-role-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    ReactiveFormsModule
  ],
  templateUrl: './update-role-dialog.component.html',
  styleUrl: './update-role-dialog.component.css'
})
export class UpdateRoleDialogComponent {
  public user: UserInfo = inject(MAT_DIALOG_DATA);
  private userService = inject(UserService);
  public errorDialog = inject(MatDialog);
  public dialogRef = inject(MatDialogRef<UpdateRoleDialogComponent>);

  formGroup = new FormGroup({
    role: new FormControl<UserRoleOption>(null!, [Validators.required]),
  });

  userRoles: UserRoleOption[] = [
    {name: Role.Administrator.toString(), role: Role.Administrator},
    {name: Role.Registered.toString(), role: Role.Registered},
  ];

  constructor() {
    this.formGroup.controls.role.setValue(this.userRoles.find(opt => opt.role === this.user.role)!);
  }

  changeRole() {
    this.userService.assignRoleToUser(this.user.id, this.formGroup.controls.role.value?.role!)
      .then(() => this.dialogRef.close())
      .catch((error) => {
        this.errorDialog.open(ErrorDialogComponent, {
          data: error.message
        });
      });
  }
}
