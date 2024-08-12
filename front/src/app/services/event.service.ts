import {inject, Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {catchError, lastValueFrom, Observable, throwError} from "rxjs";
import {EventResponse} from "../dtos/event/event-response";
import {PagedList} from "../dtos/paged-list";
import {EventUserResponse} from "../dtos/event/event-user-response";
import {FilterOptions} from "../dtos/event/filter-options";
import {CreateEvent} from "../dtos/event/create-event";
import {UpdateEvent} from "../dtos/event/update-event";
import {ImageResponse} from "../dtos/event/image-response";
import {EventWithStatusResponse} from "../dtos/event/event-with-status-response";

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private readonly eventApiUrl = `${environment.apiUrl}/events`;

  private httpClient = inject(HttpClient);

  public getEventById(eventId: string): Promise<EventWithStatusResponse> {
    const requestUrl = `${this.eventApiUrl}/${eventId}`;

    return lastValueFrom(this.httpClient.get<EventWithStatusResponse>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public getEventByName(name: string): Promise<EventWithStatusResponse> {
    const requestUrl = `${this.eventApiUrl}/${name}`;

    return lastValueFrom(this.httpClient.get<EventWithStatusResponse>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public getAllUsersFromEvent(eventId: string, pageNumber: number, pageSize: number): Promise<PagedList<EventUserResponse>> {
    const params: string[] = [`pageNumber=${pageNumber}`, `pageSize=${pageSize}`];

    const requestUrl = `${this.eventApiUrl}/${eventId}/users?${params.join('&')}`;

    return lastValueFrom(this.httpClient.get<PagedList<EventUserResponse>>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public getAllEvents(options: FilterOptions): Promise<PagedList<EventResponse>> {
    const params: string[] = [`pageNumber=${options.pageNumber}`, `pageSize=${options.pageSize}`];

    if (options.name !== null) {
      params.push(`name=${options.name}`);
    }
    if (options.address !== null) {
      params.push(`address=${options.address}`);
    }
    if (options.category !== null) {
      params.push(`category=${options.category}`);
    }
    if (options.startDate !== null) {
      params.push(`startDate=${options.startDate}`);
    }
    if (options.endDate !== null) {
      params.push(`endDate=${options.endDate}`);
    }

    const requestUrl = `${this.eventApiUrl}?${params.join('&')}`;

    return lastValueFrom(this.httpClient.get<PagedList<EventResponse>>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public createEvent(request: CreateEvent): Promise<EventResponse> {
    const requestUrl = `${this.eventApiUrl}`;

    return lastValueFrom(this.httpClient.post<EventResponse>(requestUrl, request).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public updateEvent(request: UpdateEvent): Promise<EventResponse> {
    const requestUrl = `${this.eventApiUrl}`;

    return lastValueFrom(this.httpClient.put<EventResponse>(requestUrl, request).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public deleteEvent(id: string): Promise<any> {
    const requestUrl = `${this.eventApiUrl}/${id}`;

    return lastValueFrom(this.httpClient.delete(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public uploadImage(eventId: string, file: File): Promise<ImageResponse> {
    const formData: FormData = new FormData();
    formData.append('previewImage', file, file.name);

    const requestUrl = `${this.eventApiUrl}/${eventId}/preview`;

    return lastValueFrom(this.httpClient.post<ImageResponse>(requestUrl, formData).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public updateImage(eventId: string, file: File): Promise<ImageResponse> {
    const formData: FormData = new FormData();
    formData.append('previewImage', file, file.name);

    const requestUrl = `${this.eventApiUrl}/${eventId}/preview`;

    return lastValueFrom(this.httpClient.put<ImageResponse>(requestUrl, formData).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public deleteImage(eventId: string): Promise<any> {
    const requestUrl = `${this.eventApiUrl}/${eventId}/preview`;

    return lastValueFrom(this.httpClient.delete<any>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }
}
