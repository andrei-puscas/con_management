import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface EchipaDto {
  id: number;
  nume: string;
  sefEchipaId: number | null;
  sefEchipaNume: string | null;
  nrAngajati: number;
}

export interface CreateEchipaRequest {
  nume: string;
  sefEchipaId?: number | null;
}

export interface UpdateEchipaRequest {
  nume?: string;
  sefEchipaId?: number | null;
}

@Injectable({ providedIn: 'root' })
export class EchipeService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get baseUrl(): string {
    return `${this.auth.getApiUrl()}/echipe`;
  }

  getAll(): Observable<EchipaDto[]> {
    return this.http.get<EchipaDto[]>(this.baseUrl);
  }

  getById(id: number): Observable<EchipaDto | null> {
    return this.http.get<EchipaDto>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => of(null)),
    );
  }

  create(request: CreateEchipaRequest): Observable<EchipaDto | null> {
    return this.http.post<EchipaDto>(this.baseUrl, request).pipe(
      catchError(() => of(null)),
    );
  }

  update(id: number, request: UpdateEchipaRequest): Observable<EchipaDto | null> {
    return this.http.put<EchipaDto>(`${this.baseUrl}/${id}`, request).pipe(
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
