import { Component } from '@angular/core';
import {UserListComponent} from "../common/user-list/user-list.component";

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    UserListComponent
  ],
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent {

}
