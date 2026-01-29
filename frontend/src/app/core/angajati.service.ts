import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface AngajatDto {
  id: number;
  echipaId: number | null;
  echipaNume: string | null;
  nume: string;
  rol: string;
  competente: string | null;
}

export interface CreateAngajatRequest {
  nume: string;
  rol: string;
  competente?: string | null;
  echipaId?: number | null;
}

export interface UpdateAngajatRequest {
  nume?: string;
  rol?: string;
  competente?: string | null;
  echipaId?: number | null;
}

@Injectable({ providedIn: 'root' })
export class AngajatiService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get baseUrl(): string {
    return `${this.auth.getApiUrl()}/angajati`;
  }

  getAll(echipaId?: number): Observable<AngajatDto[]> {
    const params: Record<string, string> = {};
    if (echipaId != null) params['echipaId'] = echipaId.toString();
    return this.http.get<AngajatDto[]>(this.baseUrl, Object.keys(params).length > 0 ? { params } : {});
  }

  getById(id: number): Observable<AngajatDto | null> {
    return this.http.get<AngajatDto>(`${this.baseUrl}/${id}`).pipe(
      catchError(() => of(null)),
    );
  }

  create(request: CreateAngajatRequest): Observable<AngajatDto | null> {
    return this.http.post<AngajatDto>(this.baseUrl, request).pipe(
      catchError(() => of(null)),
    );
  }

  update(id: number, request: UpdateAngajatRequest): Observable<AngajatDto | null> {
    return this.http.put<AngajatDto>(`${this.baseUrl}/${id}`, request).pipe(
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
