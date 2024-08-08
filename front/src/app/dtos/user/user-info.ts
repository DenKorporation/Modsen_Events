import {Role} from "../../enums/role";

export interface UserInfo {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  role: Role;
  birthdate: string;
}
