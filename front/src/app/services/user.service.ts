import {inject, Injectable} from '@angular/core';
import {environment} from "../../environments/environment";
import {CreateUser} from "../dtos/user/create-user";
import {catchError, lastValueFrom, throwError} from "rxjs";
import {UserResponse} from "../dtos/user/user-response";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {PagedList} from "../dtos/paged-list";
import {Role} from "../enums/role";
import {UserEventResponse} from "../dtos/user/user-event-response";
import {UserInfo} from "../dtos/user/user-info";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private userApiUrl = `${environment.apiUrl}/users`;

  private httpClient = inject(HttpClient);

  public getAllUsers(pageNumber: number, pageSize: number): Promise<PagedList<UserInfo>> {
    const params: string[] = [`pageNumber=${pageNumber}`, `pageSize=${pageSize}`];

    const requestUrl = `${this.userApiUrl}?${params.join('&')}`;

    return lastValueFrom(this.httpClient.get<PagedList<UserInfo>>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public getAllUserEvents(userId: string, pageNumber: number, pageSize: number): Promise<PagedList<UserEventResponse>> {
    const params: string[] = [`pageNumber=${pageNumber}`, `pageSize=${pageSize}`];

    const requestUrl = `${this.userApiUrl}/${userId}/events?${params.join('&')}`;

    return lastValueFrom(this.httpClient.get<PagedList<UserEventResponse>>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public getUserById(userId: string): Promise<UserResponse> {
    const requestUrl = `${this.userApiUrl}/${userId}`;

    return lastValueFrom(this.httpClient.get<UserResponse>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public createUser(request: CreateUser): Promise<UserResponse> {
    const requestUrl = `${this.userApiUrl}`;

    return lastValueFrom(this.httpClient.post<UserResponse>(requestUrl, request).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public registerToEvent(userId: string, eventId: string): Promise<any> {
    const requestUrl = `${this.userApiUrl}/${userId}/event/${eventId}`;

    return lastValueFrom(this.httpClient.post<any>(requestUrl, null).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public unregisterFromEvent(userId: string, eventId: string): Promise<any> {
    const requestUrl = `${this.userApiUrl}/${userId}/event/${eventId}`;

    return lastValueFrom(this.httpClient.delete<any>(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }


  public assignRoleToUser(id: string, role: Role): Promise<any> {
    const requestUrl = `${this.userApiUrl}/${id}?role=${role}`;

    return lastValueFrom(this.httpClient.put<any>(requestUrl, null).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }

  public deleteUser(id: string): Promise<any> {
    const requestUrl = `${this.userApiUrl}/${id}`;

    return lastValueFrom(this.httpClient.delete(requestUrl).pipe(
      catchError((error: HttpErrorResponse) => {
        return throwError(() => new Error(error.error.message ?? "Unknown error"));
      })));
  }
}
