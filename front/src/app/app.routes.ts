import {Routes} from '@angular/router';
import {LoginComponent} from "./pages/login/login.component";
import {SignUpComponent} from "./pages/sign-up/sign-up.component";
import {authGuard} from "./guards/auth.guard";
import {ProfileComponent} from "./pages/profile/profile.component";
import {UserEventsComponent} from "./pages/user-events/user-events.component";
import {EventsComponent} from "./pages/events/events.component";
import {EventDetailedComponent} from "./pages/event-detailed/event-detailed.component";
import {CreateEventComponent} from "./pages/create-event/create-event.component";

export const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'sign-up', component: SignUpComponent},
  {path: 'events', component: EventsComponent, canActivate: [authGuard]},
  {path: 'event/:id', component: EventDetailedComponent, canActivate: [authGuard]},
  {path: 'create-event', component: CreateEventComponent, canActivate: [authGuard]},
  {path: 'profile', component: ProfileComponent, canActivate: [authGuard]},
  {path: 'my-events', component: UserEventsComponent, canActivate: [authGuard]},
  {path: '', redirectTo: '/login', pathMatch: "full"},
  {path: '**', redirectTo: '/login'},
];
