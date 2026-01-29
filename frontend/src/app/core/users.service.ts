import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface UserDto {
  id: number;
  email: string;
  rol: string;
  angajatId: number | null;
  angajatNume: string | null;
}

export interface CreateUserRequest {
  email: string;
  password: string;
  rol: string;
}

export interface UpdateUserRequest {
  email?: string;
  password?: string;
  rol?: string;
}

@Injectable({ providedIn: 'root' })
export class UsersService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get baseUrl(): string {
    return `${this.auth.getApiUrl()}/users`;
  }

  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<UserDto | null> {
    return this.http.get<UserDto>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => of(null)),
    );
  }

  create(request: CreateUserRequest): Observable<UserDto | null> {
    return this.http.post<UserDto>(this.baseUrl, request).pipe(
      catchError(() => of(null)),
    );
  }

  update(id: number, request: UpdateUserRequest): Observable<UserDto | null> {
    return this.http.put<UserDto>(`${this.baseUrl}/${id}`, request).pipe(
      catchError(() => of(null)),
    );
  }

  delete(id: number): Observable<boolean> {
    return this.http.delete(`${this.baseUrl}/${id}`, { observe: 'response' }).pipe(
      map((res) => res.status === 204),
      catchError(() => of(false)),
    );
  }
}
