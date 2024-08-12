import {UserResponse} from "../user/user-response";

export interface EventUserResponse extends UserResponse {
  registrationDate: string | null;
}
