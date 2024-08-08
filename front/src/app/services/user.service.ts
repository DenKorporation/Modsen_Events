import {inject, Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {CreateUser} from "../dtos/user/create-user";
import {catchError, lastValueFrom, Observable, throwError} from "rxjs";
import {UserResponse} from "../dtos/user/user-response";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {PagedList} from "../dtos/paged-list";
import {Role} from "../enums/role";
import {UserEventResponse} from "../dtos/user/user-event-response";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private userApiUrl = `${environment.apiUrl}/users`;

  private httpClient = inject(HttpClient);

  public getAllUsers(pageNumber: number, pageSize: number): Observable<PagedList<UserResponse>> {
    const params: string[] = [`pageNumber=${pageNumber}`, `pageSize=${pageSize}`];

    const requestUrl = `${this.userApiUrl}?${params.join('&')}`;

    return this.httpClient.get<PagedList<UserResponse>>(requestUrl);
  }

  public getAllUserEvents(userId: string, pageNumber: number, pageSize: number): Observable<PagedList<UserEventResponse>> {
    const params: string[] = [`pageNumber=${pageNumber}`, `pageSize=${pageSize}`];

    const requestUrl = `${this.userApiUrl}/${userId}/events?${params.join('&')}`;

    return this.httpClient.get<PagedList<UserEventResponse>>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      }));
  }

  public getUserById(userId: string): Observable<UserResponse> {
    const requestUrl = `${this.userApiUrl}/${userId}`;

    return this.httpClient.get<UserResponse>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      }));
  }

  public createUser(request: CreateUser): Promise<UserResponse> {
    const requestUrl = `${this.userApiUrl}`;

    return lastValueFrom(this.httpClient.post<UserResponse>(requestUrl, request).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public registerToEvent(userId: string, eventId: string): Observable<any> {
    const requestUrl = `${this.userApiUrl}/${userId}/event/${eventId}`;

    return this.httpClient.post<any>(requestUrl, null).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      }));
  }

  public unregisterFromEvent(userId: string, eventId: string): Observable<any> {
    const requestUrl = `${this.userApiUrl}/${userId}/event/${eventId}`;

    return this.httpClient.delete<any>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      }));
  }


  public assignRoleToUser(id: number, role: Role): Observable<any> {
    const requestUrl = `${this.userApiUrl}/${id}?role=${role}`;

    return this.httpClient.put<any>(requestUrl, null).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      }));
  }

  public deleteUser(id: number): Observable<any> {
    const requestUrl = `${this.userApiUrl}/${id}`;

    return this.httpClient.delete(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      }));
  }
}
