import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface LucrareDto {
  id: number;
  santierId: number;
  echipaIds: number[];
  echipeNume: string;
  descriere: string;
  termen: string;
  stare: string;
}

export interface CreateLucrareRequest {
  santierId: number;
  echipaIds?: number[];
  descriere: string;
  termen: string;
  stare: string;
}

export interface UpdateLucrareRequest {
  santierId?: number;
  echipaIds?: number[];
  descriere?: string;
  termen?: string;
  stare?: string;
}

@Injectable({ providedIn: 'root' })
export class LucrariService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get baseUrl(): string {
    return `${this.auth.getApiUrl()}/lucrari`;
  }

  getAll(santierId?: number, stare?: string): Observable<LucrareDto[]> {
    const params: Record<string, string> = {};
    if (santierId != null) params['santierId'] = santierId.toString();
    if (stare) params['stare'] = stare;
    return this.http.get<LucrareDto[]>(this.baseUrl, Object.keys(params).length > 0 ? { params } : {});
  }

  getById(id: number): Observable<LucrareDto | null> {
    return this.http.get<LucrareDto>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => of(null)),
    );
  }

  create(request: CreateLucrareRequest): Observable<LucrareDto | null> {
    return this.http.post<LucrareDto>(this.baseUrl, request).pipe(
      catchError(() => of(null)),
    );
  }

  update(id: number, request: UpdateLucrareRequest): Observable<LucrareDto | null> {
    return this.http.put<LucrareDto>(`${this.baseUrl}/${id}`, request).pipe(
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
