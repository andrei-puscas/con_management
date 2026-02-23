import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface ProiectComentariuDto {
  id: number;
  proiectId: number;
  utilizatorId: number;
  utilizatorEmail: string | null;
  text: string;
  dataCreare: string;
}

@Injectable({ providedIn: 'root' })
export class ProiectComentariiService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private url(proiectId: number): string {
    return `${this.auth.getApiUrl()}/proiecte/${proiectId}/comentarii`;
  }

  getByProiectId(proiectId: number): Observable<ProiectComentariuDto[]> {
    return this.http.get<ProiectComentariuDto[]>(this.url(proiectId)).pipe(
      catchError(() => of([])),
    );
  }

  create(proiectId: number, text: string): Observable<ProiectComentariuDto | null> {
    return this.http.post<ProiectComentariuDto>(this.url(proiectId), { text }).pipe(
      catchError(() => of(null)),
    );
  }

  delete(proiectId: number, comentariuId: number): Observable<boolean> {
    return this.http.delete(`${this.url(proiectId)}/${comentariuId}`, { observe: 'response' }).pipe(
      map((res) => res.status === 204),
      catchError(() => of(false)),
    );
  }
}
