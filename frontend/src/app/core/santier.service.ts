import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface SantierDto {
  id: number;
  proiectId: number;
  adresa: string;
  descriere: string | null;
}

export interface CreateSantierRequest {
  proiectId: number;
  adresa: string;
  descriere?: string | null;
}

export interface UpdateSantierRequest {
  proiectId?: number;
  adresa?: string;
  descriere?: string | null;
}

@Injectable({ providedIn: 'root' })
export class SantierService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get baseUrl(): string {
    return `${this.auth.getApiUrl()}/santier`;
  }

  getAll(proiectId?: number): Observable<SantierDto[]> {
    const params: Record<string, string> = {};
    if (proiectId != null) params['proiectId'] = proiectId.toString();
    return this.http.get<SantierDto[]>(this.baseUrl, Object.keys(params).length > 0 ? { params } : {});
  }

  getById(id: number): Observable<SantierDto | null> {
    return this.http.get<SantierDto>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => of(null)),
    );
  }

  create(request: CreateSantierRequest): Observable<SantierDto | null> {
    return this.http.post<SantierDto>(this.baseUrl, request).pipe(
      catchError(() => of(null)),
    );
  }

  update(id: number, request: UpdateSantierRequest): Observable<SantierDto | null> {
    return this.http.put<SantierDto>(`${this.baseUrl}/${id}`, request).pipe(
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
