import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

export interface DevizLinieDto {
  id: number;
  numar: number;
  descriere: string;
  um: string;
  cantitate: number;
  pretUnitar: number;
  total: number;
}

export interface DevizDto {
  id: number;
  proiectId: number;
  numeProiect: string;
  titlu: string;
  numarInregistrare?: string;
  beneficiar?: string;
  executant?: string;
  cotaTVA: number;
  data: string;
  linii: DevizLinieDto[];
  totalGeneral: number;
}

export interface CreateDevizLinieRequest {
  numar: number;
  descriere: string;
  um: string;
  cantitate: number;
  pretUnitar: number;
}

export interface CreateDevizRequest {
  titlu: string;
  numarInregistrare?: string;
  beneficiar?: string;
  executant?: string;
  cotaTVA: number;
  data: string;
  linii: CreateDevizLinieRequest[];
}

export interface CreateDevizGlobalRequest extends CreateDevizRequest {
  proiectId: number;
}

@Injectable({ providedIn: 'root' })
export class DevizeService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  private get api(): string {
    return this.auth.getApiUrl();
  }

  // Endpoint global
  getAll(): Observable<DevizDto[]> {
    return this.http.get<DevizDto[]>(`${this.api}/devize`);
  }

  getById(id: number): Observable<DevizDto> {
    return this.http.get<DevizDto>(`${this.api}/devize/${id}`);
  }

  createGlobal(req: CreateDevizGlobalRequest): Observable<DevizDto> {
    return this.http.post<DevizDto>(`${this.api}/devize`, req);
  }

  updateGlobal(id: number, req: Partial<CreateDevizGlobalRequest>): Observable<DevizDto> {
    return this.http.put<DevizDto>(`${this.api}/devize/${id}`, req);
  }

  deleteGlobal(id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/devize/${id}`);
  }

  getPdfUrl(id: number): string {
    return `${this.api}/devize/${id}/pdf`;
  }

  // Endpoint per proiect
  getByProiectId(proiectId: number): Observable<DevizDto[]> {
    return this.http.get<DevizDto[]>(`${this.api}/proiecte/${proiectId}/devize`);
  }

  createForProiect(proiectId: number, req: CreateDevizRequest): Observable<DevizDto> {
    return this.http.post<DevizDto>(`${this.api}/proiecte/${proiectId}/devize`, req);
  }

  delete(proiectId: number, id: number): Observable<void> {
    return this.http.delete<void>(`${this.api}/proiecte/${proiectId}/devize/${id}`);
  }
}
