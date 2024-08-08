export interface UserEventResponse {
  id: string;
  name: string;
  description: string;
  date: string;
  address: string;
  category: string;
  capacity: number;
  image_url: string | null;
  placesOccupied: number;
  registrationDate: string | null;
}
