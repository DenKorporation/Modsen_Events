export interface EventResponse {
  id: string;
  name: string;
  description: string;
  date: string;
  address: string;
  category: string;
  capacity: number;
  imageUrl: string | null;
  placesOccupied: number;
}
