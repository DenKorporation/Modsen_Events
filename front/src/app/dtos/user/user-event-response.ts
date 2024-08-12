import {EventResponse} from "../event/event-response";

export interface UserEventResponse extends EventResponse{
  registrationDate: string | null;
}
