import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface ProiectDto {
  id: number;
  nume: string;
  client: string;
  dataStart: string;
  dataSfarsit: string | null;
  stare: string;
}

export interface CreateProiectRequest {
  nume: string;
  client: string;
  dataStart: string;
  dataSfarsit?: string | null;
  stare: string;
}

export interface UpdateProiectRequest {
  nume?: string;
  client?: string;
  dataStart?: string;
  dataSfarsit?: string | null;
  stare?: string;
}

@Injectable({ providedIn: 'root' })
export class ProiecteService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get baseUrl(): string {
    return `${this.auth.getApiUrl()}/proiecte`;
  }

  getAll(stare?: string): Observable<ProiectDto[]> {
    const params: Record<string, string> = {};
    if (stare) params['stare'] = stare;
    return this.http.get<ProiectDto[]>(this.baseUrl, Object.keys(params).length > 0 ? { params } : {});
  }

  getById(id: number): Observable<ProiectDto | null> {
    return this.http.get<ProiectDto>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => of(null)),
    );
  }

  create(request: CreateProiectRequest): Observable<ProiectDto | null> {
    return this.http.post<ProiectDto>(this.baseUrl, request).pipe(
      catchError(() => of(null)),
    );
  }

  update(id: number, request: UpdateProiectRequest): Observable<ProiectDto | null> {
    return this.http.put<ProiectDto>(`${this.baseUrl}/${id}`, request).pipe(
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
