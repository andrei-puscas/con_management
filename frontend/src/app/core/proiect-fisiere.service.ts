import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from './auth.service';

export interface ProiectFisierDto {
  id: number;
  proiectId: number;
  utilizatorId: number;
  utilizatorEmail: string | null;
  numeOriginal: string;
  tipFisier: string;
  dataIncarcare: string;
}

@Injectable({ providedIn: 'root' })
export class ProiectFisiereService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private url(proiectId: number): string {
    return `${this.auth.getApiUrl()}/proiecte/${proiectId}/fisiere`;
  }

  getByProiectId(proiectId: number): Observable<ProiectFisierDto[]> {
    return this.http.get<ProiectFisierDto[]>(this.url(proiectId)).pipe(
      catchError(() => of([])),
    );
  }

  upload(proiectId: number, file: File): Observable<ProiectFisierDto | null> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ProiectFisierDto>(this.url(proiectId), formData).pipe(
      catchError(() => of(null)),
    );
  }

  delete(proiectId: number, fisierId: number): Observable<boolean> {
    return this.http.delete(`${this.url(proiectId)}/${fisierId}`, { observe: 'response' }).pipe(
      map((res) => res.status === 204),
      catchError(() => of(false)),
    );
  }

  downloadUrl(proiectId: number, fisierId: number): string {
    return `${this.url(proiectId)}/${fisierId}/download`;
  }

}
