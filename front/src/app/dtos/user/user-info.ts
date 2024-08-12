import {Role} from "../../enums/role";
import {UserResponse} from "./user-response";

export interface UserInfo extends UserResponse {
  role: Role;
}
