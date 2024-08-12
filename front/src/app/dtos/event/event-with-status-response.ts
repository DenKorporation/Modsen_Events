import {EventResponse} from "./event-response";

export interface EventWithStatusResponse extends EventResponse{
  isRegistered: boolean;
}
