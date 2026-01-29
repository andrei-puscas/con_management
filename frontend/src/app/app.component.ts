import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';
import { ZardButtonComponent } from '@/shared/components/button';
import { ZardAvatarComponent } from '@/shared/components/avatar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, ZardButtonComponent, ZardAvatarComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'ConManagement';
  private router = inject(Router);
  constructor(readonly auth: AuthService) {}

  get breadcrumbItems(): { label: string; link: string }[] {
    const url = this.router.url.split('?')[0];
    if (url.startsWith('/users')) return [{ label: 'Dashboard', link: '/dashboard' }, { label: 'Utilizatori', link: '/users' }];
    if (url.startsWith('/dashboard')) return [{ label: 'Dashboard', link: '/dashboard' }];
    if (url.startsWith('/login')) return [{ label: 'Autentificare', link: '/login' }];
    if (url.startsWith('/echipe')) return [{ label: 'Dashboard', link: '/dashboard' }, { label: 'Echipe', link: '/echipe' }];
    if (url.startsWith('/angajati')) return [{ label: 'Dashboard', link: '/dashboard' }, { label: 'Angajați', link: '/angajati' }];
    const proiecteMatch = url.match(/^\/proiecte\/(\d+)\/santier\/(\d+)$/);
    if (proiecteMatch) {
      const [, proiectId, santierId] = proiecteMatch;
      return [
        { label: 'Dashboard', link: '/dashboard' },
        { label: 'Proiecte', link: '/proiecte' },
        { label: 'Proiect #' + proiectId, link: '/proiecte/' + proiectId },
        { label: 'Șantier #' + santierId, link: url },
      ];
    }
    const proiectMatch = url.match(/^\/proiecte\/(\d+)$/);
    if (proiectMatch) {
      const id = proiectMatch[1];
      return [
        { label: 'Dashboard', link: '/dashboard' },
        { label: 'Proiecte', link: '/proiecte' },
        { label: 'Proiect #' + id, link: '/proiecte/' + id },
      ];
    }
    if (url.startsWith('/proiecte')) return [{ label: 'Dashboard', link: '/dashboard' }, { label: 'Proiecte', link: '/proiecte' }];
    return [{ label: 'Dashboard', link: '/dashboard' }];
  }
}
