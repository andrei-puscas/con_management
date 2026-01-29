import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap, catchError, of } from 'rxjs';

const TOKEN_KEY = 'conmanagement_token';
const ROLE_KEY = 'conmanagement_role';
const EMAIL_KEY = 'conmanagement_email';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expires: string;
  email: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = 'http://localhost:5000/api';
  private tokenSignal = signal<string | null>(typeof localStorage !== 'undefined' ? localStorage.getItem(TOKEN_KEY) : null);
  private roleSignal = signal<string | null>(typeof localStorage !== 'undefined' ? localStorage.getItem(ROLE_KEY) : null);
  private emailSignal = signal<string | null>(typeof localStorage !== 'undefined' ? localStorage.getItem(EMAIL_KEY) : null);

  readonly isAuthenticated = () => !!this.tokenSignal();
  readonly token = this.tokenSignal.asReadonly();
  readonly role = this.roleSignal.asReadonly();
  readonly email = this.emailSignal.asReadonly();
  readonly isAdmin = () => this.roleSignal() === 'Admin';

  getEmail(): string | null {
    return this.emailSignal() ?? (typeof localStorage !== 'undefined' ? localStorage.getItem(EMAIL_KEY) : null);
  }

  /** Prima literă a emailului (pentru avatar) sau a rolului dacă nu e email. */
  getInitials(): string {
    const e = this.getEmail();
    if (e?.length) return e.charAt(0).toUpperCase();
    const r = this.roleSignal();
    if (r?.length) return r.charAt(0).toUpperCase();
    return '?';
  }

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  login(request: LoginRequest) {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, request).pipe(
      tap((res) => {
        if (res?.token) {
          localStorage.setItem(TOKEN_KEY, res.token);
          if (res?.role) localStorage.setItem(ROLE_KEY, res.role);
          if (res?.email) {
            localStorage.setItem(EMAIL_KEY, res.email);
            this.emailSignal.set(res.email);
          }
          this.tokenSignal.set(res.token);
          this.roleSignal.set(res?.role ?? null);
        }
      }),
      catchError((err) => of(null))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROLE_KEY);
    localStorage.removeItem(EMAIL_KEY);
    this.tokenSignal.set(null);
    this.roleSignal.set(null);
    this.emailSignal.set(null);
    this.router.navigate(['/login']);
  }

  getStoredToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getToken(): string | null {
    return this.tokenSignal() ?? this.getStoredToken();
  }

  getApiUrl(): string {
    return this.apiUrl;
  }
}
